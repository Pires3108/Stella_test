using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirection), typeof(Damageable))]
public class Inimigo : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float walkStopRate = 0.05f;
    public DetectionZone AtackZone;
    Rigidbody2D rb;
    TouchingDirection touchingDirection;
    Damageable damageable;
    Animator animator;
    public enum WalkableDirection {Right, Left};
    private WalkableDirection _walkDirection;
    public Vector2 walkDirectionVector = Vector2.right;
    
    public WalkableDirection WalkDirection
    {
        get {return _walkDirection;}
        set{
            if(_walkDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x *-1, gameObject.transform.localScale.y);
                if(value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                } else if(value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value; }
        
    }

    public bool _HasTarget = false;
    public bool HasTarget { 
        get { return _HasTarget; }
        private set
        {
            _HasTarget = value;
            animator.SetBool(AnimationStrings.HasTarget, value);
        }
    }

    public bool canMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }



    private void Awake(){
        rb = GetComponent<Rigidbody2D>();
        touchingDirection = GetComponent<TouchingDirection>();
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
    }

    private void Update()
    {
        HasTarget = AtackZone.DetectColliders.Count > 0;
    }

    private void FixedUpdate(){
        if(touchingDirection.IsGround && touchingDirection.IsOnWall)
        {
            FlipDirection();
        }
        if(!damageable.LockVelocity)
        {
            if(canMove)
            {
                rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
            }
        }
    }

    private void FlipDirection()
    {
        if(WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        } else if(WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        } else 
        {
            Debug.LogError("Current walkable direction is not set to legal valuer of right or left");
        }
    }

    public void OnHit(int damage, Vector2 KnockBack)
    {
        rb.velocity = new Vector2(KnockBack.x, rb.velocity.y + KnockBack.y);
    }
}
