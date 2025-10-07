using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    public int damage = 10; // Dano individual deste projétil
    public Vector2 moveSpeed = new Vector2(3f, 0);
    public Vector2 KnockBack = new Vector2(0, 0); // Knockback force applied to the target
    Rigidbody2D rb; // Rigidbody2D component for physics interactions
    
    private PlayerController playerController;
    private int baseDamage; // Dano base original deste projétil

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = FindObjectOfType<PlayerController>();
    }
    
    void Start()
    {
        rb.velocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y);
        // Preserva o dano original deste projétil
        baseDamage = damage;
    }
    
    /// <summary>
    /// Aumenta o dano deste projétil específico
    /// </summary>
    /// <param name="increaseAmount">Quantidade a ser aumentada</param>
    public void IncreaseDamage(int increaseAmount)
    {
        damage += increaseAmount;
        Debug.Log($"Projectile {gameObject.name} dano aumentado para {damage}");
    }
    
    /// <summary>
    /// Obtém o dano atual do projétil
    /// </summary>
    public int GetCurrentDamage()
    {
        return damage;
    }

    //codigo da flecha do player
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Obtém o dano atual do PlayerController
        int currentDamage = GetCurrentDamage();
        
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.localScale.x > 0 ? KnockBack : new Vector2(-KnockBack.x, KnockBack.y);

            bool gotHit = damageable.Hit(currentDamage, deliveredKnockback);

            if (gotHit)
            {
                Destroy(gameObject); // Destroy the projectile after hitting
            }
        }
        
        // Verifica se é um boss e causa dano
        BossFightSystem bossFightSystem = collision.GetComponent<BossFightSystem>();
        if (bossFightSystem != null)
        {
            // Calcula knockback baseado na direção da flecha
            Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
            Vector2 knockback = knockbackDirection * KnockBack.magnitude;
            
            bossFightSystem.TakeDamageFromPlayer(currentDamage, knockback);
            Debug.Log($"Flecha do player causou {currentDamage} de dano ao boss!");
            
            Destroy(gameObject); // Destroy the projectile after hitting boss
        }
    }
}



