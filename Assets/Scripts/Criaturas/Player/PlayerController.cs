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
    public float staminaRunDrain = 10f; // Consumo por segundo ao correr

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
                gameObject.transform.eulerAngles = new Vector2(180, 0);
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

        // Recuperação de estamina quando parado
        if (!IsMoving && estamina.Energy < estamina.MaxEnergy)
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

                // Consome estamina ao correr
                if (IsRunning)
                {
                    estamina.Energy -= staminaRunDrain * Time.fixedDeltaTime;
                    if (estamina.Energy <= 0)
                    {
                        estamina.Energy = 0;
                        IsRunning = false; // Para de correr se acabar estamina
                    }
                }
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
        // Aguarda um tempo para a animação de comer maçã terminar
        yield return new WaitForSeconds(1.5f); // Ajuste este valor conforme a duração da animação
        
        // Desbloqueia o movimento
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

        if (coll.CompareTag("Trofeu1"))
        {
            StartCoroutine(GoFase2());
            Debug.Log("Player has completed fase 1");
        }
        // if (coll.CompareTag("WinBoss1"))
        // {
        //     StartCoroutine(GoFase2());
        //     Debug.Log("Player has won the boss 1");
        // }


        // FASE 2
        if (coll.CompareTag("OutMap"))
        {
            isAlive.IsAlive = false;
            Debug.Log("Player has left the map 2");
        }
        if (coll.CompareTag("Trofeu2"))
        {
            StartCoroutine(GoMenu());
            Debug.Log("Player has completed the fase 2");
        }
        // if (coll.CompareTag("WinBoss2"))
        // {
        //     StartCoroutine(Win());
        //     Debug.Log("Player has won the game");
        // }


        // // BOSSES
        // if (coll.CompareTag("Boss1"))
        // {
        //     isAlive.IsAlive = false;
        //     Debug.Log("Player has died by the boss 1");
        //     StartCoroutine(Death1());
        // }

        // if (coll.CompareTag("Boss2"))
        // {
        //     isAlive.IsAlive = false;
        //     Debug.Log("Player has died by the boss 2");
        //     StartCoroutine(Death2());
        // }


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
    IEnumerator GoMenu()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Menu");
    }


    // Ir para os bosses
    // IEnumerator GoBoss1()
    // {
    //     yield return new WaitForSeconds(2f);
    //     SceneManager.LoadScene("F1 - Boss");
    // }
    // IEnumerator GoBoss2()
    // {
    //     yield return new WaitForSeconds(2f);
    //     SceneManager.LoadScene("F2 - Boss");
    // }
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


    // // Mortes
    // IEnumerator Death1()
    // {
    //     yield return new WaitForSeconds(1f);
    //     SceneManager.LoadScene("F1");
    // }

    // IEnumerator Death2()
    // {
    //     yield return new WaitForSeconds(1f);
    //     SceneManager.LoadScene("F2");
    // }
}