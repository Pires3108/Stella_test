using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    [SerializeField] public GameObject projectilePrefab;
    public Transform launchPoint;

    public void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, projectilePrefab.transform.rotation);
        Vector3 originalScale = projectile.transform.localScale;

        // Adjust the projectile's scale based on the player's facing direction
        // If the parent (player) is facing right, set scale to positive; if left, set to negative
        projectile.transform.localScale = new Vector3(
            originalScale.x * transform.localScale.x > 0 ? 1 : -1, // Adjust scale based on parent scale
            originalScale.y,
            originalScale.z);
    }

}
