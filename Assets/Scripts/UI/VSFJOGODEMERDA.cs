using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class VSFJOGODEMERDA : MonoBehaviour
{
    public float pixelsPerUnit = 32f;

    void LateUpdate()
    {
        Vector3 newPos = transform.position;
        newPos.x = Mathf.Round(newPos.x * pixelsPerUnit) / pixelsPerUnit;
        newPos.y = Mathf.Round(newPos.y * pixelsPerUnit) / pixelsPerUnit;
        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }
}