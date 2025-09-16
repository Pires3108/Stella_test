using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackCollider : MonoBehaviour
{
    [Header("Configurações de Ataque")]
    public int attackDamage = 10;
    public Vector2 knockback = Vector2.zero;
    public string attackName = "DefaultAttack";
    
    [Header("Referências")]
    public BossFightSystem bossFightSystem;
    
    private bool hasHit = false; // Evita múltiplos hits do mesmo ataque
    
    void Start()
    {
        // Se não foi atribuído manualmente, tenta encontrar o BossFightSystem no pai
        if (bossFightSystem == null)
        {
            bossFightSystem = GetComponentInParent<BossFightSystem>();
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
        // Verifica se é o player
        if (collision.CompareTag("Player"))
        {
            if (!hasHit)
            {
                // Usa o método DealDamageToPlayer do BossFightSystem para consistência
                if (bossFightSystem != null)
                {
                    bossFightSystem.DealDamageToPlayer(attackDamage);
                    hasHit = true; // Evita múltiplos hits
                }
                else
                {
                    // Fallback: método direto se BossFightSystem não estiver disponível
                    Damageable damageable = collision.GetComponent<Damageable>();
                    if (damageable != null)
                    {
                        Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                        Vector2 deliveredKnockback = knockbackDirection * 5f;
                        
                        bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);
                        if (gotHit)
                        {
                            hasHit = true;
                        }
                    }
                }
            }
        }
    }
    
    
    // Método para ser chamado pela animação para ativar/desativar o collider
    public void EnableAttackCollider()
    {
        // Só reseta hasHit se o collider estava desabilitado (novo ataque)
        if (!GetComponent<Collider2D>().enabled)
        {
            hasHit = false; // Reseta o flag de hit para o novo ataque
        }
        
        GetComponent<Collider2D>().enabled = true;
    }
    
    public void DisableAttackCollider()
    {
        GetComponent<Collider2D>().enabled = false;
        // NÃO reseta hasHit aqui - mantém o estado até o próximo EnableAttackCollider
    }
    
    // Método para configurar o ataque dinamicamente
    public void SetupAttack(int damage, Vector2 knockbackForce, string name)
    {
        attackDamage = damage;
        knockback = knockbackForce;
        attackName = name;
    }
}
