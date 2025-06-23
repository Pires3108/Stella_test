using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDirection : MonoBehaviour
{
    public ContactFilter2D castFilter;
    public float GroundDistance = 0.05f;
    public float WallDistance = 0.2f;
    public float CeilingDistance = 0.05f;
    
    CapsuleCollider2D Touchingcol;
    Animator animator;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];

    [SerializeField]
    private bool _isGrounded;

    public bool IsGround 
    {
        get
        {
            return _isGrounded;
        }
        private set
        {
            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);
        }
    }

    [SerializeField]
    private bool _isOnWall;

    public bool IsOnWall 
    {
        get
        {
            return _isOnWall;
        }
        private set
        {
            _isOnWall = value;
            animator.SetBool(AnimationStrings.isOnWall, value);
        }
    }

    [SerializeField]
    private bool _isOnCeiling;
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0? Vector2.right : Vector2.left;

    public bool IsOnCeiling 
    {
        get
        {
            return _isOnCeiling;
        }
        private set
        {
            _isOnCeiling = value;
            animator.SetBool(AnimationStrings.isOnCeiling, value);
        }
    }

    // Start is called before the first frame update

    private void Awake()
    {
        Touchingcol = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        IsGround = Touchingcol.Cast(Vector2.down, castFilter, groundHits, GroundDistance) > 0;
        IsOnWall = Touchingcol.Cast(wallCheckDirection, castFilter, wallHits, WallDistance) > 0;
        IsOnCeiling = Touchingcol.Cast(Vector2.up, castFilter, ceilingHits, CeilingDistance) > 0;
    }
}
