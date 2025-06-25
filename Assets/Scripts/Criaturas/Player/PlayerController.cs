using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{

    public float walkSpeed;
    public float runSpeed;
    public float airWalkSpeed;
    public float jumpImpulse;
    public bool canAttack = true;
    Vector2 moveInput;
    TouchingDirection touchingDirection;
    Damageable damageable;
    public ProjectileLauncher projectileLauncher;
    public Personagem NPCScript;
    public GameObject caixaDialogo;

    // codigo que define o movimento do player true or false
    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirection.IsOnWall)
                {
                    if (touchingDirection.IsGround)
                    {
                        if (IsRunning)
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
            }
            else
            {
                //movement locked
                return 0;
            }

        }
    }

    [SerializeField]
    private bool _isMoving = false;

    //detecta movimentação
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    // detecta se o player is running
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

    //faz o player inverter de lado
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {

                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;


        }
    }

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
        if (!damageable.LockVelocity)
        {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        if (IsAlive)
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
        if (moveInput.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (moveInput.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }
    //corrida do player
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }
    //Pulo do player
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirection.IsGround && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jump);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }
    //attack
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && caixaDialogo.activeSelf == false && canAttack)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }
    //Area de attack
    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (context.started && projectileLauncher.canFire)
        {
            animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
        }
    }
    //set the movimentation in relation of the HIT
    public void OnHit(int damage, Vector2 KnockBack)
    {
        rb.velocity = new Vector2(KnockBack.x, rb.velocity.y + KnockBack.y);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("NPC"))
        {
            Debug.Log("Player entrou na área de interação com NPC");
            NPCScript.podeInteragir = true;
            projectileLauncher.canFire = false;
            canAttack = false;

        }
    }
    
    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.CompareTag("NPC"))
        {
            Debug.Log("Player saiu da área de interação com NPC");
            NPCScript.podeInteragir = false;
            projectileLauncher.canFire = true;
            canAttack = true;
        }
    }
}