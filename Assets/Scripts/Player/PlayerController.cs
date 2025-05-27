using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{

    public float walkSpeed;
    public float runSpeed;
    public float airWalkSpeed;
    public float jumpImpulse;
    Vector2 moveInput;
    TouchingDirection touchingDirection;
    Damageable damageable;

    public float CurrentMoveSpeed{
    get
    {
        if (CanMove)
        {
        if(IsMoving && !touchingDirection.IsOnWall)
            {
                if(touchingDirection.IsGround)
                {
                    if(IsRunning)
                    {
                        return runSpeed;
                    }
                    else
                    {
                        return walkSpeed;
                    }
                }
                else
                {
                    return airWalkSpeed;
                }
            }
            else
            {
                return 0;
            }
        } else
        {
            //movement locked
            return 0;
        }
        
    }
    }

    [SerializeField]
    private bool _isMoving = false;


    public bool IsMoving { get
    {
        return _isMoving;
    } 
    private set
    {
        _isMoving = value;
        animator.SetBool(AnimationStrings.isMoving,value);
    }
    }
    
    [SerializeField]
    private bool _isrunning = false;
    public bool IsRunning
    {
        get
        {
            return _isrunning;
        }
        set
        {
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    public bool _isFacingRight = true;

    public bool IsFacingRight { get {return _isFacingRight; } private set{
        if(_isFacingRight != value)
        {

            transform.localScale *= new Vector2(-1, 1);
        }
        _isFacingRight = value;
    
    
    } }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }

    Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirection>();
        damageable = GetComponent<Damageable>();
    }

    private void FixedUpdate()
    {
        if(!damageable.LockVelocity)
        {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if(IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;

            SetFacingDirection(moveInput);
        }
        else
        {
            IsMoving = false;
        }
    }

    private void SetFacingDirection(Vector2 moveInput)
    {
        if(moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if(moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started && touchingDirection.IsGround && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jump);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
        }
    }

    public void OnHit(int damage, Vector2 KnockBack)
    {
        rb.velocity = new Vector2(KnockBack.x, rb.velocity.y + KnockBack.y);
    }
}