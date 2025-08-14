using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerFight : MonoBehaviour
{
    public Slider barraBoss;
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            barraBoss.gameObject.SetActive(true);
        }
    }
}
