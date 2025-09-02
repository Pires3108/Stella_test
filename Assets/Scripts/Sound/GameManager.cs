using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public AudioSource somMusica;
    public AudioSource somEfeitos;
    void Start()
    {
        somMusica.volume = PlayerPrefs.GetFloat("volMusica", 0.5f);
        somEfeitos.volume = PlayerPrefs.GetFloat("volEfeitos", 0.5f);
    }
}
