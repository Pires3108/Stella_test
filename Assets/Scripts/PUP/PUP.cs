using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PUP : MonoBehaviour
{
    [Header("UI")]
    public Canvas TextoLevelUp;
    public GameObject powerupTextPrefab; // Arraste o prefab do texto aqui

    [Header("Novos valores de vida e estamina")]
    public int novoMaxHealth = 100;
    public float novoMaxStamina = 100f;

    private Animator animator;

    void Awake()
    {
        TextoLevelUp = GameObject.Find("TextoLevelUp").GetComponent<Canvas>();
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Atualiza e restaura vida
            Damageable dmg = other.GetComponent<Damageable>();
            if (dmg != null)
            {
                dmg.MaxHealth = novoMaxHealth;
                dmg.Health = dmg.MaxHealth;
            }

            // Atualiza e restaura estamina
            Estamina estamina = other.GetComponent<Estamina>();
            if (estamina != null)
            {
                estamina.MaxEnergy = novoMaxStamina;
                estamina.Energy = estamina.MaxEnergy;
            }

            // Exibe texto em cima do player
            if (powerupTextPrefab != null)
            {
                Vector3 spawnPosition = Camera.main.WorldToScreenPoint(other.transform.position);
                Vector3 offset = new Vector3(0, 60f, 0);

                TMP_Text tmp_Text = Instantiate(
                    powerupTextPrefab,
                    spawnPosition + offset,
                    Quaternion.identity,
                    TextoLevelUp.transform
                ).GetComponent<TMP_Text>();

                tmp_Text.text = "Vida & Estamina Restauradas!";
                Destroy(tmp_Text.gameObject, 1.0f);
            }

            // Toca animação de sumir
            if (animator != null)
            {
                animator.SetTrigger("SumirMaca");
            }

            // Destroi o objeto após a animação (ajuste o tempo conforme necessário)
            Destroy(gameObject, 1f);
        }
    }
}