using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{

    public float walkSpeed;
    public float runSpeed;
    public float airWalkSpeed;
    public float jumpImpulse;
    public bool canAttack = true;
    public float delayMorte, delayWin;
    Vector2 moveInput;
    TouchingDirection touchingDirection;
    Damageable damageable;
    public bool canFlip = true;
    Damageable isAlive;
    public ProjectileLauncher projectileLauncher;
    public Personagem[] NPCScript;
    public GameObject caixaDialogo;
    public GameObject[] eKey;

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
                    //movement locked
                    return 0;
                }
            }
            else
            {
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
            if (_isFacingRight != value && canFlip)
            {

                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;


        }
    }

    //bool da animação de movimento
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
        isAlive = GetComponent<Damageable>();
        foreach (Personagem npc in NPCScript)
        {
            npc.podeInteragir = false;
        }
        foreach (GameObject key in eKey)
        {
            key.SetActive(false);
        }
    }

    private void Update()
    {

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
            Personagem npc = coll.GetComponent<Personagem>();
            if (npc != null)
            {
                npc.podeInteragir = true;
            }
            projectileLauncher.canFire = false;
            canAttack = false;
            foreach (GameObject key in eKey)
            {
                key.SetActive(true);
            }
        }

        // FASE 1
        if (coll.CompareTag("OutMap1"))
        {
            isAlive.IsAlive = false;
            Debug.Log("Player has left the map 1");
            StartCoroutine(Death1());
        }
        if (coll.CompareTag("Trofeu1"))
        {
            StartCoroutine(GoBoss1());
            Debug.Log("Player has completed fase 1");
        }
        if (coll.CompareTag("WinBoss1"))
        {
            StartCoroutine(GoFase2());
            Debug.Log("Player has won the boss 1");
        }


        // FASE 2
        if (coll.CompareTag("OutMap2"))
        {
            isAlive.IsAlive = false;
            Debug.Log("Player has left the map 2");
            StartCoroutine(Death2());
        }
        if (coll.CompareTag("Trofeu2"))
        {
            StartCoroutine(GoBoss2());
            Debug.Log("Player has completed the fase 2");
        }
        if (coll.CompareTag("WinBoss2"))
        {
            StartCoroutine(Win());
            Debug.Log("Player has won the game");
        }


        // BOSSES
        if (coll.CompareTag("Boss1"))
        {
            isAlive.IsAlive = false;
            Debug.Log("Player has died by the boss 1");
            StartCoroutine(Death1());
        }

        if (coll.CompareTag("Boss2"))
        {
            isAlive.IsAlive = false;
            Debug.Log("Player has died by the boss 2");
            StartCoroutine(Death2());
        }


        /*
        if (coll.CompareTag("Boss3"))
        {
            isAlive.IsAlive = false;
            Debug.Log("Player has died by the boss 3");
            StartCoroutine(Death3());
        }
        if (coll.CompareTag("Boss4"))
        {
            isAlive.IsAlive = false;
            Debug.Log("Player has died by the boss 4");
            StartCoroutine(Death4());
        }
        */
        
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.CompareTag("NPC"))
        {
            Personagem npc = coll.GetComponent<Personagem>();
            if (npc != null)
            {
                npc.podeInteragir = false;
            }
            projectileLauncher.canFire = true;
            canAttack = true;
            foreach (GameObject key in eKey)
            {
                key.SetActive(false);
            }
        }
    }


    IEnumerator GoFase2()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("F2");
    }


    // Ir para os bosses
    IEnumerator GoBoss1()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("F1 - Boss");
    }
    IEnumerator GoBoss2()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("F2 - Boss");
    }


    // WIN
    IEnumerator Win()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("WinScreen");
    }


    // Mortes
    IEnumerator Death1()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("F1");
    }

    IEnumerator Death2()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("F2");
    }
}