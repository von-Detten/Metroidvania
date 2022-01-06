using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rigid;
    public BoxCollider2D box;
    private Animator playerAnimator;

    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
    public KeyCode moveJumpKey = KeyCode.Space;
    public KeyCode moveCrouchKey = KeyCode.LeftShift;
    public KeyCode moveDodgeKey = KeyCode.LeftControl;
    public KeyCode moveRunKey = KeyCode.LeftAlt;

    public float maxSpeed = 1.0f;
    public float acceleration = 0.1f;
    public float jumpStrenght = 1.0f;
    public float wallJumpStrength = 1.0f;

    public int numberOfDoubleJumps = 2;
    private int currentDoubleJumps = 0;

    public LayerMask ground;

    private bool isMovingLeft = false;
    private bool isMovingRight = false;
    private bool isFlipped = false;
    private float playerSpeed;

    void Start()
    {
        playerSpeed = maxSpeed;
        playerAnimator = gameObject.GetComponent<Animator>();
        rigid = gameObject.GetComponent<Rigidbody2D>();
        box = gameObject.GetComponent<BoxCollider2D>();
    }


    void Update()
    {
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
            Crouch();
        }
        else if (Input.GetKeyDown(moveRunKey))
        {
            Run();
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

        if (IsGrounded() || IsTouchingWallOnLeft() || IsTouchingWallOnRight())
        {
            currentDoubleJumps = numberOfDoubleJumps;
        }

        Float();
        Flip();
    }

    private void MoveLeft()
    {
        if(rigid.velocity.x >= -1 * playerSpeed)
        {
            playerAnimator.SetBool("IsWalking", true);
            rigid.velocity += new Vector2(-1 * playerSpeed * acceleration, 0);
            return;
        }
    }
    private void Run()
    {
        if (playerAnimator.GetBool("IsRunning"))
        {
            playerAnimator.SetBool("IsRunning", false);
            playerSpeed = maxSpeed;

        }
        else if (!playerAnimator.GetBool("IsRunning"))
        {
            playerAnimator.SetBool("IsCrouching", false);
            playerAnimator.SetBool("IsRunning", true);
            playerSpeed = maxSpeed * 3f;
        }
    }
    private void Dodge()
    {
        //TEMPORARY WEIRD BS BECAUSE TIRED AND I HATE MYSELF
        if (isMovingLeft && !isMovingRight)
        {
            playerAnimator.SetTrigger("Dodger");
        }
        else if (isMovingRight && !isMovingLeft)
        {
            playerAnimator.SetTrigger("Dodger");
        }
    }

    private void Crouch()
    {
        if (playerAnimator.GetBool("IsCrouching"))
        {
            playerAnimator.SetBool("IsCrouching", false);
            playerSpeed = maxSpeed;

        }
        else if (!playerAnimator.GetBool("IsCrouching"))
        {
            playerAnimator.SetBool("IsRunning", false);
            playerAnimator.SetBool("IsCrouching", true);
            playerSpeed = maxSpeed*0.5f;
        }
        

    }
    private void MoveRight()
    {
        if (rigid.velocity.x <= playerSpeed)
        {
            playerAnimator.SetBool("IsWalking", true);
            rigid.velocity += new Vector2(1 * playerSpeed * acceleration, 0);
            return;
        }
    }
    private void Float()
    {
        if (!IsGrounded())
        {
            playerAnimator.SetBool("IsFloat", true);
        }
        else playerAnimator.SetBool("IsFloat", false);
    }
        private void Jump()
    {
        if (IsGrounded())
        {
            playerAnimator.SetTrigger("Jumper");
            Debug.Log("Jump");
            rigid.velocity = new Vector2(rigid.velocity.x, jumpStrenght);
            return;
        }

        if (IsTouchingWallOnLeft() && !playerAnimator.GetBool("IsCrouching"))
        {
            playerAnimator.SetTrigger("Walljumper");
            Debug.Log("WallJumpLeft");
            rigid.velocity = new Vector2(1 * playerSpeed * wallJumpStrength, jumpStrenght);
            return;
        }

        if (IsTouchingWallOnRight() && !playerAnimator.GetBool("IsCrouching"))
        {
            playerAnimator.SetTrigger("Walljumper");
            Debug.Log("WallJumpRight");
            rigid.velocity = new Vector2(-1 * playerSpeed * wallJumpStrength, jumpStrenght);
            return;
        }

        if (currentDoubleJumps > 0 && !playerAnimator.GetBool("IsCrouching"))
        {
            playerAnimator.SetTrigger("DoubleJump");
            Debug.Log("DoubleJump");
            currentDoubleJumps--;
            rigid.velocity = new Vector2(rigid.velocity.x, jumpStrenght);
        }        
    }
    private void Flip()
    {
        Vector3 playerScale = transform.localScale;
        if (isMovingLeft && !isMovingRight && !isFlipped)
        {
            playerScale.x *= -1;
            transform.localScale = playerScale;
            isFlipped = true;

        }
        if (!isMovingLeft && isMovingRight && isFlipped)
        {
            playerScale.x *= -1;
            transform.localScale = playerScale;
            isFlipped = false;
        }
    }
    
    private bool IsGrounded()
    {
        float distanceToCheck = 0.1f;
        return Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0, Vector2.down, distanceToCheck, ground);
    }

    private bool IsTouchingWallOnLeft()
    {
        float distanceToCheck = 0.1f;
        return Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0, Vector2.left, distanceToCheck, ground);
    }

    private bool IsTouchingWallOnRight()
    {
        float distanceToCheck = 0.1f;
        return Physics2D.BoxCast(box.bounds.center, box.bounds.size, 0, Vector2.right, distanceToCheck, ground);
    }
}
