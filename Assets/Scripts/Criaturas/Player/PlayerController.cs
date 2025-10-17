using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Mathematics;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    [Header("Component References")]
    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;
    public Animator animator;
    public TouchingDirection touchingDirection;
    public Damageable damageable;
    public Damageable isAlive;
    public ProjectileLauncher projectileLauncher;
    public Estamina estamina; // Referência ao script de estamina

    [Header("Movement Settings")]
    public float walkSpeed;
    public float runSpeed;
    public float airWalkSpeed;
    public float jumpImpulse;
    [Range(1f, 20f)]
    public float acceleration = 5f; // Aceleração gradual ao andar (editável pelo Inspector)
    private float currentSpeed = 0f; // Velocidade atual para aceleração
    public Vector2 moveInput;

    [Header("Attack Settings")]
    public float delayBow;
    public float delayMorte, delayWin;
    public float staminaAttackDrain = 25f; // Consumo por ataque de espada
    public float staminaRangedDrain = 10f; // Consumo por ataque de arco
    public float staminaRunDrain; // Consumo por segundo ao correr

    [Header("Stamina Settings")]
    public float staminaRecoverRate = 15f; // Recuperação por segundo parado
    
    [Header("Damage Settings")]
    public int baseMeleeDamage = 10; // Dano base do ataque corpo a corpo
    public int baseProjectileDamage = 10; // Dano base dos projéteis
    public int currentMeleeDamage = 10; // Dano atual do ataque corpo a corpo
    public int currentProjectileDamage = 10; // Dano atual dos projéteis

    [Header("State Bools")]
    public bool isInDelayBow;
    public bool canAttack = true;
    public bool canFlip = true;
    public bool IsGrounded;

    [SerializeField]
    private bool _isMoving = false;
    [SerializeField]
    private bool _isrunning = false;
    public bool _isFacingRight = true;

    [Header("NPC & UI")]
    public Personagem[] NPCScript;
    public GameObject caixaDialogo;
    public GameObject[] eKey;
    public string cenaAtual;

    [Header("Audios")]
    public AudioSource Andar;
    public AudioSource Pulo;
    public AudioSource Ataque;
    public AudioSource Hit;

    [Header("Imports")]
    public GameObject MenudePausa;

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
    public bool IsRunning
    {
        get
        {
            return _isrunning;
        }
        set
        {
            _isrunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);   
        }
    }

    //faz o player inverter de lado
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value && canFlip)
            {
                // flip only if allowed
            }
            _isFacingRight = value;
            gameObject.transform.eulerAngles = new Vector2(0, _isFacingRight ? 0 : 180);
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

    private static PlayerController _instance;
    public static PlayerController Instance => _instance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirection = GetComponent<TouchingDirection>();
        damageable = GetComponent<Damageable>();
        isAlive = GetComponent<Damageable>();
        estamina = GetComponent<Estamina>();
        foreach (Personagem npc in NPCScript)
        {
            npc.podeInteragir = false;
        }
        foreach (GameObject key in eKey)
        {
            key.SetActive(false);
        }
    }
    
    void Start()
    {
        // Inicializa os valores de dano
        currentMeleeDamage = baseMeleeDamage;
        currentProjectileDamage = baseProjectileDamage;
        Debug.Log($"Dano inicializado - Melee: {currentMeleeDamage}, Projétil: {currentProjectileDamage}");
    }

    private void Update()
    {
        cenaAtual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if (!isAlive.IsAlive)
        {
            StartCoroutine(GoCena(cenaAtual));
        }

        // Pausa animações se o menu de pausa estiver ativo
        animator.speed = MenudePausa.activeSelf ? 0f : 1f;

        // Recuperação de estamina sempre que estiver abaixo do máximo
        if (estamina.Energy < estamina.MaxEnergy)
        {
            estamina.Energy += staminaRecoverRate * Time.deltaTime;
            estamina.Energy = Mathf.Min(estamina.Energy, estamina.MaxEnergy);
        }

        if (IsMoving && touchingDirection.IsGround && !Andar.isPlaying)
        {
            Andar.PlayOneShot(Andar.clip);
        }
        else if ((!IsMoving || !touchingDirection.IsGround) && Andar.isPlaying)
        {
            Andar.Stop();
        }
        
    }

    private void FixedUpdate()
    {
        if (!damageable.LockVelocity)
        {
            // Aceleração gradual ao andar
            if (IsMoving && estamina.Energy > 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, CurrentMoveSpeed, acceleration * Time.fixedDeltaTime);
                rb.velocity = new Vector2(moveInput.x * currentSpeed, rb.velocity.y);
                // Não consome estamina ao andar ou correr
            }
            else
            {
                currentSpeed = 0f;
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    #region Audio Camolezed
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
            Pulo.PlayOneShot(Pulo.clip);
        }
    }
    //attack
    public void OnAttack(InputAction.CallbackContext context)
    {
        // Bloqueia ataque se o menu de pausa estiver ativo
        if (MenudePausa.activeSelf) return;
        if (context.started && caixaDialogo.activeSelf == false && canAttack)
        {
            if (estamina.Energy >= staminaAttackDrain)
            {
                estamina.Energy -= staminaAttackDrain;
                animator.SetTrigger(AnimationStrings.attackTrigger);
                Ataque.PlayOneShot(Ataque.clip);
            }
            // else: sem estamina, não ataca
        }
    }

    #endregion
    //Area de attack
    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        // Bloqueia ataque se o menu de pausa estiver ativo
        if (MenudePausa.activeSelf) return;
        if (context.started && projectileLauncher.canFire && !isInDelayBow)
        {
            if (estamina.Energy >= staminaRangedDrain)
            {
                estamina.Energy -= staminaRangedDrain;
                animator.SetTrigger(AnimationStrings.rangedAttackTrigger);
                StartCoroutine(BowDelayCoroutine());
            }
            // else: sem estamina, não ataca
        }
    }

    private IEnumerator BowDelayCoroutine()
    {
        isInDelayBow = true;
        yield return new WaitForSeconds(delayBow);
        isInDelayBow = false;
    }
    
    /// <summary>
    /// Aumenta o dano de todos os ataques do player
    /// </summary>
    /// <param name="increaseAmount">Quantidade a ser aumentada</param>
    public void IncreaseAllDamage(int increaseAmount)
    {
        Debug.Log($"IncreaseAllDamage chamado com {increaseAmount}");
        
        // Aumenta dano de todos os ataques corpo a corpo
        Attack[] attacks = FindObjectsOfType<Attack>();
        foreach (Attack attack in attacks)
        {
            attack.IncreaseDamage(increaseAmount);
        }
        
        // Aumenta dano de todos os projéteis
        PlayerProjectile[] projectiles = FindObjectsOfType<PlayerProjectile>();
        foreach (PlayerProjectile projectile in projectiles)
        {
            projectile.IncreaseDamage(increaseAmount);
        }
        
        // Atualiza os valores globais para referência
        currentMeleeDamage += increaseAmount;
        currentProjectileDamage += increaseAmount;
        
        Debug.Log($"Dano global aumentado! Melee: {currentMeleeDamage}, Projétil: {currentProjectileDamage}");
    }
    
    /// <summary>
    /// Desbloqueia movimento após animação de comer maçã
    /// </summary>
    public IEnumerator UnlockMovementAfterAnimation()
    {
        // Força Stella no Idle usando apenas isMoving e isRunning
        animator.SetBool(AnimationStrings.isMoving, false);
        animator.SetBool(AnimationStrings.isRunning, false);
        // Aguarda o tempo da animação de comer maçã
        yield return new WaitForSeconds(1.5f); // Ajuste conforme necessário

        // Libera o movimento
        if (damageable != null)
        {
            damageable.LockVelocity = false;
        }
    }

    //set the movimentation in relation of the HIT
    public void OnHit(int damage, Vector2 KnockBack)
    {
        rb.velocity = new Vector2(KnockBack.x, rb.velocity.y + KnockBack.y);
        Hit.Play();
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

        #region Vitorias 

        //Fase 1
        if (coll.CompareTag("Trofeu1"))
        {
            StartCoroutine(GoCena("F1 - Boss"));
            Debug.Log("Player has completed fase 1");
        }

        if (coll.CompareTag("TrofeuTo2"))
        {
            StartCoroutine(GoCena("F2"));
            Debug.Log("Player has completed fase 1 e derrotou o Boss Lagarto");
        }

        //Fase 2
        if (coll.CompareTag("Trofeu2"))
        {
            StartCoroutine(GoCena("F2 - Boss"));
            Debug.Log("Player has completed the fase 2");
        }

        if (coll.CompareTag("TrofeuTo3"))
        {
            StartCoroutine(GoCena("F3"));
            Debug.Log("Player has completed fase 2 e derrotou o Boss Cachorro");
        }

        //Fase 3
        if (coll.CompareTag("Trofeu3"))
        {
            StartCoroutine(GoCena("F3 - Boss"));
            Debug.Log("Player has completed the fase 3");
        }

        if (coll.CompareTag("TrofeuTo4"))
        {
            StartCoroutine(GoCena("WinScreen"));
            Debug.Log("Player has completed the game");
        }

        #endregion
    }


    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("Ground"))
        {
            IsGrounded = true;
            animator.SetBool(AnimationStrings.isGrounded, true);
            animator.ResetTrigger(AnimationStrings.jump);
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("Ground"))
        {
            IsGrounded = false;
            animator.SetBool(AnimationStrings.isGrounded, false);
        }
    }

    IEnumerator GoMenu()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Menu");
    }

    IEnumerator GoCena(string cena)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(cena);
    }

    // WIN
    IEnumerator Win()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("WinScreen");
    }
}