using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Opcoes : MonoBehaviour
{
    [Header("Slider Canvas")]
    public Slider sliderMusica;
    public Slider sliderEfeitos;

    [Header("Audio")]
    public AudioSource audioMusica;
    public AudioSource audioEfeitos;
    public AudioClip somClip;
    float vMusica;
    float vEfeitos;

    void Start()
    {
        vMusica = PlayerPrefs.GetFloat("volMusica", 0.5f);
        vEfeitos = PlayerPrefs.GetFloat("volEfeitos", 0.5f);
        sliderMusica.value = vMusica * 100f; // Ajuste para slider 0-100
        sliderEfeitos.value = vEfeitos;
        audioMusica.volume = vMusica;
        audioEfeitos.volume = vEfeitos;
    }

    public void VolumeMusica()
    {
        vMusica = sliderMusica.value / 100f; // Ajuste para volume 0-1
        audioMusica.volume = vMusica;
        PlayerPrefs.SetFloat("volMusica", vMusica);
    }

    public void VolumeEfeitos()
    {
        vEfeitos = sliderEfeitos.value;
        audioEfeitos.volume = vEfeitos;
        audioEfeitos.PlayOneShot(somClip);
        PlayerPrefs.SetFloat("volEfeitos", vEfeitos);
    }

    public void SalvarOpcoes()
    {
        PlayerPrefs.Save();
    }
}
