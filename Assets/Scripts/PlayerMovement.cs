using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    /// Main script for the movement of the player.
    /// 
    /// Includes:
    /// movement in air and on ground
    /// sprinting
    /// sneaking
    /// dodging
    /// sneaking
    /// jump, doublejump, walljump
    /// </summary>
    public Rigidbody2D rigid;
    public BoxCollider2D box;
    private Animator playerAnimator;

    #region Config variables
    public float maxSpeed = 1.0f;
    public float acceleration = 0.1f;
    public float jumpStrenght = 1.0f;
    public float wallJumpStrength = 1.0f;
    public int numberOfDoubleJumps = 2;
    public float dogeForce = 10.0f;
    public float dogeTime = 0.2f;
    public float dogeCooldownTime = 2.0f;

    public float sprintSpeedFactor = 3.0f;
    public float sneakSpeedFactor = 0.65f;

    
    public LayerMask ground;

    private float dogeStart = 0.0f;
    private float playerSpeed;
    public int currentDoubleJumps = 0;
    #endregion

    #region Current state variables
    public bool isMovingLeft = false;
    public bool isMovingRight = false;
    public bool isFacingLeft = false;

    public bool isSneaking = false;
    public bool isSprinting = false;
    public bool isDogeing = false;

    private Vector3 defaultScale;
    #endregion


    void Start()
    {
        defaultScale = transform.localScale;
        playerSpeed = maxSpeed;
        playerAnimator = gameObject.GetComponent<Animator>();
        rigid = gameObject.GetComponent<Rigidbody2D>();
        box = gameObject.GetComponent<BoxCollider2D>();
    }
    void Update()
    {
        //Reset double Jumps
        if (IsGrounded() || IsTouchingWallOnLeft() || IsTouchingWallOnRight())
        {
            currentDoubleJumps = numberOfDoubleJumps;
        }

        //End doge
        if (isDogeing)
        {
            if (Time.fixedTime > dogeStart)
            {
                isDogeing = false;
                rigid.constraints = rigid.constraints & ~RigidbodyConstraints2D.FreezePositionY;
                rigid.velocity = Vector2.zero;
            }
        }

        //EndSprint
        if(!isMovingLeft && !isMovingRight)
        {
            CancelSprinting();
        }

        //Set player direction
        Flip();
        //Set sneaking/sprinting/walking
        SetMovementSpeed();
    }

    public void MoveLeft()
    {
        isMovingLeft = true;
        isFacingLeft = true;
        if(rigid.velocity.x >= -1 * playerSpeed)
        {
            rigid.AddForce(Vector2.left * acceleration);
            return;
        }
    }

    public void MoveRight()
    {
        isMovingRight = true;
        isFacingLeft = false;
        if (rigid.velocity.x <= playerSpeed)
        {
            rigid.AddForce(Vector2.right * acceleration);
            return;
        }
    }

    private void SetMovementSpeed()
    {
        if (isSprinting)
        {
            playerSpeed = maxSpeed * sprintSpeedFactor;
        }
        else if (isSneaking)
        {
            playerSpeed = maxSpeed * sneakSpeedFactor;
        }
        else
        {
            playerSpeed = maxSpeed;
        }
        return;
    }

    public void Jump()
    {
        if (IsGrounded()) //Normal Jump
        {
            rigid.AddForce(Vector2.up * jumpStrenght, ForceMode2D.Impulse);
            return;
        }

        if (IsTouchingWallOnLeft() && !isSneaking) //Wall Jump left side
        {
            rigid.velocity = new Vector2(1 * playerSpeed * wallJumpStrength, jumpStrenght);
            return;
        }

        if (IsTouchingWallOnRight() && !isSneaking) //Wall Jump left side
        {
            rigid.velocity = new Vector2(-1 * playerSpeed * wallJumpStrength, jumpStrenght);
            return;
        }

        if (currentDoubleJumps > 0 && !isSneaking) //Double Jump
        {
            currentDoubleJumps--;
            rigid.velocity = new Vector2(rigid.velocity.x, jumpStrenght);
        }        
    }

    //Orient Player into the right direction
    private void Flip()
    {
        if (isFacingLeft)
        {        
            transform.localScale = new Vector3(-1 * defaultScale.x, defaultScale.y, defaultScale.z);
        }
        else
        {
            transform.localScale = defaultScale;
        }
    }

    public void Sneak()
    {
        isSneaking = true;
        isSprinting = false;
    }

    public void CancelSneaking()
    {
        isSneaking = false;
    }

    public void Sprint()
    {
        isSprinting = true;
        isSneaking = false;
    }

    public void CancelSprinting()
    {
        isSprinting = false;
    }

    //Starts dodge and sets cooldown timer
    public void Dodge()
    {
        if(dogeStart + dogeCooldownTime > Time.fixedTime)
        {
            return;
        }
        if (isDogeing)
        {
            return;
        }
        isDogeing = true;
        transform.position += Vector3.up * 0.01f;
        rigid.constraints = rigid.constraints | RigidbodyConstraints2D.FreezePositionY;
        dogeStart = Time.fixedTime + dogeTime;
        if (isFacingLeft)
        {
            rigid.velocity = new Vector2(-1 * dogeForce, 0);
        }
        else
        {
            rigid.velocity = new Vector2(1 * dogeForce, 0);
        }
    }


    #region position checks
    public bool IsGrounded()
    {
        float distanceToCheck = 0.1f;
        return Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0, Vector2.down, distanceToCheck, ground);
    }
    public bool IsTouchingWallOnLeft()
    {
        float distanceToCheck = 0.1f;
        return Physics2D.BoxCast(box.bounds.center, new Vector2(box.bounds.size.x, box.bounds.size.y / 2), 0, Vector2.left, distanceToCheck, ground);
    }
    public bool IsTouchingWallOnRight()
    {
        float distanceToCheck = 0.1f;
        return Physics2D.BoxCast(box.bounds.center, new Vector2(box.bounds.size.x, box.bounds.size.y / 2), 0, Vector2.right, distanceToCheck, ground);
    }
    #endregion
}
