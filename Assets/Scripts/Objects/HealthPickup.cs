using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script para maçãs normais (vermelhas) de cura
/// Restaura a vida ao máximo sem causar bloqueio de movimento ou animação
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
            
            // Maçã normal não causa bloqueio de movimento nem animação
            // Apenas restaura a vida e destrói o objeto
            
            Destroy(gameObject);
        }
    }
    

    private void Update()
    {
        // Rotaciona o pickup
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }
}
