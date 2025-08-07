using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] public GameObject projectilePrefab;
    public Transform launchPoint;
    public bool canFire = true;
    
    public void FireProjectile()
    {
        if (!canFire)
        {
            return; // Exit if the launcher cannot fire
        }
        
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, projectilePrefab.transform.rotation);
        Vector3 originalScale = projectile.transform.localScale;

        // Get the player's facing direction from the parent transform
        bool isFacingRight = transform.eulerAngles.y == 0;
        
        // Set the projectile's scale based on the player's facing direction
        // If player is facing right (y = 0), keep positive scale; if left (y = 180), make negative
        float directionMultiplier = isFacingRight ? 1f : -1f;
        
        projectile.transform.localScale = new Vector3(
            originalScale.x * directionMultiplier,
            originalScale.y,
            originalScale.z
        );
    }
}