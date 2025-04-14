using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// from: https://www.youtube.com/watch?v=tMXgLBwtsvI

public class ParallaxEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;

    // Starting position for the parallax game object
    Vector2 startingPosition;

    // Start Z value of the parallax game object
    float startingZ;

    // Distance that the camera has moved from the starting position of the parallax object
    Vector2 camMoveSinceStart => (Vector2) cam.transform.position - startingPosition;

    float zDistanceFromTarget => transform.position.z - followTarget.transform.position.z;

    // If object is in front of target, use near clip plane. If pehind target, use farClipPlane
    float clippingPlane => (cam.transform.position.z + (zDistanceFromTarget > 0 ? cam.farClipPlane : cam.nearClipPlane));

    // The futther the object from the player, the faster the ParallaxEffect object will move. Drag it´s Z value closer to the target to make it move slower
    float parallaxFactor => Mathf.Abs(zDistanceFromTarget) / clippingPlane;

    void Start()
    {
        startingPosition = transform.position;
        startingZ = transform.position.z;
    }

    void Update()
    {
        if(followTarget != null)
        {
            // When the target moves, move the parallax object the same distance times a multiplayer
            Vector2 newPosition = startingPosition + camMoveSinceStart * parallaxFactor;

            // The X/Y position changes based on target travel speed times the parallax factor, but Z stays consistent
            transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
        }
    }
}