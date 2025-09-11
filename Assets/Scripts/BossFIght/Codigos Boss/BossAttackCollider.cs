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
        
        // Debug da configuração inicial
        Debug.Log($"🔧 BOSS ATTACK COLLIDER: '{attackName}' inicializado - Collider2D enabled: {GetComponent<Collider2D>().enabled}, IsTrigger: {GetComponent<Collider2D>().isTrigger}");
        
        // Verifica se está configurado como trigger
        Collider2D col = GetComponent<Collider2D>();
        if (!col.isTrigger)
        {
            Debug.LogError($"❌ BOSS ATTACK COLLIDER: Collider '{attackName}' NÃO está configurado como Trigger! Isso impedirá a detecção de colisão.");
        }
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"🔴 BOSS ATTACK: Collider '{attackName}' detectou: {collision.name} (Tag: {collision.tag})");
        
        // Verifica se é o player
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"🟢 BOSS ATTACK: PLAYER DETECTADO! Nome: {collision.name}");
            
            // Verifica se pode causar dano
            Damageable damageable = collision.GetComponent<Damageable>();
            
            if (damageable != null)
            {
                if (!hasHit)
                {
                    Debug.Log($"🟡 BOSS ATTACK: Tentando causar dano... Dano: {attackDamage}");
                    
                    // Calcula knockback simples
                    Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                    Vector2 deliveredKnockback = knockbackDirection * 5f; // Knockback fixo de 5
                    
                    Debug.Log($"🟡 BOSS ATTACK: Knockback calculado: {deliveredKnockback}");
                    
                    bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);
                    
                    if (gotHit)
                    {
                        Debug.Log($"✅ BOSS ATTACK: SUCESSO! Dano {attackDamage} aplicado ao player!");
                        hasHit = true; // Evita múltiplos hits
                    }
                    else
                    {
                        Debug.Log($"❌ BOSS ATTACK: FALHOU - Player invulnerável ou morto");
                    }
                }
                else
                {
                    Debug.Log($"⚠️ BOSS ATTACK: Já causou dano neste ataque");
                }
            }
            else
            {
                Debug.LogError($"❌ BOSS ATTACK: Player não tem componente Damageable! Objeto: {collision.name}");
            }
        }
        else
        {
            Debug.Log($"🔵 BOSS ATTACK: Colisão com objeto que não é player: {collision.name} (Tag: {collision.tag})");
        }
    }
    
    // Método para ser chamado pela animação para ativar/desativar o collider
    public void EnableAttackCollider()
    {
        GetComponent<Collider2D>().enabled = true;
        hasHit = false; // Reseta o flag de hit
        Debug.Log($"🟢 COLLIDER ATIVADO: '{attackName}' - Collider2D enabled: {GetComponent<Collider2D>().enabled}, IsTrigger: {GetComponent<Collider2D>().isTrigger}");
    }
    
    public void DisableAttackCollider()
    {
        GetComponent<Collider2D>().enabled = false;
        hasHit = false; // Reseta o flag de hit
        Debug.Log($"🔴 COLLIDER DESATIVADO: '{attackName}' - Collider2D enabled: {GetComponent<Collider2D>().enabled}");
    }
    
    // Método para configurar o ataque dinamicamente
    public void SetupAttack(int damage, Vector2 knockbackForce, string name)
    {
        attackDamage = damage;
        knockback = knockbackForce;
        attackName = name;
        Debug.Log($"🔧 BOSS ATTACK COLLIDER: '{attackName}' configurado - Dano: {damage}, Knockback: {knockbackForce}");
    }
}
