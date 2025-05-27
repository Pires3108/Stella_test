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
        if(damageable != null)
        {
            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? KnockBack : new Vector2(-KnockBack.x, KnockBack.y);

            bool gotHit = damageable.Hit(attackDamage, deliveredKnockback);

            if(gotHit)
            {
                Debug.Log(collision.name + "hit for" + attackDamage);  
            }
        }
    }
}
