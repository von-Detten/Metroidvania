using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public KeyCode leftKC;
    public KeyCode rightKC;
    public KeyCode jumpKC;
    public KeyCode grappleKC;
    public KeyCode ropeKC;
    public KeyCode sprintKC;
    public KeyCode dogeKC;
    public KeyCode sneakKC;

    public SwingingRope rope;
    public GrapplingHook grapple;
    public PlayerMovement movement;
    public PlayerAnimationController ani;

    void Start()
    {
        if(rope == null || grapple == null || movement == null || ani == null)
        {
            Debug.LogError("Critical script not assigned in PlayerController.");
        }
    }
    //KeyCode Checks
    void Update()
    {
        if (Input.GetKeyDown(sneakKC))
        {
            Sneak();
        }

        if (Input.GetKeyDown(dogeKC))
        {
            Doge();
        }

        if (Input.GetKeyDown(ropeKC))
        {
            Rope();
        }

        if (Input.GetKeyDown(grappleKC))
        {
            Grapple();
        }

        if (Input.GetKeyDown(jumpKC))
        {
            Jump();
        }

        if(Input.GetKeyDown(leftKC) || Input.GetKey(leftKC))
        {
            if(!(Input.GetKeyDown(rightKC) || Input.GetKey(rightKC)))
            {
                if (Input.GetKeyDown(sprintKC))
                {
                    Sprint();
                }
                MoveLeft();
            }
            else
            {
                movement.isMovingLeft = false;
            }
        }
        else
        {
            movement.isMovingLeft = false;
        }

        if (Input.GetKeyDown(rightKC) || Input.GetKey(rightKC))
        {
            if (!(Input.GetKeyDown(leftKC) || Input.GetKey(leftKC)))
            {
                if (Input.GetKeyDown(sprintKC))
                {
                    Sprint();
                }
                MoveRight();
            }
            else
            {
                movement.isMovingRight = false;
            }
        }
        else
        {
            movement.isMovingRight = false;
        }

        ani.UpdateAnimator(movement);
    }

    #region Movement Options
    private void MoveLeft()
    {
        if (grapple.isAttatched || movement.isDogeing) return;
        movement.MoveLeft();
        ani.Walk(movement);
    }

    private void MoveRight()
    {
        if (grapple.isAttatched || movement.isDogeing) return;
        movement.MoveRight();
        ani.Walk(movement);
    }

    private void Jump()
    {
        if (movement.isDogeing) return;
        rope.Release();
        grapple.Release();

        movement.Jump();
        ani.Jump(movement);
    }

    private void Grapple()
    {
        movement.CancelSneaking();
        ani.Sneak(movement.isSneaking);
        movement.CancelSprinting();
        grapple.Attatch();
        if(grapple.anchor.transform.position.x > movement.transform.position.x)
        {
            movement.isFacingLeft = false;
        }
        else
        {
            movement.isFacingLeft = true;
        }
    }

    private void Rope()
    {
        movement.CancelSneaking();
        ani.Sneak(movement.isSneaking);
        movement.CancelSprinting();
        rope.Attatch();
    }

    private void Sprint()
    {
        if (grapple.isAttatched || rope.isAttatched) return;
        movement.Sprint();
    }

    private void Doge()
    {
        if (grapple.isAttatched || rope.isAttatched) return;
        movement.Dodge();
    }

    private void Sneak()
    {
        if (grapple.isAttatched || rope.isAttatched) return;
        if (movement.isSneaking)
        {
            movement.CancelSneaking();
            ani.Sneak(movement.isSneaking);
        }
        else
        {
            movement.Sneak();
            ani.Sneak(movement.isSneaking);
        }
    }
    #endregion
}
