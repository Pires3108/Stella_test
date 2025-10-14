using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gerencia o sistema de vida, dano e UI do boss
/// </summary>
public class BossHealthSystem : MonoBehaviour
{
    [Header("Configurações de Vida")]
    public float health = 100f;
    public float maxHealth = 100f;
    public float stunChance = 0.3f;
    
    [Header("UI e Efeitos")]
    public Slider healthBar;
    public Animator healthBarAnimator;
    public GameObject victoryReward;
    
    [Header("Referências")]
    public GameObject player;
    public Animator animator;
    public Rigidbody2D rb;
    
    // Estado interno
    private bool isDead = false;
    
    // Eventos
    public event System.Action OnBossDeath;
    public event System.Action<float> OnHealthChanged;
    public event System.Action<float> OnDamageTaken;
    public event System.Action OnBossStunned;
    
    // Propriedades públicas
    public float Health => health;
    public float MaxHealth => maxHealth;
    public float HealthPercentage => health / maxHealth;
    public bool IsDead => isDead;
    
    void Start()
    {
        FindMissingReferences();
        InitializeHealthBar();
    }
    
    void FindMissingReferences()
    {
        // Encontra a barra de vida se não foi configurada
        if (healthBar == null)
        {
            // Procura por uma barra de vida na cena
            Slider[] allSliders = FindObjectsOfType<Slider>();
            foreach (var slider in allSliders)
            {
                if (slider.name.ToLower().Contains("boss") || 
                    slider.name.ToLower().Contains("health") ||
                    slider.gameObject.name.ToLower().Contains("boss"))
                {
                    healthBar = slider;
                    Debug.Log($"Boss: Barra de vida encontrada automaticamente: {healthBar.name}");
                    break;
                }
            }
            
            if (healthBar == null)
            {
                Debug.LogWarning("Boss: Barra de vida não encontrada automaticamente. Configure manualmente no Inspector.");
            }
        }
        
        // Encontra o victory reward se não foi configurado
        if (victoryReward == null)
        {
            // Procura por objetos que podem ser recompensas
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            foreach (var obj in allObjects)
            {
                if (obj.name.ToLower().Contains("reward") || 
                    obj.name.ToLower().Contains("trophy") ||
                    obj.name.ToLower().Contains("victory") ||
                    obj.name.ToLower().Contains("prize"))
                {
                    victoryReward = obj;
                    Debug.Log($"Boss: Victory reward encontrado automaticamente: {victoryReward.name}");
                    break;
                }
            }
            
            if (victoryReward == null)
            {
                Debug.LogWarning("Boss: Victory reward não encontrado automaticamente. Configure manualmente no Inspector.");
            }
        }
        
        // Encontra o animator da barra de vida se não foi configurado
        if (healthBarAnimator == null && healthBar != null)
        {
            healthBarAnimator = healthBar.GetComponent<Animator>();
            if (healthBarAnimator == null)
            {
                healthBarAnimator = healthBar.GetComponentInChildren<Animator>();
            }
            
            if (healthBarAnimator != null)
            {
                Debug.Log("Boss: Health bar animator encontrado automaticamente");
            }
        }
    }
    
    /// <summary>
    /// Inicializa a barra de vida
    /// </summary>
    private void InitializeHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
            healthBar.gameObject.SetActive(false); // Começa desativada
        }
        
        // Desativa o troféu inicialmente
        if (victoryReward != null)
        {
            victoryReward.SetActive(false);
        }
    }
    
    /// <summary>
    /// Causa dano ao boss
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;
        
        health -= damage;
        OnHealthChanged?.Invoke(HealthPercentage);
        OnDamageTaken?.Invoke(damage);
        
        // Debug.Log($"Boss recebeu {damage} de dano! Vida restante: {health}/{maxHealth}");
        
        // Interrompe qualquer movimento em andamento
        InterruptSystems();
        
        // Ativa animação de hit
        if (animator != null)
        {
            animator.SetTrigger("Stunned");
        }
        
        // Chance de ficar atordoado
        if (Random.Range(0f, 1f) < stunChance)
        {
            OnBossStunned?.Invoke();
        }
        
        if (health <= 0)
        {
            TesteDie();
        }
    }
    
    /// <summary>
    /// Causa dano ao boss com knockback (chamado pelo player)
    /// </summary>
    public void TakeDamageFromPlayer(int damage, Vector2 knockback)
    {
        if (isDead) return;
        
        // Debug.Log($"Boss: Recebendo dano do player: {damage}, Knockback: {knockback}");
        TakeDamage(damage);
        
        // Aplica knockback ao boss se necessário
        if (rb != null && knockback.magnitude > 0)
        {
            rb.AddForce(knockback, ForceMode2D.Impulse);
        }
    }
    
    /// <summary>
    /// Causa dano ao player
    /// </summary>
    public void DealDamageToPlayer(float damage)
    {
        Debug.Log($"🎯 BOSS HEALTH SYSTEM: DealDamageToPlayer chamado! Dano: {damage}");
        
        if (player != null)
        {
            Debug.Log($"✅ BOSS HEALTH SYSTEM: Player encontrado: {player.name}");
            
            Damageable playerDamageable = player.GetComponent<Damageable>();
            if (playerDamageable != null)
            {
                Debug.Log($"✅ BOSS HEALTH SYSTEM: Damageable component encontrado no player");
                
                // Calcula knockback baseado na direção do boss
                Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
                Vector2 knockback = knockbackDirection * 5f;
                
                Debug.Log($"💥 BOSS HEALTH SYSTEM: Chamando playerDamageable.Hit({Mathf.RoundToInt(damage)}, {knockback})");
                bool hitSuccess = playerDamageable.Hit(Mathf.RoundToInt(damage), knockback);
                
                if (hitSuccess)
                {
                    Debug.Log($"✅ BOSS DAMAGE: SUCESSO! Boss causou {damage} de dano ao player!");
                }
                else
                {
                    Debug.Log($"❌ BOSS DAMAGE: FALHA! playerDamageable.Hit retornou false!");
                }
            }
            else
            {
                Debug.LogError($"❌ BOSS HEALTH SYSTEM: Damageable component NÃO encontrado no player!");
            }
        }
        else
        {
            Debug.LogError($"❌ BOSS HEALTH SYSTEM: Player é null!");
        }
    }

    /// <summary>
    /// Mata o boss
    /// </summary>
    private void Die()
    {
        isDead = true;
        // Debug.Log("Boss: Morrendo...");

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        OnBossDeath?.Invoke();

        // Desativa a barra de vida quando o boss morre
        if (healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }

        // Move o troféu para a posição do boss e ativa
        if (victoryReward != null)
        {
            victoryReward.transform.position = transform.position;
            victoryReward.SetActive(true);
        }

        // Desabilita o sistema de combate
        enabled = false;

        // Destrói o GameObject do boss após um pequeno delay
        StartCoroutine(DestroyBossAfterDeath());
    }


#region TesteDie
    public void TesteDie()
    {
        animator.SetTrigger("Death");
        victoryReward.transform.position = transform.position;
        StartCoroutine(Teste());
    }

    public System.Collections.IEnumerator Teste()
    {
        yield return new WaitForSeconds(2f);
        victoryReward.SetActive(true);
        this.gameObject.SetActive(false);
    }
#endregion

    /// <summary>
    /// Destrói o boss após a animação de morte
    /// </summary>
    private System.Collections.IEnumerator DestroyBossAfterDeath()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Interrompe todos os sistemas quando o boss toma dano
    /// </summary>
    private void InterruptSystems()
    {
        // Interrompe movimento
        BossMovement movement = GetComponent<BossMovement>();
        if (movement != null)
        {
            movement.InterruptMovement();
        }
        
        // Interrompe ataques
        BossAttackSystem attackSystem = GetComponent<BossAttackSystem>();
        if (attackSystem != null)
        {
            attackSystem.InterruptAttack();
        }
    }
    
    /// <summary>
    /// Define a vida do boss
    /// </summary>
    public void SetHealth(float newHealth)
    {
        health = Mathf.Clamp(newHealth, 0f, maxHealth);
        OnHealthChanged?.Invoke(HealthPercentage);
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Cura o boss
    /// </summary>
    public void Heal(float healAmount)
    {
        SetHealth(health + healAmount);
    }
    
    /// <summary>
    /// Reseta a vida do boss
    /// </summary>
    public void ResetHealth()
    {
        health = maxHealth;
        isDead = false;
        enabled = true;
        OnHealthChanged?.Invoke(HealthPercentage);
    }
    
    /// <summary>
    /// Configura a barra de vida (chamado pelo ControllerFight)
    /// </summary>
    public void SetupHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = health;
        }
    }
    
    /// <summary>
    /// Atualiza a barra de vida
    /// </summary>
    public void UpdateHealthBar()
    {
        if (healthBar != null && healthBar.gameObject.activeInHierarchy)
        {
            healthBar.value = health;
            
            // Ativa animação da barra de vida se disponível
            if (healthBarAnimator != null)
            {
                float healthPercentage = HealthPercentage;
                // Aqui você pode adicionar lógica para animações da barra
            }
        }
    }
    
    /// <summary>
    /// Aplica configurações de um preset
    /// </summary>
    public void ApplyPreset(BossPreset preset)
    {
        health = preset.health;
        maxHealth = preset.health;
        stunChance = preset.stunChance;
        victoryReward = preset.victoryReward;
    }
    
    /// <summary>
    /// Método público para encontrar referências automaticamente (chamado pelo BossFightSystem)
    /// </summary>
    public void FindMissingReferencesPublic()
    {
        FindMissingReferences();
    }
}
