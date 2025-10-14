using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUP : MonoBehaviour
{
    [Header("PUP Imports")]
    public GameObject pup;
    public Animator anim;
    public CircleCollider2D col;

    [Header("Imports")]
    public GameObject player;
    
    [Header("Power-up Settings")]
    public PowerUpType powerUpType = PowerUpType.Health;
    
    [Header("Upgrade Settings")]
    public int healthIncrease; // Aumento da vida máxima
    public int staminaIncrease; // Aumento da estamina máxima
    public int damageIncrease; // Aumento do dano

    public enum PowerUpType
    {
        Health,     // Maçã vermelha - restaura vida ao máximo
        Upgrade     // Maçã dourada - aumenta stats máximos
    }

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Stella");
        pup = this.gameObject;
    }

    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            // Aplica o efeito do power-up antes de destruir
            ApplyPowerUpEffect();
        }
    }

    private void ApplyPowerUpEffect()
    {
        if (player == null) return;
        player.GetComponent<Animator>().Play("Player_Idle");
        player.GetComponent<Animator>().speed = 0;
        Damageable damageable = player.GetComponent<Damageable>();
        Estamina stamina = player.GetComponent<Estamina>();
        PlayerController playerController = player.GetComponent<PlayerController>();
        Animator playerAnimator = player.GetComponent<Animator>();

        switch (powerUpType)
        {
            case PowerUpType.Health:
                // Sempre enche a vida ao máximo
                if (damageable != null)
                {
                    damageable.Health = damageable.MaxHealth;
                    Debug.Log("Vida restaurada ao máximo!");
                }
                break;

            case PowerUpType.Upgrade:
                // Aumenta stats máximos
                if (damageable != null)
                {
                    damageable.MaxHealth += healthIncrease;
                    damageable.Health = damageable.MaxHealth; // Enche vida ao máximo
                    Debug.Log($"Vida máxima aumentada para {damageable.MaxHealth}");
                }

                if (stamina != null)
                {
                    stamina.MaxEnergy += staminaIncrease;
                    stamina.Energy = stamina.MaxEnergy; // Enche estamina ao máximo
                    Debug.Log($"Estamina máxima aumentada para {stamina.MaxEnergy}");
                }

                // Aumenta dano em todos os ataques usando o PlayerController
                if (playerController != null)
                {
                    Debug.Log($"PUP: Chamando IncreaseAllDamage com {damageIncrease}");
                    playerController.IncreaseAllDamage(damageIncrease);
                }
                else
                {
                    Debug.LogError("PUP: PlayerController não encontrado!");
                }
                break;
        }

        // Bloqueia movimento durante a animação
        if (damageable != null)
        {
            damageable.LockVelocity = true;
        }

        // Zera a velocidade do player para garantir que ele fique parado
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.velocity = Vector2.zero;
        }

        // Ativa animação de comer maçã
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(AnimationStrings.eatingApple);
        }

        // Desbloqueia movimento após a animação usando o PlayerController
        if (playerController != null)
        {
            playerController.StartCoroutine(playerController.UnlockMovementAfterAnimation());
        }

        // Destrói o objeto imediatamente
        Destroy(gameObject);
    }
}
