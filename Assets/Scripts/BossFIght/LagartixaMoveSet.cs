using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LagartixaMoveSet : MonoBehaviour
{
    public GameObject trofeuFase;
    public Animator animator;
    public Rigidbody2D rb;
    public GameObject player;
    public SpriteRenderer spriteRenderer; // Adicione esta linha

    public float approachSpeed = 3f;
    public float attackDistance = 2f;
    private int comboStep = 0;
    private bool isAttacking = false;
    private bool isWalking = false; // Adicione esta linha

    void Update()
    {
        if (!isAttacking)
        {
            StartCoroutine(ComboCycle());
        }
    }

    IEnumerator ComboCycle()
    {
        isAttacking = true;

        // Step 1: Aproxima para ComboGiro
        yield return StartCoroutine(MoveToPlayer());
        ComboGiro();
        yield return new WaitForSeconds(1f);

        // Step 2: Aproxima para PunchRight
        yield return StartCoroutine(MoveToPlayer());
        PunchRight();
        yield return new WaitForSeconds(1f);

        // Step 3: Aproxima para PunchLeft
        yield return StartCoroutine(MoveToPlayer());
        PunchLeft();
        yield return new WaitForSeconds(1f);

        isAttacking = false;
    }

    IEnumerator MoveToPlayer()
    {
        isWalking = true;
        animator.SetBool("isWalking", true); // Ativa animação Walk

        while (Vector2.Distance(transform.position, player.transform.position) > attackDistance)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;

            // Flip sprite: ativa flipX se estiver indo para a direita
            if (direction.x > 0)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;

            // Mantém o boss no chão (preserva Y)
            Vector2 newPosition = new Vector2(rb.position.x + direction.x * approachSpeed * Time.fixedDeltaTime, rb.position.y);
            rb.MovePosition(newPosition);

            yield return null;
        }

        isWalking = false;
        animator.SetBool("isWalking", false); // Desativa animação Walk
    }

    public void ComboGiro()
    {
        // Exemplo de animação
        animator.SetTrigger("ComboGiro");
        Debug.Log("ComboGiro!");
    }

    public void PunchRight()
    {
        animator.SetTrigger("PunchRight");
        Debug.Log("PunchRight!");
    }

    public void PunchLeft()
    {
        animator.SetTrigger("PunchLeft");
        Debug.Log("PunchLeft!");
    }
}
