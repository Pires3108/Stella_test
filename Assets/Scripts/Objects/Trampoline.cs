using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [SerializeField]
    float Impulso;
    [SerializeField]
    Animator animator;


    void OnCollisionEnter2D(Collision2D colisao)
    {
        if(colisao.gameObject.CompareTag("Player"))
        {
            colisao.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * Impulso, ForceMode2D.Impulse);
            animator.SetBool("Trampoline_Jump", true);
        }
    }

    void OnCollisionExit2D(Collision2D colisao)
    {
        if(colisao.gameObject.CompareTag("Player"))
        {
            animator.SetBool("Trampoline_Jump", false);
        }
    }
}
