using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rigid;
    public BoxCollider2D box;
    private Animator playerAnimator;

    //public KeyCode moveLeftKey = KeyCode.A;
    //public KeyCode moveRightKey = KeyCode.D;
    //public KeyCode moveJumpKey = KeyCode.Space;
    //public KeyCode moveCrouchKey = KeyCode.LeftShift;
    //public KeyCode moveDodgeKey = KeyCode.LeftControl;
    //public KeyCode moveRunKey = KeyCode.LeftAlt;

    #region Config variables
    public float maxSpeed = 1.0f;
    public float acceleration = 0.1f;
    public float jumpStrenght = 1.0f;
    public float wallJumpStrength = 1.0f;
    public int numberOfDoubleJumps = 2;
    public float dogeForce = 10.0f;
    public float dogeTime = 0.2f;
    public float sprintSpeedFactor = 3.0f;
    public float sneakSpeedFactor = 0.65f;

    
    public LayerMask ground;

    private float dogeStart = 0.0f;
    private float playerSpeed;
    public int currentDoubleJumps = 0;
    #endregion

    #region Current state variables
    public bool isMovingLeft = false; //TODO replace with walking
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
        /*
        if (Input.GetKeyDown(moveLeftKey) || Input.GetKey(moveLeftKey))
        {
            isMovingLeft = true;
        }
        else
        {
            isMovingLeft = false;
        }
        if (Input.GetKeyDown(moveRightKey) || Input.GetKey(moveRightKey))
        {
            isMovingRight = true;
        }
        else
        {
            isMovingRight = false;
        }
        if (Input.GetKeyDown(moveJumpKey))
        {
            Jump();
        }
        else if (Input.GetKeyDown(moveCrouchKey))
        {
            Sneak();
        }
        else if (Input.GetKeyDown(moveRunKey))
        {
            Sprint();
        }
        else if (Input.GetKeyDown(moveDodgeKey))
        {
            Dodge();
        }

        else if (isMovingLeft)
        {
            MoveLeft();
        }
        else if (isMovingRight)
        {
            MoveRight();
        }
        else if (!isMovingRight && !isMovingLeft)
        {
            playerAnimator.SetBool("IsWalking", false);
        }
        */

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

        //TODO: Remove
        //Float();
        Flip();
        //WallSlide();
        SetMovementSpeed();
    }

    public void MoveLeft()
    {
        if(rigid.velocity.x >= -1 * playerSpeed)
        {
            //playerAnimator.SetBool("IsWalking", true);
            isMovingLeft = true;
            isFacingLeft = true;
            rigid.AddForce(Vector2.left * acceleration);
            return;
        }
    }
    public void MoveRight()
    {
        if (rigid.velocity.x <= playerSpeed)
        {
            //playerAnimator.SetBool("IsWalking", true);
            isMovingRight = true;
            isFacingLeft = false;
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
        if (IsGrounded())
        {
            //playerAnimator.SetTrigger("Jumper");
            //rigid.velocity = new Vector2(rigid.velocity.x, jumpStrenght);
            rigid.AddForce(Vector2.up * jumpStrenght, ForceMode2D.Impulse);
            return;
        }

        if (IsTouchingWallOnLeft() && !isSneaking)
        {
            //playerAnimator.SetTrigger("Walljumper");
            rigid.velocity = new Vector2(1 * playerSpeed * wallJumpStrength, jumpStrenght);
            return;
        }

        if (IsTouchingWallOnRight() && !isSneaking)
        {
            //playerAnimator.SetTrigger("Walljumper");
            rigid.velocity = new Vector2(-1 * playerSpeed * wallJumpStrength, jumpStrenght);
            return;
        }

        if (currentDoubleJumps > 0 && !isSneaking)
        {
            //playerAnimator.SetTrigger("DoubleJump");
            currentDoubleJumps--;
            rigid.velocity = new Vector2(rigid.velocity.x, jumpStrenght);
            //rigid.AddForce(Vector2.up * jumpStrenght, ForceMode2D.Impulse); Doesnt reset vertical momentum
        }        
    }
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
    #region animator
    /*
    private void WallSlide()
    {
        if (!IsGrounded() && (IsTouchingWallOnLeft() || IsTouchingWallOnRight()))
        {
            playerAnimator.SetBool("IsWallslide", true);
            playerAnimator.SetBool("IsRunning", false);
        }
        else playerAnimator.SetBool("IsWallslide", false);
    }
    */
    /*
    private void Float()
    {
        if (!IsGrounded())
        {
            playerAnimator.SetBool("IsFloat", true);
        }
        else playerAnimator.SetBool("IsFloat", false);
    }
    */
    public void Sneak()
    {
        isSneaking = true;
        isSprinting = false;
        /*
        if (playerAnimator.GetBool("IsCrouching"))
        {
            playerAnimator.SetBool("IsCrouching", false);

        }
        else if (!playerAnimator.GetBool("IsCrouching"))
        {
            playerAnimator.SetBool("IsRunning", false);
            playerAnimator.SetBool("IsCrouching", true);
        }
        */
    }

    public void CancelSneaking()
    {
        isSneaking = false;
    }

    public void Sprint()
    {
        isSprinting = true;
        isSneaking = false; //TODO: Add Check for Height
        /*
        if (playerAnimator.GetBool("IsRunning"))
        {
            playerAnimator.SetBool("IsRunning", false);

        }
        else if (!playerAnimator.GetBool("IsRunning"))
        {
            playerAnimator.SetBool("IsCrouching", false);
            playerAnimator.SetBool("IsRunning", true);

        }
        */
    }

    public void CancelSprinting()
    {
        isSprinting = false;
    }

    public void Dodge()
    {
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
        //TODO: Remove
        playerAnimator.SetTrigger("Dodger");
    }
    #endregion


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
