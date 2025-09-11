using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightSystem : MonoBehaviour
{
    [Header("Referências")]
    public GameObject player;
    public Animator animator;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public GameObject victoryReward; // Troféu ou recompensa
    
    [Header("Colliders de Ataque")]
    public List<BossAttackCollider> attackColliders = new List<BossAttackCollider>();

    [Header("Preset do Boss (Opcional)")]
    public BossPreset bossPreset;

    [Header("Configurações de Movimento")]
    public float chaseSpeed = 3f;
    public float attackDistance = 2f;
    public float maxChaseDistance = 10f;
    public float restDistance = 5f; // Distância mínima para descansar
    public float restDuration = 3f;
    public float stunDuration = 2f;
    public float retreatDistance = 3f; // Distância que recua após ataque
    public float retreatSpeed = 2f; // Velocidade do recuo

    [Header("Sistema de Ataques")]
    public List<BossAttack> availableAttacks = new List<BossAttack>();
    public int maxComboLength = 3;
    public float timeBetweenAttacks = 1f;

    [Header("Configurações de IA")]
    public bool aggressiveMode = true;
    public float health = 100f;
    public float maxHealth = 100f;
    public float stunChance = 0.3f;

    // Estados privados
    private BossState currentState = BossState.Idle;
    private BossAttack currentAttack;
    private int currentComboCount = 0;
    private int currentAttackIndex = 0; // Índice do próximo ataque na sequência
    private float stateTimer = 0f;
    public bool isFacingRight = true;
    private Vector2 startPosition;
    private bool isDead = false;

    // Eventos
    public System.Action<BossAttack> OnAttackStart;
    public System.Action<BossAttack> OnAttackEnd;
    public System.Action OnBossDeath;
    public System.Action<float> OnHealthChanged;

    void Start()
    {
        Debug.Log("Boss: Start() chamado");
        InitializeBoss();
    }

    void Update()
    {
        if (isDead) return;

        UpdateState();
        UpdateAnimations();
    }

    void InitializeBoss()
    {
        startPosition = transform.position;
        
        Debug.Log("Boss: Inicializando boss...");
        
        // Aplica preset se configurado
        if (bossPreset != null)
        {
            ApplyPreset(bossPreset);
        }
        
        // Configura ataques padrão se nenhum foi configurado
        if (availableAttacks.Count == 0)
        {
            SetupDefaultAttacks();
        }

        // Inicializa os colliders de ataque
        InitializeAttackColliders();

        // Inicia o ciclo de combate
        StartCoroutine(BossFightCycle());
    }

    void ApplyPreset(BossPreset preset)
    {
        // Aplica configurações do preset
        health = preset.health;
        maxHealth = preset.health;
        chaseSpeed = preset.chaseSpeed;
        attackDistance = preset.attackDistance;
        maxChaseDistance = preset.maxChaseDistance;
        restDistance = preset.restDistance;
        restDuration = preset.restDuration;
        maxComboLength = preset.maxComboLength;
        aggressiveMode = preset.aggressiveMode;
        stunChance = preset.stunChance;
        stunDuration = preset.stunDuration;
        retreatDistance = preset.retreatDistance;
        retreatSpeed = preset.retreatSpeed;
        victoryReward = preset.victoryReward;

        // Aplica ataques do preset
        availableAttacks.Clear();
        foreach (var attack in preset.attacks)
        {
            availableAttacks.Add(attack);
        }

        // Aplica sprite e animator se disponível
        if (preset.bossSprite != null && spriteRenderer != null)
        {
            spriteRenderer.sprite = preset.bossSprite;
        }

        if (preset.animatorController != null && animator != null)
        {
            animator.runtimeAnimatorController = preset.animatorController;
        }

        Debug.Log($"Preset '{preset.bossName}' aplicado ao boss!");
    }

    void InitializeAttackColliders()
    {
        // Se não foram configurados manualmente, procura por colliders filhos
        if (attackColliders.Count == 0)
        {
            Debug.Log($"🔍 BOSS ATTACK: Procurando colliders filhos...");
            BossAttackCollider[] childColliders = GetComponentsInChildren<BossAttackCollider>();
            Debug.Log($"🔍 BOSS ATTACK: Encontrados {childColliders.Length} colliders filhos");
            foreach (var collider in childColliders)
            {
                Debug.Log($"🔍 BOSS ATTACK: Adicionando collider: {collider.attackName}");
                attackColliders.Add(collider);
                collider.bossFightSystem = this;
            }
        }
        
        // Desabilita todos os colliders inicialmente
        foreach (var collider in attackColliders)
        {
            if (collider != null)
            {
                Debug.Log($"🔧 BOSS ATTACK: Desabilitando collider inicial: {collider.attackName}");
                collider.DisableAttackCollider();
            }
        }
        
        Debug.Log($"Inicializados {attackColliders.Count} colliders de ataque");
        foreach (var collider in attackColliders)
        {
            if (collider != null)
            {
                Debug.Log($"🔧 BOSS ATTACK: Collider final: {collider.attackName} - Enabled: {collider.GetComponent<Collider2D>().enabled}, IsTrigger: {collider.GetComponent<Collider2D>().isTrigger}");
            }
        }
    }

    void SetupDefaultAttacks()
    {
        Debug.Log("Boss: Configurando ataques padrão...");
        
        availableAttacks.Add(new BossAttack
        {
            attackName = "ComboGiro", // Changed to match collider names
            animationTrigger = "ComboGiro",
            attackDuration = 1.5f,
            cooldown = 3f,
            damage = 15f,
            range = 2f
        });

        availableAttacks.Add(new BossAttack
        {
            attackName = "PunchRight", // Changed to match collider names
            animationTrigger = "PunchRight",
            attackDuration = 1f,
            cooldown = 2f,
            damage = 10f,
            range = 1.5f
        });

        availableAttacks.Add(new BossAttack
        {
            attackName = "PunchLeft", // Changed to match collider names
            animationTrigger = "PunchLeft",
            attackDuration = 1f,
            cooldown = 2f,
            damage = 10f,
            range = 1.5f
        });
        
        Debug.Log($"Boss: {availableAttacks.Count} ataques padrão configurados");
    }

    IEnumerator BossFightCycle()
    {
        Debug.Log("Boss: Iniciando ciclo de combate");
        
        while (!isDead)
        {
            // Verifica se o player existe
            if (player == null)
            {
                Debug.LogWarning("Player não encontrado! BossFightSystem precisa de uma referência ao player.");
                yield return new WaitForSeconds(1f);
                continue;
            }

            // Estado Idle - Aguarda player se aproximar
            if (currentState == BossState.Idle)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
                if (distanceToPlayer <= maxChaseDistance)
                {
                    Debug.Log("Boss: Player detectado, iniciando perseguição");
                    ChangeState(BossState.Chasing);
                }
                yield return new WaitForSeconds(0.5f);
            }

            // Estado Chasing - Persegue o player
            else if (currentState == BossState.Chasing)
            {
                Debug.Log("Boss: Perseguindo player");
                yield return StartCoroutine(ChasePlayer());
                
                // Verifica se ainda está no estado de perseguição após a coroutine
                if (currentState == BossState.Chasing)
                {
                    float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
                    if (distanceToPlayer <= attackDistance)
                    {
                        Debug.Log("Boss: Próximo o suficiente para atacar");
                        ChangeState(BossState.Attacking);
                    }
                    else
                    {
                        // Se não conseguiu chegar perto, volta ao idle
                        Debug.Log("Boss: Não conseguiu chegar perto do player, voltando ao idle");
                        ChangeState(BossState.Idle);
                    }
                }
            }

            // Estado Attacking - Executa ataques
            else if (currentState == BossState.Attacking)
            {
                Debug.Log("Boss: Executando ataque");
                yield return StartCoroutine(ExecuteAttack());
                ChangeState(BossState.Resting);
            }

            // Estado Resting - Descansa após ataque
            else if (currentState == BossState.Resting)
            {
                Debug.Log("Boss: Descansando");
                yield return StartCoroutine(RestAndRetreat());
                ChangeState(BossState.Idle);
            }

            yield return null;
        }
        
        Debug.Log("Boss: Ciclo de combate terminado");
    }

    IEnumerator ChasePlayer()
    {
        if (animator != null)
            animator.SetBool("isWalking", true);
        
        float chaseTime = 0f;
        float maxChaseTime = 10f; // Aumentado o tempo máximo de perseguição

        Debug.Log($"Boss: Iniciando perseguição. Distância atual: {Vector2.Distance(transform.position, player.transform.position)}");

        while (chaseTime < maxChaseTime && currentState == BossState.Chasing)
        {
            if (player == null) 
            {
                Debug.LogWarning("Boss: Player perdido durante perseguição");
                break;
            }

            Vector2 direction = (player.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            Debug.Log($"Boss: Perseguindo - Distância: {distanceToPlayer:F2}, Direção: {direction}");

            // Para de perseguir se chegou perto o suficiente
            if (distanceToPlayer <= attackDistance)
            {
                Debug.Log("Boss: Chegou perto o suficiente para atacar");
                break;
            }

            // Para de perseguir se o player fugiu muito
            if (distanceToPlayer > maxChaseDistance)
            {
                Debug.Log("Boss: Player fugiu muito, parando perseguição");
                break;
            }

            // Flip sprite baseado na direção (mais tolerante)
            if (direction.x > 0.1f && !isFacingRight)
            {
                Debug.Log("Boss: Virando para direita (perseguindo)");
                FlipSprite(true); // Vira para direita quando vai para direita
            }
            else if (direction.x < -0.1f && isFacingRight)
            {
                Debug.Log("Boss: Virando para esquerda (perseguindo)");
                FlipSprite(false); // Vira para esquerda quando vai para esquerda
            }

            // Move em direção ao player (apenas no eixo X para não voar)
            if (rb != null)
            {
                Vector2 newPosition = new Vector2(
                    rb.position.x + direction.x * chaseSpeed * Time.fixedDeltaTime,
                    rb.position.y
                );
                rb.MovePosition(newPosition);
            }

            chaseTime += Time.deltaTime;
            yield return null;
        }

        if (animator != null)
            animator.SetBool("isWalking", false);
        
        Debug.Log($"Boss: Perseguição terminada após {chaseTime:F2} segundos");
    }

    IEnumerator RetreatAfterAttack()
    {
        Debug.Log("Boss: Recuando após ataque");
        
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }

        float retreatTime = 0f;
        float maxRetreatTime = 2f; // Tempo máximo de recuo
        Vector2 retreatDirection = -transform.right; // Recua na direção oposta ao que está olhando

        Debug.Log($"🔄 RETREAT AFTER ATTACK: Direção de recuo: {retreatDirection}, FacingRight: {isFacingRight}");

        while (retreatTime < maxRetreatTime && currentState == BossState.Attacking)
        {
            if (rb != null)
            {
                // Move para trás
                Vector2 newPosition = new Vector2(
                    rb.position.x + retreatDirection.x * retreatSpeed * Time.fixedDeltaTime,
                    rb.position.y
                );
                rb.MovePosition(newPosition);
                Debug.Log($"🔄 RETREAT AFTER ATTACK: Movendo para posição: {newPosition}");
            }

            retreatTime += Time.deltaTime;
            yield return null;
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }

        Debug.Log($"Boss: Recuo terminado após {retreatTime:F2} segundos");
    }

    IEnumerator RestAndRetreat()
    {
        Debug.Log("Boss: Iniciando descanso e afastamento");
        
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
        }

        float retreatTime = 0f;
        float maxRetreatTime = 3f; // Tempo máximo de afastamento

        // Afasta até atingir a distância de descanso ou tempo máximo
        while (retreatTime < maxRetreatTime && currentState == BossState.Resting)
        {
            if (player != null)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
                
                // Se já está longe o suficiente, para de se afastar
                if (distanceToPlayer >= restDistance)
                {
                    Debug.Log($"Boss: Distância de descanso atingida: {distanceToPlayer:F2}");
                    break;
                }

                // Calcula direção de afastamento baseada na posição do player
                Vector2 directionAwayFromPlayer = (transform.position - player.transform.position).normalized;
                
                Debug.Log($"🔄 RETREAT: Distância: {distanceToPlayer:F2}, Direção afastamento: {directionAwayFromPlayer}, FacingRight: {isFacingRight}");
                Debug.Log($"🔄 RETREAT: Posição Boss: {transform.position}, Posição Player: {player.transform.position}");
                
                // Se está muito perto do player, força afastamento
                if (distanceToPlayer < restDistance)
                {
                    // Vira o boss na direção que está recuando
                    if (directionAwayFromPlayer.x > 0.1f)
                    {
                        if (!isFacingRight)
                        {
                            Debug.Log($"🔄 RETREAT: Virando para DIREITA (recuando para direita)");
                            FlipSprite(true); // Vira para direita quando recua para direita
                        }
                        else
                        {
                            Debug.Log($"🔄 RETREAT: Já está virado para direita (recuando para direita)");
                        }
                    }
                    else if (directionAwayFromPlayer.x < -0.1f)
                    {
                        if (isFacingRight)
                        {
                            Debug.Log($"🔄 RETREAT: Virando para ESQUERDA (recuando para esquerda)");
                            FlipSprite(false); // Vira para esquerda quando recua para esquerda
                        }
                        else
                        {
                            Debug.Log($"🔄 RETREAT: Já está virado para esquerda (recuando para esquerda)");
                        }
                    }
                    else
                    {
                        Debug.Log($"🔄 RETREAT: Não precisa virar - Direção: {directionAwayFromPlayer.x:F2}, FacingRight: {isFacingRight}");
                    }
                    
                    // Força o flip a cada frame durante o recuo para garantir que não seja sobrescrito pela animação
                    if (directionAwayFromPlayer.x > 0.1f && !isFacingRight)
                    {
                        FlipSprite(true);
                    }
                    else if (directionAwayFromPlayer.x < -0.1f && isFacingRight)
                    {
                        FlipSprite(false);
                    }
                    
                    if (rb != null)
                    {
                        // Move para longe do player
                        Vector2 newPosition = new Vector2(
                            rb.position.x + directionAwayFromPlayer.x * retreatSpeed * Time.fixedDeltaTime,
                            rb.position.y
                        );
                        rb.MovePosition(newPosition);
                        Debug.Log($"🔄 RETREAT: Movendo para posição: {newPosition}");
                    }
                }
            }

            retreatTime += Time.deltaTime;
            yield return null;
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }

        Debug.Log($"Boss: Afastamento terminado após {retreatTime:F2} segundos");
        
        // Aguarda o tempo de descanso
        Debug.Log($"Boss: Aguardando {restDuration} segundos de descanso");
        yield return new WaitForSeconds(restDuration);
        
        Debug.Log("Boss: Descanso terminado");
    }

    IEnumerator ExecuteAttack()
    {
        // Escolhe o próximo ataque na sequência
        currentAttack = ChooseNextAttack();
        if (currentAttack == null) 
        {
            Debug.LogWarning("Boss: Nenhum ataque disponível!");
            yield break;
        }

        Debug.Log($"Boss: Executando ataque: {currentAttack.attackName} (Sequência: {currentAttackIndex}/{availableAttacks.Count})");

        // Para de se mover durante o ataque
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }

        // Configura o collider de ataque apropriado
        BossAttackCollider attackCollider = GetAttackCollider(currentAttack.attackName);
        if (attackCollider != null)
        {
            Debug.Log($"🔧 BOSS ATTACK: Configurando collider para ataque: {currentAttack.attackName}");
            attackCollider.SetupAttack(
                Mathf.RoundToInt(currentAttack.damage), 
                Vector2.right * 5f, // Knockback padrão
                currentAttack.attackName
            );
        }
        else
        {
            Debug.LogError($"❌ BOSS ATTACK: Collider não encontrado para ataque: {currentAttack.attackName}");
        }

        // Ativa a animação do ataque
        if (animator != null)
        {
            Debug.Log($"🎬 BOSS ATTACK: Ativando animação: {currentAttack.animationTrigger}");
            animator.SetTrigger(currentAttack.animationTrigger);
        }
        OnAttackStart?.Invoke(currentAttack);

        // Aguarda a duração do ataque
        Debug.Log($"Boss: Aguardando {currentAttack.attackDuration} segundos para ataque: {currentAttack.attackName}");
        yield return new WaitForSeconds(currentAttack.attackDuration);

        // Desativa o trigger após o ataque
        if (animator != null)
        {
            Debug.Log($"🎬 BOSS ATTACK: Desativando animação: {currentAttack.animationTrigger}");
            animator.ResetTrigger(currentAttack.animationTrigger);
        }

        // Desativa o collider de ataque
        if (attackCollider != null)
        {
            Debug.Log($"🔧 BOSS ATTACK: Desativando collider para ataque: {currentAttack.attackName}");
            attackCollider.DisableAttackCollider();
        }
        else
        {
            Debug.LogError($"❌ BOSS ATTACK: Collider não encontrado para desativar: {currentAttack.attackName}");
        }

        OnAttackEnd?.Invoke(currentAttack);
        currentComboCount++;

        // Recua após o ataque
        Debug.Log($"🔄 BOSS ATTACK: Iniciando recuo após ataque");
        yield return StartCoroutine(RetreatAfterAttack());

        // Se completou o combo máximo, descansa mais tempo
        if (currentComboCount >= maxComboLength)
        {
            Debug.Log($"Boss: Combo máximo atingido ({currentComboCount}/{maxComboLength}), resetando contador");
            currentComboCount = 0;
        }
    }

    BossAttack ChooseNextAttack()
    {
        if (availableAttacks.Count == 0) return null;

        // Escolhe o próximo ataque na sequência
        BossAttack nextAttack = availableAttacks[currentAttackIndex];
        
        Debug.Log($"Boss: Escolhendo ataque {currentAttackIndex + 1}/{availableAttacks.Count}: {nextAttack.attackName}");
        
        // Avança para o próximo ataque na lista
        currentAttackIndex = (currentAttackIndex + 1) % availableAttacks.Count;
        
        return nextAttack;
    }

    BossAttackCollider GetAttackCollider(string attackName)
    {
        Debug.Log($"🔍 BOSS ATTACK: Procurando collider para ataque: {attackName}");
        Debug.Log($"🔍 BOSS ATTACK: Colliders disponíveis: {attackColliders.Count}");
        
        // Procura por um collider que corresponda ao nome do ataque
        foreach (var collider in attackColliders)
        {
            Debug.Log($"🔍 BOSS ATTACK: Verificando collider: {collider.attackName}");
            if (collider.attackName == attackName)
            {
                Debug.Log($"✅ BOSS ATTACK: Collider encontrado: {collider.attackName}");
                return collider;
            }
        }
        
        // Se não encontrou, retorna o primeiro collider disponível
        if (attackColliders.Count > 0)
        {
            Debug.Log($"⚠️ BOSS ATTACK: Collider específico não encontrado, usando primeiro disponível: {attackColliders[0].attackName}");
            return attackColliders[0];
        }
        
        Debug.LogError($"❌ BOSS ATTACK: Nenhum collider disponível!");
        return null;
    }

    void ChangeState(BossState newState)
    {
        if (currentState == newState) return; // Evita mudanças desnecessárias

        Debug.Log($"Boss: Mudando estado de {currentState} para {newState}");
        
        currentState = newState;
        stateTimer = 0f;

        // Reseta animações
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isResting", false);
            animator.SetBool("isIdle", false);

            // Reseta todos os triggers de ataque para evitar conflitos
            foreach (var attack in availableAttacks)
            {
                animator.ResetTrigger(attack.animationTrigger);
            }

            // Configura animações específicas do estado
            switch (newState)
            {
                case BossState.Idle:
                    animator.SetBool("isIdle", true);
                    Debug.Log("Boss: Configurando animação Idle");
                    break;
                case BossState.Resting:
                    animator.SetBool("isResting", true);
                    Debug.Log("Boss: Configurando animação Resting");
                    break;
                case BossState.Stunned:
                    animator.SetTrigger("Stunned");
                    Debug.Log("Boss: Configurando animação Stunned");
                    break;
            }
        }
    }

    void UpdateState()
    {
        stateTimer += Time.deltaTime;

        // Transições automáticas de estado
        switch (currentState)
        {
            case BossState.Stunned:
                if (stateTimer >= stunDuration)
                {
                    Debug.Log($"Boss: Saindo do estado Stunned após {stateTimer:F2} segundos");
                    ChangeState(BossState.Idle);
                }
                break;
        }
    }

    void UpdateAnimations()
    {
        // Atualiza animações baseadas no estado atual
        if (animator != null)
        {
            animator.SetBool("isIdle", currentState == BossState.Idle);
            animator.SetBool("isResting", currentState == BossState.Resting);
        }
    }

    void FlipSprite(bool faceRight)
    {
        isFacingRight = faceRight;
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = faceRight; // flipX=true quando faceRight=true (olhando para direita)
            Debug.Log($"Boss: Sprite virado - FacingRight: {faceRight}, flipX: {spriteRenderer.flipX}");
            
            // Força a atualização do sprite para garantir que a mudança seja aplicada
            spriteRenderer.enabled = false;
            spriteRenderer.enabled = true;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        OnHealthChanged?.Invoke(health / maxHealth);

        Debug.Log($"Boss recebeu {damage} de dano! Vida restante: {health}/{maxHealth}");

        // Ativa animação de hit
        if (animator != null)
        {
            animator.SetTrigger("Stunned");
            Debug.Log("Boss: Animação de hit ativada!");
        }

        // Chance de ficar atordoado
        if (Random.Range(0f, 1f) < stunChance)
        {
            Debug.Log($"Boss: Chance de stun ativada ({stunChance * 100}%)");
            ChangeState(BossState.Stunned);
        }

        if (health <= 0)
        {
            Debug.Log("Boss: Vida zerada, morrendo...");
            Die();
        }
    }

    // Método para ser chamado pelos ataques do player
    public void TakeDamageFromPlayer(int damage, Vector2 knockback)
    {
        if (isDead) return;

        Debug.Log($"Boss: Recebendo dano do player: {damage}, Knockback: {knockback}");
        TakeDamage(damage);
        
        // Aplica knockback ao boss se necessário
        if (rb != null && knockback.magnitude > 0)
        {
            Debug.Log($"Boss: Aplicando knockback: {knockback}");
            rb.AddForce(knockback, ForceMode2D.Impulse);
        }
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Boss: Morrendo...");
        animator.SetTrigger("Death");
        OnBossDeath?.Invoke();

        // Spawna recompensa
        if (victoryReward != null)
        {
            Debug.Log("Boss: Spawnando recompensa...");
            Instantiate(victoryReward, transform.position, Quaternion.identity);
        }

        // Desabilita o sistema de combate
        enabled = false;
        Debug.Log("Boss: Sistema de combate desabilitado");
    }

    void DealDamageToPlayer(float damage)
    {
        // Implementa interface de dano do player usando o sistema Damageable
        if (player != null)
        {
            Damageable playerDamageable = player.GetComponent<Damageable>();
            if (playerDamageable != null)
            {
                // Calcula knockback baseado na direção do boss
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                Vector2 knockback = knockbackDirection * 5f; // Força do knockback
                
                Debug.Log($"Boss: Tentando causar {damage} de dano ao player, knockback: {knockback}");
                bool hitSuccess = playerDamageable.Hit(Mathf.RoundToInt(damage), knockback);
                if (hitSuccess)
                {
                    Debug.Log($"Boss causou {damage} de dano ao player!");
                }
                else
                {
                    Debug.Log("Boss tentou atacar mas o player está invulnerável!");
                }
            }
            else
            {
                Debug.LogWarning("Player não possui componente Damageable!");
            }
        }
    }

    // Métodos públicos para configuração externa
    public void AddAttack(BossAttack attack)
    {
        availableAttacks.Add(attack);
    }

    public void RemoveAttack(string attackName)
    {
        availableAttacks.RemoveAll(a => a.attackName == attackName);
    }

    public float GetHealthPercentage()
    {
        return health / maxHealth;
    }

    public BossState GetCurrentState()
    {
        return currentState;
    }

    // Métodos para informações da sequência de ataques
    public int GetCurrentAttackIndex()
    {
        return currentAttackIndex;
    }

    public int GetTotalAttacks()
    {
        return availableAttacks.Count;
    }

    public string GetNextAttackName()
    {
        if (availableAttacks.Count == 0) return "Nenhum ataque";
        return availableAttacks[currentAttackIndex].attackName;
    }

    // Métodos para controle externo
    public void ForceState(BossState newState)
    {
        ChangeState(newState);
    }

    public void SetHealth(float newHealth)
    {
        health = Mathf.Clamp(newHealth, 0f, maxHealth);
        OnHealthChanged?.Invoke(health / maxHealth);
        
        if (health <= 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        SetHealth(health + healAmount);
    }

    public void SetAggressiveMode(bool aggressive)
    {
        aggressiveMode = aggressive;
        if (aggressive && currentState == BossState.Idle)
        {
            ChangeState(BossState.Chasing);
        }
    }

    public void ResetBoss()
    {
        health = maxHealth;
        currentState = BossState.Idle;
        currentComboCount = 0;
        currentAttackIndex = 0; // Reseta a sequência de ataques
        stateTimer = 0f;
        isDead = false;
        enabled = true;
        
        Debug.Log("Boss: Resetando boss...");
        
        // Retorna à posição inicial
        transform.position = startPosition;
        
        // Reinicia o ciclo de combate
        StartCoroutine(BossFightCycle());
    }

    // Métodos para serem chamados pela animação do boss
    public void EnableAttackCollider()
    {
        Debug.Log($"🎬 ANIMATION EVENT: EnableAttackCollider chamado!");
        if (currentAttack != null)
        {
            Debug.Log($"🎬 ANIMATION EVENT: Ataque atual: {currentAttack.attackName}");
            BossAttackCollider attackCollider = GetAttackCollider(currentAttack.attackName);
            if (attackCollider != null)
            {
                Debug.Log($"🎬 ANIMATION EVENT: Collider encontrado, ativando...");
                attackCollider.EnableAttackCollider();
            }
            else
            {
                Debug.LogError($"❌ ANIMATION EVENT: Collider não encontrado para ataque: {currentAttack.attackName}");
            }
        }
        else
        {
            Debug.LogError($"❌ ANIMATION EVENT: Nenhum ataque atual!");
        }
    }

    public void DisableAttackCollider()
    {
        Debug.Log($"🎬 ANIMATION EVENT: DisableAttackCollider chamado!");
        if (currentAttack != null)
        {
            Debug.Log($"🎬 ANIMATION EVENT: Ataque atual: {currentAttack.attackName}");
            BossAttackCollider attackCollider = GetAttackCollider(currentAttack.attackName);
            if (attackCollider != null)
            {
                Debug.Log($"🎬 ANIMATION EVENT: Collider encontrado, desativando...");
                attackCollider.DisableAttackCollider();
            }
            else
            {
                Debug.LogError($"❌ ANIMATION EVENT: Collider não encontrado para ataque: {currentAttack.attackName}");
            }
        }
        else
        {
            Debug.LogError($"❌ ANIMATION EVENT: Nenhum ataque atual!");
        }
    }

    // Método para ativar collider específico por nome (útil para animações)
    public void EnableAttackColliderByName(string attackName)
    {
        Debug.Log($"🎬 ANIMATION EVENT: EnableAttackColliderByName chamado para: {attackName}");
        BossAttackCollider attackCollider = GetAttackCollider(attackName);
        if (attackCollider != null)
        {
            Debug.Log($"🎬 ANIMATION EVENT: Collider encontrado, ativando...");
            attackCollider.EnableAttackCollider();
        }
        else
        {
            Debug.LogError($"❌ ANIMATION EVENT: Collider não encontrado para ataque: {attackName}");
        }
    }

    public void DisableAttackColliderByName(string attackName)
    {
        Debug.Log($"🎬 ANIMATION EVENT: DisableAttackColliderByName chamado para: {attackName}");
        BossAttackCollider attackCollider = GetAttackCollider(attackName);
        if (attackCollider != null)
        {
            Debug.Log($"🎬 ANIMATION EVENT: Collider encontrado, desativando...");
            attackCollider.DisableAttackCollider();
        }
        else
        {
            Debug.LogError($"❌ ANIMATION EVENT: Collider não encontrado para ataque: {attackName}");
        }
    }

    // Método para debug
    void OnDrawGizmosSelected()
    {
        // Desenha range de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        // Desenha range de perseguição
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxChaseDistance);

        // Desenha range de descanso
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, restDistance);

        // Desenha direção do boss
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, isFacingRight ? Vector2.right : Vector2.left);
        
        // Desenha colliders de ataque
        Gizmos.color = Color.magenta;
        foreach (var collider in attackColliders)
        {
            if (collider != null)
            {
                Gizmos.DrawWireCube(collider.transform.position, Vector3.one * 0.5f);
            }
        }
    }
}
