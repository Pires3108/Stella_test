using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gerencia o sistema de ataques do boss
/// </summary>
public class BossAttackSystem : MonoBehaviour
{
    [Header("Sistema de Ataques")]
    public List<BossAttack> availableAttacks = new List<BossAttack>();
    public int maxComboLength = 3;
    public float timeBetweenAttacks = 1f;
    
    [Header("Colliders de Ataque")]
    public List<BossAttackCollider> attackColliders = new List<BossAttackCollider>();
    
    [Header("Referências")]
    public GameObject player;
    public Animator animator;
    
    // Estado interno
    private BossAttack currentAttack;
    private int currentComboCount = 0;
    private int currentAttackIndex = 0;
    private bool isInterrupted = false;
    
    // Eventos
    public event System.Action<BossAttack> OnAttackStart;
    public event System.Action<BossAttack> OnAttackEnd;
    public event System.Action OnComboCompleted;
    
    // Propriedades públicas
    public BossAttack CurrentAttack => currentAttack;
    public int CurrentComboCount => currentComboCount;
    public int CurrentAttackIndex => currentAttackIndex;
    public bool IsInterrupted => isInterrupted;
    
    void Start()
    {
        InitializeAttackColliders();
    }
    
    /// <summary>
    /// Inicializa os colliders de ataque
    /// </summary>
    private void InitializeAttackColliders()
    {
        // Se não foram configurados manualmente, procura por colliders filhos
        if (attackColliders.Count == 0)
        {
            BossAttackCollider[] childColliders = GetComponentsInChildren<BossAttackCollider>();
            foreach (var collider in childColliders)
            {
                attackColliders.Add(collider);
                collider.bossFightSystem = GetComponent<BossFightSystem>();
                collider.bossHealthSystem = GetComponent<BossHealthSystem>();
            }
        }
        
        // Desabilita todos os colliders inicialmente
        foreach (var collider in attackColliders)
        {
            if (collider != null)
            {
                collider.DisableAttackCollider();
            }
        }
    }
    
    /// <summary>
    /// Executa um ataque
    /// </summary>
    public IEnumerator ExecuteAttack()
    {
        // Escolhe o próximo ataque na sequência
        currentAttack = ChooseNextAttack();
        if (currentAttack == null) 
        {
            yield break;
        }
        
        // Debug.Log($"Boss: Executando ataque: {currentAttack.attackName} (Sequência: {currentAttackIndex}/{availableAttacks.Count})");
        
        // Para de se mover durante o ataque
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
        
        // Configura o collider de ataque apropriado
        BossAttackCollider attackCollider = GetAttackCollider(currentAttack.attackName);
        if (attackCollider != null)
        {
            attackCollider.SetupAttack(
                Mathf.RoundToInt(currentAttack.damage), 
                Vector2.right * 5f, // Knockback padrão
                currentAttack.attackName
            );
        }
        
        // Vira o boss para olhar o jogador antes de atacar
        FacePlayer();
        
        // Ativa a animação do ataque
        if (animator != null)
        {
            animator.SetTrigger(currentAttack.animationTrigger);
        }
        
        OnAttackStart?.Invoke(currentAttack);
        
        // Aguarda a duração do ataque
        yield return new WaitForSeconds(currentAttack.attackDuration);
        
        // Verifica se foi interrompido durante o ataque
        if (isInterrupted)
        {
            // Debug.Log("Boss: Ataque interrompido por dano!");
            
            // Desativa o collider de ataque se ainda estiver ativo
            if (attackCollider != null)
            {
                attackCollider.DisableAttackCollider();
            }
            
            // Reseta o trigger da animação
            if (animator != null)
            {
                animator.ResetTrigger(currentAttack.animationTrigger);
            }
            
            OnAttackEnd?.Invoke(currentAttack);
            yield break;
        }
        
        // Desativa o trigger após o ataque
        if (animator != null)
        {
            animator.ResetTrigger(currentAttack.animationTrigger);
        }
        
        // Desativa o collider de ataque
        if (attackCollider != null)
        {
            attackCollider.DisableAttackCollider();
        }
        
        OnAttackEnd?.Invoke(currentAttack);
        currentComboCount++;
        
        // Verifica se completou o combo máximo
        if (currentComboCount >= maxComboLength)
        {
            // Debug.Log($"Boss: Combo máximo atingido ({currentComboCount}/{maxComboLength}), resetando contador");
            currentComboCount = 0;
            OnComboCompleted?.Invoke();
        }
    }
    
    /// <summary>
    /// Escolhe o próximo ataque na sequência
    /// </summary>
    private BossAttack ChooseNextAttack()
    {
        if (availableAttacks.Count == 0) return null;
        
        BossAttack nextAttack = availableAttacks[currentAttackIndex];
        
        // Avança para o próximo ataque na lista
        currentAttackIndex = (currentAttackIndex + 1) % availableAttacks.Count;
        
        return nextAttack;
    }
    
    /// <summary>
    /// Encontra o collider correspondente ao ataque
    /// </summary>
    private BossAttackCollider GetAttackCollider(string attackName)
    {
        Debug.Log($"🔍 BOSS ATTACK SYSTEM: GetAttackCollider chamado! Procurando: {attackName}");
        Debug.Log($"📋 BOSS ATTACK SYSTEM: Colliders disponíveis: {attackColliders.Count}");
        
        // Lista todos os colliders disponíveis
        for (int i = 0; i < attackColliders.Count; i++)
        {
            if (attackColliders[i] != null)
            {
                Debug.Log($"   [{i}] AttackName: {attackColliders[i].attackName}, Damage: {attackColliders[i].attackDamage}, InstanceID: {attackColliders[i].GetInstanceID()}");
            }
        }
        
        // Procura por um collider que corresponda ao nome do ataque
        foreach (var collider in attackColliders)
        {
            if (collider != null && collider.attackName == attackName)
            {
                Debug.Log($"✅ BOSS ATTACK SYSTEM: Collider encontrado por nome exato! AttackName: {collider.attackName}, InstanceID: {collider.GetInstanceID()}");
                return collider;
            }
        }
        
        // Se não encontrou por nome exato, tenta correspondência parcial
        foreach (var collider in attackColliders)
        {
            if (collider != null && (collider.attackName.ToLower().Contains(attackName.ToLower()) || 
                attackName.ToLower().Contains(collider.attackName.ToLower())))
            {
                Debug.Log($"✅ BOSS ATTACK SYSTEM: Collider encontrado por correspondência parcial! AttackName: {collider.attackName}, InstanceID: {collider.GetInstanceID()}");
                return collider;
            }
        }
        
        // Se ainda não encontrou, tenta buscar por índice baseado no nome do ataque
        // Attack1 = índice 0, Attack2 = índice 1, etc.
        if (attackName.StartsWith("Attack"))
        {
            string indexStr = attackName.Replace("Attack", "");
            if (int.TryParse(indexStr, out int attackIndex))
            {
                attackIndex = attackIndex - 1; // Convert to 0-based index
                if (attackIndex >= 0 && attackIndex < attackColliders.Count && attackColliders[attackIndex] != null)
                {
                    Debug.Log($"✅ BOSS ATTACK SYSTEM: Collider encontrado por índice! AttackName: {attackColliders[attackIndex].attackName}, Index: {attackIndex}, InstanceID: {attackColliders[attackIndex].GetInstanceID()}");
                    return attackColliders[attackIndex];
                }
            }
        }
        
        // Se ainda não encontrou, retorna o primeiro collider disponível
        if (attackColliders.Count > 0 && attackColliders[0] != null)
        {
            Debug.Log($"⚠️ BOSS ATTACK SYSTEM: Usando primeiro collider disponível! AttackName: {attackColliders[0].attackName}, InstanceID: {attackColliders[0].GetInstanceID()}");
            return attackColliders[0];
        }
        
        Debug.LogError($"❌ BOSS ATTACK SYSTEM: Nenhum collider encontrado para {attackName}!");
        return null;
    }
    
    /// <summary>
    /// Vira o boss para olhar o jogador
    /// </summary>
    private void FacePlayer()
    {
        if (player != null)
        {
            Vector2 directionToPlayer = (player.transform.position - transform.position).normalized;
            bool shouldFaceRight = directionToPlayer.x > 0;
            
            BossMovement movement = GetComponent<BossMovement>();
            if (movement != null && shouldFaceRight != movement.IsFacingRight)
            {
                movement.FlipSprite(shouldFaceRight);
            }
        }
    }
    
    /// <summary>
    /// Interrompe o ataque atual
    /// </summary>
    public void InterruptAttack()
    {
        isInterrupted = true;
    }
    
    /// <summary>
    /// Reseta o sistema de ataques
    /// </summary>
    public void ResetAttackSystem()
    {
        currentComboCount = 0;
        currentAttackIndex = 0;
        isInterrupted = false;
        currentAttack = null;
        
        // Desabilita todos os colliders
        foreach (var collider in attackColliders)
        {
            if (collider != null)
            {
                collider.DisableAttackCollider();
            }
        }
    }
    
    /// <summary>
    /// Adiciona um ataque à lista
    /// </summary>
    public void AddAttack(BossAttack attack)
    {
        availableAttacks.Add(attack);
    }
    
    /// <summary>
    /// Remove um ataque da lista
    /// </summary>
    public void RemoveAttack(string attackName)
    {
        availableAttacks.RemoveAll(a => a.attackName == attackName);
    }
    
    /// <summary>
    /// Configura ataques padrão
    /// </summary>
    public void SetupDefaultAttacks()
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
    
    /// <summary>
    /// Aplica configurações de um preset
    /// </summary>
    public void ApplyPreset(BossPreset preset)
    {
        maxComboLength = preset.maxComboLength;
        
        // Aplica ataques do preset
        availableAttacks.Clear();
        foreach (var attack in preset.attacks)
        {
            availableAttacks.Add(attack);
        }
    }
    
    // Métodos para serem chamados pela animação do boss
    public void EnableAttackCollider()
    {
        Debug.Log($"🎯 BOSS ATTACK SYSTEM: EnableAttackCollider chamado! CurrentAttack: {currentAttack?.attackName}");
        
        if (currentAttack != null)
        {
            BossAttackCollider attackCollider = GetAttackCollider(currentAttack.attackName);
            if (attackCollider != null)
            {
                attackCollider.EnableAttackCollider();
            }
            else
            {
                Debug.LogError($"❌ BOSS ATTACK SYSTEM: AttackCollider não encontrado para {currentAttack.attackName}!");
            }
        }
        else
        {
            Debug.LogError($"❌ BOSS ATTACK SYSTEM: currentAttack é null!");
        }
    }
    
    public void DisableAttackCollider()
    {
        if (currentAttack != null)
        {
            BossAttackCollider attackCollider = GetAttackCollider(currentAttack.attackName);
            if (attackCollider != null)
            {
                attackCollider.DisableAttackCollider();
            }
        }
    }
    
    public void EnableAttackColliderByName(string attackName)
    {
        BossAttackCollider attackCollider = GetAttackCollider(attackName);
        if (attackCollider != null)
        {
            attackCollider.EnableAttackCollider();
        }
    }
    
    public void DisableAttackColliderByName(string attackName)
    {
        BossAttackCollider attackCollider = GetAttackCollider(attackName);
        if (attackCollider != null)
        {
            attackCollider.DisableAttackCollider();
        }
    }
    
    // Métodos de informação
    public int GetTotalAttacks()
    {
        return availableAttacks.Count;
    }
    
    public string GetNextAttackName()
    {
        if (availableAttacks.Count == 0) return "Nenhum ataque";
        return availableAttacks[currentAttackIndex].attackName;
    }
}
