using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    public UnityEvent<int, Vector2>damageableHit;

    Animator animator;
    [SerializeField]
    private int _maxHealth = 100;



    public int MaxHealth{
        get{
            return _maxHealth;
        }
        set{
            _maxHealth = value;
        }
    }

    [SerializeField]
    private int _health = 100;


    public int Health{
        get{
            return _health;
        }
        set{
            _health = value;

            if(_health <= 0 )
            {
                IsAlive = false;
            }
        }
    }

    [SerializeField] private bool _isAlive = true;
    [SerializeField] private bool isInvincible = false;


    public float invicibilityTime = 0.25f;
    private float timeSinceHit = 0;

    public bool IsAlive{
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log ("IsAlive set" + value);
        }
    }
     
    public bool LockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInvincible)
        {
            if (timeSinceHit > invicibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }

    }

    public bool Hit(int damage, Vector2 KnockBack){
        if(IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;
            LockVelocity = true;
            animator.SetTrigger(AnimationStrings.hitTrigger);
            damageableHit?.Invoke(damage, KnockBack);
            CharacterEvents.characterDamaged.Invoke(gameObject, damage);
            return true;
        }
        return false;
    }
}
