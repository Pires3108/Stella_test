using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public Vector2 KnockBack = Vector2.zero;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //see if it can be hit
        Damageable damageable = collision.GetComponent<Damageable>();

        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? KnockBack : new Vector2(-KnockBack.x, KnockBack.y);

            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);
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
                
                bossFightSystem.TakeDamageFromPlayer(attackDamage, knockback);
                Debug.Log($"Player causou {attackDamage} de dano ao boss!");
            }
        }
    }
}
