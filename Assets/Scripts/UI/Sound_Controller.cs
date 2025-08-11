using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Sound_Controller : MonoBehaviour
{
    public Slider Musicas;
    public Slider Efeitos;

    public TextMeshProUGUI MusicaText;
    public TextMeshProUGUI EfeitoText;

    public GameObject Opcoes;
    public Animator OpcoesAnim;

    public Button AbrirOpcoesButton;
    public Button FecharOpcoesButton;


    public void Awake()
    {
        Opcoes.transform.position = new Vector3(1921, 0, 0);

        Musicas.value = 50;
        Efeitos.value = 50;
        AtualizarMusicaSlider();
        AtualizarEfeitoSlider();

        AbrirOpcoesButton.onClick.AddListener(AbrirOpcoes);
        FecharOpcoesButton.onClick.AddListener(FecharOpcoes);

        Musicas.onValueChanged.AddListener(delegate { AtualizarMusicaSlider(); });
        Efeitos.onValueChanged.AddListener(delegate { AtualizarEfeitoSlider(); });
    }

    public void AtualizarMusicaSlider()
    {
        int valor = Mathf.RoundToInt(Musicas.value);
        MusicaText.text = valor + "%";
    }

    public void AtualizarEfeitoSlider()
    {
        int valor = Mathf.RoundToInt(Efeitos.value);
        EfeitoText.text = valor + "%";
    }

    public void AbrirOpcoes()
    {
        OpcoesAnim.SetBool("isActivate", true);
    }

    public void FecharOpcoes()
    {
        OpcoesAnim.SetBool("isActivate", false);
    }
}
