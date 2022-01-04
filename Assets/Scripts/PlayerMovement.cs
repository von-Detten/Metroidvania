using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rigid;
    public BoxCollider2D box;

    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;
    public KeyCode moveJumpKey = KeyCode.Space;
    public KeyCode moveCrouchKey = KeyCode.LeftShift;

    public float maxSpeed = 1.0f;
    public float acceleration = 0.1f;
    public float jumpStrenght = 1.0f;
    public float wallJumpStrength = 1.0f;

    public int numberOfDoubleJumps = 2;
    private int currentDoubleJumps = 0;

    public LayerMask ground;

    private bool isMovingLeft = false;
    private bool isMovingRight = false;

    void Start()
    {
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


        else if (isMovingLeft)
        {
            MoveLeft();
        }
        else if (isMovingRight)
        {
            MoveRight();
        }

        if (IsGrounded() || IsTouchingWallOnLeft() || IsTouchingWallOnRight())
        {
            currentDoubleJumps = numberOfDoubleJumps;
        }
    }

    private void MoveLeft()
    {
        if(rigid.velocity.x >= -1 * maxSpeed)
        {
            rigid.velocity += new Vector2(-1 * maxSpeed * acceleration, 0);
            return;
        }
    }
    private void MoveRight()
    {
        if (rigid.velocity.x <= maxSpeed)
        {
            rigid.velocity += new Vector2(1 * maxSpeed * acceleration, 0);
            return;
        }
    }
    private void Jump()
    {
        if (IsGrounded())
        {
            Debug.Log("Jump");
            rigid.velocity = new Vector2(rigid.velocity.x, jumpStrenght);
            return;
        }

        if (IsTouchingWallOnLeft())
        {
            Debug.Log("WallJumpLeft");
            rigid.velocity = new Vector2(1 * maxSpeed * wallJumpStrength, jumpStrenght);
            return;
        }

        if (IsTouchingWallOnRight())
        {
            Debug.Log("WallJumpRight");
            rigid.velocity = new Vector2(-1 * maxSpeed * wallJumpStrength, jumpStrenght);
            return;
        }

        if (currentDoubleJumps > 0)
        {
            Debug.Log("DouleJump");
            currentDoubleJumps--;
            rigid.velocity = new Vector2(rigid.velocity.x, jumpStrenght);
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
