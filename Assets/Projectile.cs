using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10; // Damage dealt by the projectile
    public Vector2 moveSpeed = new Vector2(3f, 0);
    public Vector2 KnockBack = new Vector2(3f, 0); // Knockback force applied to the target

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.velocity = new Vector2(moveSpeed.x * transform.localScale.x, moveSpeed.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the projectile collides with a Damageable object
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? KnockBack : new Vector2(-KnockBack.x, KnockBack.y);

            bool gotHit = damageable.Hit(damage, deliveredKnockback);

            if (gotHit)
            {
                Debug.Log(collision.name + "hit for" + damage);
            }
        }
    }
}



