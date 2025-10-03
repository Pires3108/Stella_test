using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TocarTrilha : MonoBehaviour
{
    [Header("Imports")]
    public AudioSource Trilha1;
    public GameObject Transição;
    public void Update()
    {
        if (Transição.activeSelf == true)
        {
            Trilha1.Play();
        }
    }
}
