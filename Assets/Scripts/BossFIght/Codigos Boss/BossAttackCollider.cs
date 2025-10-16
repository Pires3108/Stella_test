using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackCollider : MonoBehaviour
{
    [Header("Configurações de Ataque")]
    public int attackDamage;
    public Vector2 knockback = Vector2.zero;
    public string attackName = "DefaultAttack";
    
    [Header("Referências")]
    public BossFightSystem bossFightSystem;
    public BossHealthSystem bossHealthSystem;
    
    // Sistema hasHit removido - cada ataque pode causar dano
    
    void Start()
    {
        // Se não foi atribuído manualmente, tenta encontrar os sistemas no pai
        if (bossFightSystem == null)
        {
            bossFightSystem = GetComponentInParent<BossFightSystem>();
        }
        
        if (bossHealthSystem == null)
        {
            bossHealthSystem = GetComponentInParent<BossHealthSystem>();
        }
        
        // Desabilita o collider por padrão
        GetComponent<Collider2D>().enabled = false;
        
        // Verifica se está configurado como trigger
        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            Debug.LogError($"❌ BOSS ATTACK COLLIDER: Collider '{attackName}' NÃO está configurado como Trigger!");
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"🔍 BOSS ATTACK COLLIDER: OnTriggerEnter2D chamado! Collision: {collision.name}, Tag: {collision.tag}");
        Debug.Log($"🎯 COLLIDER INFO: AttackName: {attackName}, Damage: {attackDamage}, GameObject: {gameObject.name}, InstanceID: {GetInstanceID()}");
        
        // Verifica se é o player
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"✅ BOSS ATTACK COLLIDER: Player detectado! attackDamage: {attackDamage}");
            Debug.Log($"🎯 BOSS ATTACK COLLIDER: Aplicando dano...");
            
            // Usa o método DealDamageToPlayer do BossHealthSystem para consistência
            if (bossHealthSystem != null)
            {
                Debug.Log($"💥 BOSS ATTACK COLLIDER: Usando BossHealthSystem.DealDamageToPlayer({attackDamage})");
                bossHealthSystem.DealDamageToPlayer(attackDamage);
                Debug.Log($"✅ BOSS ATTACK COLLIDER: Dano aplicado via BossHealthSystem!");
            }
            else if (bossFightSystem != null)
            {
                Debug.Log($"💥 BOSS ATTACK COLLIDER: Usando BossFightSystem.DealDamageToPlayer({attackDamage})");
                // Fallback para compatibilidade
                bossFightSystem.DealDamageToPlayer(attackDamage);
                Debug.Log($"✅ BOSS ATTACK COLLIDER: Dano aplicado via BossFightSystem!");
            }
            else
            {
                Debug.Log($"⚠️ BOSS ATTACK COLLIDER: Nenhum sistema encontrado, tentando método direto...");
                // Fallback: método direto se nenhum sistema estiver disponível
                Damageable damageable = collision.GetComponent<Damageable>();
                if (damageable != null)
                {
                    Debug.Log($"💥 BOSS ATTACK COLLIDER: Usando Damageable.Hit({attackDamage}) diretamente");
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    Vector2 deliveredKnockback = knockbackDirection * 5f;
                    
                    bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);
                    if (gotHit)
                    {
                        Debug.Log($"✅ BOSS ATTACK COLLIDER: Dano aplicado diretamente!");
                    }
                    else
                    {
                        Debug.Log($"❌ BOSS ATTACK COLLIDER: Damageable.Hit retornou false!");
                    }
                }
                else
                {
                    Debug.LogError($"❌ BOSS ATTACK COLLIDER: Damageable component não encontrado no player!");
                }
            }
        }
        else
        {
            Debug.Log($"❌ BOSS ATTACK COLLIDER: Objeto não é o player! Tag: {collision.tag}");
        }
    }
    
    
    // Método para ser chamado pela animação para ativar/desativar o collider
    public void EnableAttackCollider()
    {
        Debug.Log($"🔧 BOSS ATTACK COLLIDER: EnableAttackCollider chamado! Attack: {attackName}");
        
        GetComponent<Collider2D>().enabled = true;
        Debug.Log($"✅ BOSS ATTACK COLLIDER: Collider ativado! enabled = true");
    }
    
    public void DisableAttackCollider()
    {
        GetComponent<Collider2D>().enabled = false;
        Debug.Log($"🔧 BOSS ATTACK COLLIDER: Collider desativado! Attack: {attackName}");
    }
    
    // Método para configurar o ataque dinamicamente
    public void SetupAttack(int damage, Vector2 knockbackForce, string name)
    {
        attackDamage = damage;
        knockback = knockbackForce;
        attackName = name;
    }
    
}
