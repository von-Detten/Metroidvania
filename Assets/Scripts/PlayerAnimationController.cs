using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    /// <summary>
    /// Used to offer a callable script for the animator of the player.
    /// 
    /// Decides current animation state if no inputs are given.
    /// </summary>
    public Animator ani = null;

    private bool wasGrounded = false;

    public void UpdateAnimator(PlayerMovement move, bool isGrappled)
    {
        if (move.isDogeing)
        {
            ani.SetBool("IsDodging", true);
            return;
        }
        else
        {
            ani.SetBool("IsDodging", false);
        }

        if(!move.isMovingLeft && !move.isMovingRight)
        {
            ani.SetBool("IsWalking", false);
            ani.SetBool("IsRunning", false);
        }

        if (!move.IsGrounded())
        {
            //Floating
            if (!WallSlide(move))
            {
                wasGrounded = false;
                //Ceiling
                if (move.IsTouchingCeiling() && isGrappled)
                {
                    ani.SetBool("IsTopwall", true);
                    return;
                }
                else
                {
                    ani.SetBool("IsTopwall", false);
                }
                ani.SetBool("IsFloat", true);
            }
            else
            {
                ani.SetBool("IsTopwall", false);
                wasGrounded = true;
            }
        }
        else
        {
            ani.SetBool("IsTopwall", false);
            ani.SetBool("IsWallslide", false);
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
        if((move.IsTouchingWallOnLeft() || move.IsTouchingWallOnRight()) && !move.IsGrounded())
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
