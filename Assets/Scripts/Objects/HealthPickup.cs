using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healthAmount = 20; // Amount of health to restore   
    public Vector3 spinRotationSpeed = new Vector3(0, 180, 0); // Speed of rotation
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        Debug.Log("Collision with: " + collision.gameObject.name);
        if (damageable)
        {
            bool wasHealed = damageable.Heal(healthAmount);
            if(wasHealed)
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Rotate the pickup
        transform.eulerAngles += spinRotationSpeed * Time.deltaTime;
    }
}
