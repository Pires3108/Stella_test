using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script de compatibilidade para maçãs de cura
/// Agora sempre enche a vida ao máximo e ativa animação ComendoMaca
/// </summary>
public class HealthPickup : MonoBehaviour
{
    [Header("Legacy Settings")]
    public int healthAmount = 20; // Amount of health to restore (não usado mais)   
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0); // Speed of rotation
    
    void Start()
    {
        // Mantém rotação para compatibilidade visual
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        Debug.Log("Collision with: " + collision.gameObject.name);
        if (damageable)
        {
            // Sempre enche a vida ao máximo (conforme solicitado)
            damageable.Health = damageable.MaxHealth;
            Debug.Log("Vida restaurada ao máximo!");
            
            // Bloqueia movimento durante a animação
            damageable.LockVelocity = true;
            
            // Zera a velocidade do player para garantir que ele fique parado
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector2.zero;
            }
            
            // Ativa animação de comer maçã
            Animator playerAnimator = collision.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger(AnimationStrings.eatingApple);
            }
            
            // Desbloqueia movimento após a animação
            StartCoroutine(UnlockMovementAfterAnimation(damageable));
            
            Destroy(gameObject);
        }
    }
    
    private IEnumerator UnlockMovementAfterAnimation(Damageable damageable)
    {
        // Aguarda um tempo para a animação de comer maçã terminar
        yield return new WaitForSeconds(1.5f); // Ajuste este valor conforme a duração da animação
        
        // Desbloqueia o movimento
        if (damageable != null)
        {
            damageable.LockVelocity = false;
        }
    }

    private void Update()
    {
        // Rotaciona o pickup
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }
}
