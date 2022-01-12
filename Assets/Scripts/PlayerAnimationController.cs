using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator ani = null;

    private bool wasGrounded = false;

    public void UpdateAnimator(PlayerMovement move)
    {
        if (move.isDogeing)
        {
            ani.SetTrigger("Dodger");
            return;
        }
        else
        {
            //Implement Dogeing as bool
        }

        if(!move.isMovingLeft && !move.isMovingRight)
        {
            ani.SetBool("IsWalking", false);
            ani.SetBool("IsRunning", false);
        }

        if (!move.IsGrounded())
        {
            if (!WallSlide(move))
            {
                ani.SetBool("IsFloat", true);
                wasGrounded = false;
            }
            else
            {
                wasGrounded = true;
            }
        }
        else
        {
            if (wasGrounded)
            {
                ani.SetBool("IsFloat", false);
            }
            else
            {
                FloorImpact();
                wasGrounded = true;
            }
        }
    }

    public void Walk(PlayerMovement move)
    {
        if (!move.IsGrounded())
        {
            ani.SetBool("IsFloat", true);
            return;
        }
        if (move.isSprinting)
        {
            ani.SetBool("IsRunning", true);
            ani.SetBool("IsWalking", true);
        }
        else if (move.isSneaking)
        {
            return;
        }
        else
        {
            DisableAll();
            ani.SetBool("IsWalking", true);
        }
    }

    public void Sneak(bool bo)
    {
        ani.SetBool("IsCrouching", bo);
    }

    public void Roped()
    {
        //TODO: Implement
    }

    public void Grappled()
    {
        //TODO: Implement
    }

    public bool WallSlide(PlayerMovement move)
    {
        if(move.IsTouchingWallOnLeft() || move.IsTouchingWallOnRight())
        {
            ani.SetBool("IsWallslide", true);
            return true;
        }
        else
        {
            ani.SetBool("IsWallslide", false);
            return false;
        }
    }

    public void CeilingImpact()
    {
        //TODO: Implement
    }

    public void FloorImpact()
    {
        //TODO: Implement
    }

    public void Jump(PlayerMovement move)
    {
        if (move.IsGrounded())
        {
            ani.SetTrigger("Jumper");
        }
        else if ((move.IsTouchingWallOnLeft() || move.IsTouchingWallOnRight() && !move.isSneaking))
        {
            ani.SetTrigger("Walljumper");
        }
        else if (move.currentDoubleJumps > 0 && !move.isSneaking)
        {
            ani.SetTrigger("DoubleJump");
        }
    }

    public void DisableAll()
    {
        ani.SetBool("IsFloat", false);
        ani.SetBool("IsWalking", false);
        ani.SetBool("IsRunning", false);
        ani.SetBool("IsCrouching", false);
        ani.SetBool("IsWallslide", false);
    }
}
