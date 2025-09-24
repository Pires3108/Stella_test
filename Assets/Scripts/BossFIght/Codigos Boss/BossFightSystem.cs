using System.Collections;
using UnityEngine;

/// <summary>
/// Sistema principal de combate do boss - Coordena todos os componentes
/// </summary>
[RequireComponent(typeof(BossStateMachine))]
[RequireComponent(typeof(BossMovement))]
[RequireComponent(typeof(BossAttackSystem))]
[RequireComponent(typeof(BossHealthSystem))]
public class BossFightSystem : MonoBehaviour
{
    [Header("Referências")]
    public GameObject player;
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    
    [Header("Preset do Boss (Opcional)")]
    public BossPreset bossPreset;
    
    [Header("Configurações de IA")]
    public bool aggressiveMode = true;
    
    // Componentes do sistema
    private BossStateMachine stateMachine;
    private BossMovement movement;
    private BossAttackSystem attackSystem;
    private BossHealthSystem healthSystem;
    
    // Eventos públicos (para compatibilidade)
    public System.Action<BossAttack> OnAttackStart;
    public System.Action<BossAttack> OnAttackEnd;
    public System.Action OnBossDeath;
    public System.Action<float> OnHealthChanged;

    void Start()
    {
        InitializeBoss();
    }

    void Update()
    {
        if (healthSystem != null && healthSystem.IsDead) return;
        
        UpdateAnimations();
        healthSystem?.UpdateHealthBar();
    }

    void InitializeBoss()
    {
        // Inicializa os componentes
        InitializeComponents();
        
        // Aplica preset se configurado
        if (bossPreset != null)
        {
            ApplyPreset(bossPreset);
        }
        
        // Configura ataques padrão se nenhum foi configurado
        if (attackSystem != null && attackSystem.GetTotalAttacks() == 0)
        {
            attackSystem.SetupDefaultAttacks();
        }
        
        // Conecta eventos
        ConnectEvents();
        
        // Inicia o ciclo de combate
        StartCoroutine(BossFightCycle());
    }
    
    void InitializeComponents()
    {
        // Obtém os componentes (RequireComponent garante que existam)
        stateMachine = GetComponent<BossStateMachine>();
        movement = GetComponent<BossMovement>();
        attackSystem = GetComponent<BossAttackSystem>();
        healthSystem = GetComponent<BossHealthSystem>();
        
        // Configura referências nos componentes
        ConfigureComponentReferences();
    }
    
    void ConfigureComponentReferences()
    {
        // Encontra automaticamente as referências se não foram configuradas
        FindMissingReferences();
        
        // Configura referências no movimento
        if (movement != null)
        {
            movement.player = player;
            movement.rb = rb;
            movement.spriteRenderer = spriteRenderer;
            movement.animator = animator;
        }
        
        // Configura referências no sistema de ataques
        if (attackSystem != null)
        {
            attackSystem.player = player;
            attackSystem.animator = animator;
        }
        
        // Configura referências no sistema de vida
        if (healthSystem != null)
        {
            healthSystem.player = player;
            healthSystem.animator = animator;
            healthSystem.rb = rb;
            
            // Encontra referências automaticamente no sistema de vida
            healthSystem.FindMissingReferencesPublic();
        }
    }
    
    void FindMissingReferences()
    {
        // Encontra o player se não foi configurado
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj;
                Debug.Log($"Boss: Player encontrado automaticamente: {player.name}");
            }
            else
            {
                Debug.LogWarning("Boss: Player não encontrado! Certifique-se de que existe um GameObject com tag 'Player' na cena.");
            }
        }
        
        // Encontra o Animator se não foi configurado
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("Boss: Animator não encontrado! Adicione um componente Animator ao GameObject do boss.");
            }
            else
            {
                Debug.Log("Boss: Animator encontrado automaticamente");
            }
        }
        
        // Encontra o Rigidbody2D se não foi configurado
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                Debug.LogWarning("Boss: Rigidbody2D não encontrado! Adicione um componente Rigidbody2D ao GameObject do boss.");
            }
            else
            {
                Debug.Log("Boss: Rigidbody2D encontrado automaticamente");
            }
        }
        
        // Encontra o SpriteRenderer se não foi configurado
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
            
            if (spriteRenderer == null)
            {
                Debug.LogWarning("Boss: SpriteRenderer não encontrado! Adicione um componente SpriteRenderer ao GameObject do boss.");
            }
            else
            {
                Debug.Log("Boss: SpriteRenderer encontrado automaticamente");
            }
        }
    }
    
    void ConnectEvents()
    {
        // Conecta eventos do sistema de ataques
        if (attackSystem != null)
        {
            attackSystem.OnAttackStart += (attack) => OnAttackStart?.Invoke(attack);
            attackSystem.OnAttackEnd += (attack) => OnAttackEnd?.Invoke(attack);
        }
        
        // Conecta eventos do sistema de vida
        if (healthSystem != null)
        {
            healthSystem.OnBossDeath += () => OnBossDeath?.Invoke();
            healthSystem.OnHealthChanged += (percentage) => OnHealthChanged?.Invoke(percentage);
            healthSystem.OnBossStunned += () => stateMachine?.ChangeState(BossState.Stunned);
        }
    }

    void ApplyPreset(BossPreset preset)
    {
        // Aplica configurações nos componentes
        if (movement != null)
        {
            movement.ApplyPreset(preset);
        }
        
        if (attackSystem != null)
        {
            attackSystem.ApplyPreset(preset);
        }
        
        if (healthSystem != null)
        {
            healthSystem.ApplyPreset(preset);
        }
        
        if (stateMachine != null)
        {
            stateMachine.stunDuration = preset.stunDuration;
        }
        
        aggressiveMode = preset.aggressiveMode;

        // Aplica sprite e animator se disponível
        if (preset.bossSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = preset.bossSprite;
        }

        if (preset.animatorController != null && animator != null)
        {
            animator.runtimeAnimatorController = preset.animatorController;
        }
    }


    IEnumerator BossFightCycle()
    {
        while (healthSystem != null && !healthSystem.IsDead)
        {
            // Verifica se o player existe
            if (player == null)
            {
                yield return new WaitForSeconds(1f);
                continue;
            }

            BossState currentState = stateMachine.CurrentState;

            // Estado Idle - Aguarda player se aproximar
            if (currentState == BossState.Idle)
            {
                if (movement.IsPlayerInChaseRange())
                {
                    stateMachine.ChangeState(BossState.Chasing);
                }
                yield return new WaitForSeconds(0.5f);
            }

            // Estado Chasing - Persegue o player
            else if (currentState == BossState.Chasing)
            {
                yield return StartCoroutine(movement.ChasePlayer());
                
                // Verifica se ainda está no estado de perseguição após a coroutine
                if (stateMachine.CurrentState == BossState.Chasing)
                {
                    if (movement.IsPlayerInAttackRange())
                    {
                        stateMachine.ChangeState(BossState.Attacking);
                    }
                    else
                    {
                        stateMachine.ChangeState(BossState.Idle);
                    }
                }
            }

            // Estado Attacking - Executa ataques
            else if (currentState == BossState.Attacking)
            {
                yield return StartCoroutine(attackSystem.ExecuteAttack());
                stateMachine.ChangeState(BossState.Resting);
            }

            // Estado Resting - Descansa após ataque
            else if (currentState == BossState.Resting)
            {
                yield return StartCoroutine(movement.RestAndRetreat());
                stateMachine.ChangeState(BossState.Idle);
            }

            yield return null;
        }
    }

    void UpdateAnimations()
    {
        // Atualiza animações baseadas no estado atual
        if (animator != null && stateMachine != null)
        {
            animator.SetBool("isIdle", stateMachine.IsInState(BossState.Idle));
            animator.SetBool("isResting", stateMachine.IsInState(BossState.Resting));
        }
    }

    // Métodos de compatibilidade para manter a interface pública

    // Métodos de compatibilidade para manter a interface pública
    public void TakeDamage(float damage)
    {
        healthSystem?.TakeDamage(damage);
    }
    
    public void TakeDamageFromPlayer(int damage, Vector2 knockback)
    {
        healthSystem?.TakeDamageFromPlayer(damage, knockback);
    }
    
    public void DealDamageToPlayer(float damage)
    {
        healthSystem?.DealDamageToPlayer(damage);
    }
    
    public float GetHealthPercentage()
    {
        return healthSystem?.HealthPercentage ?? 0f;
    }
    
    public BossState GetCurrentState()
    {
        return stateMachine?.CurrentState ?? BossState.Idle;
    }
    
    public int GetCurrentAttackIndex()
    {
        return attackSystem?.CurrentAttackIndex ?? 0;
    }
    
    public int GetTotalAttacks()
    {
        return attackSystem?.GetTotalAttacks() ?? 0;
    }
    
    public string GetNextAttackName()
    {
        return attackSystem?.GetNextAttackName() ?? "Nenhum ataque";
    }
    
    public void ForceState(BossState newState)
    {
        stateMachine?.ForceState(newState);
    }
    
    public void SetHealth(float newHealth)
    {
        healthSystem?.SetHealth(newHealth);
    }
    
    public void Heal(float healAmount)
    {
        healthSystem?.Heal(healAmount);
    }
    
    public void SetAggressiveMode(bool aggressive)
    {
        aggressiveMode = aggressive;
        if (aggressive && stateMachine != null && stateMachine.IsInState(BossState.Idle))
        {
            stateMachine.ChangeState(BossState.Chasing);
        }
    }
    
    public void ResetBoss()
    {
        healthSystem?.ResetHealth();
        stateMachine?.ResetState();
        attackSystem?.ResetAttackSystem();
        movement?.ResetMovement();
        
        // Reinicia o ciclo de combate
        StartCoroutine(BossFightCycle());
    }
    
    public void SetupHealthBar()
    {
        healthSystem?.SetupHealthBar();
    }
    
    /// <summary>
    /// Método público para forçar busca de referências (útil para debugging)
    /// </summary>
    [ContextMenu("Find Missing References")]
    public void FindMissingReferencesManual()
    {
        FindMissingReferences();
        healthSystem?.FindMissingReferencesPublic();
        Debug.Log("Boss: Busca manual de referências concluída!");
    }

    // Métodos para serem chamados pela animação do boss
    public void EnableAttackCollider()
    {
        attackSystem?.EnableAttackCollider();
    }
    
    public void DisableAttackCollider()
    {
        attackSystem?.DisableAttackCollider();
    }
    
    public void EnableAttackColliderByName(string attackName)
    {
        attackSystem?.EnableAttackColliderByName(attackName);
    }
    
    public void DisableAttackColliderByName(string attackName)
    {
        attackSystem?.DisableAttackColliderByName(attackName);
    }

    // Método para debug
    void OnDrawGizmosSelected()
    {
        if (movement != null)
        {
            // Desenha range de ataque
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, movement.attackDistance);

            // Desenha range de perseguição
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, movement.maxChaseDistance);

            // Desenha range de descanso
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, movement.restDistance);

            // Desenha direção do boss
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, movement.IsFacingRight ? Vector2.right : Vector2.left);
        }
    }
}
