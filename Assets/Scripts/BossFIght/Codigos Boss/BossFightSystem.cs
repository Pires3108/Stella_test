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
    private bool isFacingRight = true;
    private Vector2 startPosition;
    private bool isDead = false;

    // Eventos
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
        if (isDead) return;

        UpdateState();
        UpdateAnimations();
    }

    void InitializeBoss()
    {
        startPosition = transform.position;
        
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

    void SetupDefaultAttacks()
    {
        availableAttacks.Add(new BossAttack
        {
            attackName = "ComboGiro",
            animationTrigger = "ComboGiro",
            attackDuration = 1.5f,
            cooldown = 3f,
            damage = 15f,
            range = 2f
        });

        availableAttacks.Add(new BossAttack
        {
            attackName = "PunchRight",
            animationTrigger = "PunchRight",
            attackDuration = 1f,
            cooldown = 2f,
            damage = 10f,
            range = 1.5f
        });

        availableAttacks.Add(new BossAttack
        {
            attackName = "PunchLeft",
            animationTrigger = "PunchLeft",
            attackDuration = 1f,
            cooldown = 2f,
            damage = 10f,
            range = 1.5f
        });
    }

    IEnumerator BossFightCycle()
    {
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

            // Flip sprite baseado na direção
            if (direction.x > 0 && !isFacingRight)
            {
                FlipSprite(true);
            }
            else if (direction.x < 0 && isFacingRight)
            {
                FlipSprite(false);
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
        Vector2 retreatDirection = -transform.right; // Afasta na direção oposta ao que está olhando

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
            }

            if (rb != null)
            {
                // Move para trás
                Vector2 newPosition = new Vector2(
                    rb.position.x + retreatDirection.x * retreatSpeed * Time.fixedDeltaTime,
                    rb.position.y
                );
                rb.MovePosition(newPosition);
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

        // Ativa a animação do ataque
        if (animator != null)
        {
            animator.SetTrigger(currentAttack.animationTrigger);
        }
        OnAttackStart?.Invoke(currentAttack);

        // Aguarda a duração do ataque
        yield return new WaitForSeconds(currentAttack.attackDuration);

        // Desativa o trigger após o ataque
        if (animator != null)
        {
            animator.ResetTrigger(currentAttack.animationTrigger);
        }

        // Verifica se acertou o player
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            if (distanceToPlayer <= currentAttack.range)
            {
                Debug.Log($"Boss: Ataque acertou! Distância: {distanceToPlayer:F2}, Range: {currentAttack.range}");
                DealDamageToPlayer(currentAttack.damage);
            }
            else
            {
                Debug.Log($"Boss: Ataque errou! Distância: {distanceToPlayer:F2}, Range: {currentAttack.range}");
            }
        }

        OnAttackEnd?.Invoke(currentAttack);
        currentComboCount++;

        // Recua após o ataque
        yield return StartCoroutine(RetreatAfterAttack());

        // Se completou o combo máximo, descansa mais tempo
        if (currentComboCount >= maxComboLength)
        {
            currentComboCount = 0;
        }
    }

    BossAttack ChooseNextAttack()
    {
        if (availableAttacks.Count == 0) return null;

        // Escolhe o próximo ataque na sequência
        BossAttack nextAttack = availableAttacks[currentAttackIndex];
        
        // Avança para o próximo ataque na lista
        currentAttackIndex = (currentAttackIndex + 1) % availableAttacks.Count;
        
        return nextAttack;
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
                    break;
                case BossState.Resting:
                    animator.SetBool("isResting", true);
                    break;
                case BossState.Stunned:
                    animator.SetTrigger("Stunned");
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
            spriteRenderer.flipX = faceRight;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        OnHealthChanged?.Invoke(health / maxHealth);

        // Chance de ficar atordoado
        if (Random.Range(0f, 1f) < stunChance)
        {
            ChangeState(BossState.Stunned);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Death");
        OnBossDeath?.Invoke();

        // Spawna recompensa
        if (victoryReward != null)
        {
            Instantiate(victoryReward, transform.position, Quaternion.identity);
        }

        // Desabilita o sistema de combate
        enabled = false;
    }

    void DealDamageToPlayer(float damage)
    {
        // Implementar interface de dano do player
        // Exemplo: player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        Debug.Log($"Boss causou {damage} de dano ao player!");
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
        
        // Retorna à posição inicial
        transform.position = startPosition;
        
        // Reinicia o ciclo de combate
        StartCoroutine(BossFightCycle());
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
    }
}
