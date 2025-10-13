using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Attack : MonoBehaviour
{
    public int attackDamage = 10; // Dano individual deste ataque
    public Vector2 KnockBack = Vector2.zero;
    
    private PlayerController playerController;
    private int baseDamage; // Dano base original deste ataque
    
    void Start()
    {
        // Inicializa a referência
        playerController = FindObjectOfType<PlayerController>();
        // Preserva o dano original deste ataque
        baseDamage = attackDamage;
    }
    
    /// <summary>
    /// Aumenta o dano deste ataque específico
    /// </summary>
    /// <param name="increaseAmount">Quantidade a ser aumentada</param>
    public void IncreaseDamage(int increaseAmount)
    {
        attackDamage += increaseAmount;
        Debug.Log($"Attack {gameObject.name} dano aumentado para {attackDamage}");
    }
    
    /// <summary>
    /// Obtém o dano atual do ataque
    /// </summary>
    public int GetCurrentDamage()
    {
        return attackDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Obtém o dano atual do PlayerController
        int currentDamage = GetCurrentDamage();
        
        //see if it can be hit
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? KnockBack : new Vector2(-KnockBack.x, KnockBack.y);

            bool gotHit = damageable.Hit(currentDamage, deliveredKnockback);
        }

        // Verifica se é um boss e causa dano
        if (collision.CompareTag("Boss1") || collision.CompareTag("Boss2") || 
            collision.CompareTag("Boss3") || collision.CompareTag("Boss4"))
        {
            BossFightSystem bossFightSystem = collision.GetComponent<BossFightSystem>();
            if (bossFightSystem != null)
            {
                // Calcula knockback baseado na direção do player
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                Vector2 knockback = knockbackDirection * KnockBack.magnitude;
                
                bossFightSystem.TakeDamageFromPlayer(currentDamage, knockback);
                Debug.Log($"Player causou {currentDamage} de dano ao boss!");
            }
        }
    }
}
