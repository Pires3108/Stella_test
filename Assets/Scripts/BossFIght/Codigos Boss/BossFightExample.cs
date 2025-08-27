using UnityEngine;
using UnityEngine.UI;

public class BossFightExample : MonoBehaviour
{
    [Header("Referências")]
    public BossFightSystem bossFightSystem;
    public Slider healthBar;
    public Text stateText;
    public Button damageButton;
    public Button healButton;
    public Button resetButton;

    void Start()
    {
        // Configura a UI
        SetupUI();
        
        // Conecta eventos do boss
        if (bossFightSystem != null)
        {
            bossFightSystem.OnHealthChanged += UpdateHealthBar;
            bossFightSystem.OnBossDeath += OnBossDeath;
            bossFightSystem.OnAttackStart += OnBossAttackStart;
            bossFightSystem.OnAttackEnd += OnBossAttackEnd;
        }
    }

    void SetupUI()
    {
        if (healthBar != null)
        {
            healthBar.value = 1f;
        }

        if (damageButton != null)
        {
            damageButton.onClick.AddListener(() => DealDamageToBoss(10f));
        }

        if (healButton != null)
        {
            healButton.onClick.AddListener(() => HealBoss(20f));
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetBoss);
        }
    }

    void Update()
    {
        // Atualiza texto do estado
        if (stateText != null && bossFightSystem != null)
        {
            stateText.text = $"Estado: {bossFightSystem.GetCurrentState()}";
        }
    }

    void UpdateHealthBar(float healthPercentage)
    {
        if (healthBar != null)
        {
            healthBar.value = healthPercentage;
        }
    }

    void DealDamageToBoss(float damage)
    {
        if (bossFightSystem != null)
        {
            bossFightSystem.TakeDamage(damage);
            Debug.Log($"Causou {damage} de dano ao boss!");
        }
    }

    void HealBoss(float healAmount)
    {
        if (bossFightSystem != null)
        {
            bossFightSystem.Heal(healAmount);
            Debug.Log($"Curou {healAmount} de vida do boss!");
        }
    }

    void ResetBoss()
    {
        if (bossFightSystem != null)
        {
            bossFightSystem.ResetBoss();
            Debug.Log("Boss resetado!");
        }
    }

    void OnBossDeath()
    {
        Debug.Log("Boss foi derrotado! Parabéns!");
        
        // Desabilita botões de dano
        if (damageButton != null)
        {
            damageButton.interactable = false;
        }
    }

    void OnBossAttackStart(BossAttack attack)
    {
        Debug.Log($"Boss iniciou ataque: {attack.attackName}");
    }

    void OnBossAttackEnd(BossAttack attack)
    {
        Debug.Log($"Boss terminou ataque: {attack.attackName}");
    }

    void OnDestroy()
    {
        // Remove eventos para evitar memory leaks
        if (bossFightSystem != null)
        {
            bossFightSystem.OnHealthChanged -= UpdateHealthBar;
            bossFightSystem.OnBossDeath -= OnBossDeath;
            bossFightSystem.OnAttackStart -= OnBossAttackStart;
            bossFightSystem.OnAttackEnd -= OnBossAttackEnd;
        }
    }
}
