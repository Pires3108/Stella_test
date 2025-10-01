using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Sound_Controller : MonoBehaviour
{
    [Header("Musica - Slider")]
    public Slider Musicas;
    public TextMeshProUGUI MusicaText;

    [Header("Efeito - Slider")]
    public Slider Efeitos;
    public TextMeshProUGUI EfeitoText;

    [Header("Botões")]
    public Button AbrirOpcoesButton;
    public Button FecharOpcoesButton;
    public Button SalvarOpcoesButton;


    [Header("Opcoes Painel")]
    public GameObject Opcoes;


    private float musicaSalva;
    private float efeitoSalvo;

    public void Awake()
    {
        Opcoes.SetActive(false);

        // Carrega valores salvos ou usa 50 como padrão
        musicaSalva = PlayerPrefs.GetFloat("MusicaVolume", 50);
        efeitoSalvo = PlayerPrefs.GetFloat("EfeitoVolume", 50);

        Musicas.value = musicaSalva;
        Efeitos.value = efeitoSalvo;
        AtualizarMusicaSlider();
        AtualizarEfeitoSlider();

        AbrirOpcoesButton.onClick.AddListener(AbrirOpcoes);
        FecharOpcoesButton.onClick.AddListener(FecharOpcoes);
        SalvarOpcoesButton.onClick.AddListener(SalvarOpcoes);

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
        // Carrega valores salvos ao abrir
        musicaSalva = PlayerPrefs.GetFloat("MusicaVolume", 50);
        efeitoSalvo = PlayerPrefs.GetFloat("EfeitoVolume", 50);

        Musicas.value = musicaSalva;
        Efeitos.value = efeitoSalvo;
        AtualizarMusicaSlider();
        AtualizarEfeitoSlider();

        Opcoes.SetActive(true);
    }

    public void FecharOpcoes()
    {
        // Restaura valores antigos ao fechar sem salvar
        Musicas.value = musicaSalva;
        Efeitos.value = efeitoSalvo;
        AtualizarMusicaSlider();
        AtualizarEfeitoSlider();

        Opcoes.SetActive(false);
    }

    public void SalvarOpcoes()
    {
        // Salva os valores atuais
        PlayerPrefs.SetFloat("MusicaVolume", Musicas.value);
        PlayerPrefs.SetFloat("EfeitoVolume", Efeitos.value);
        PlayerPrefs.Save();

        // Atualiza os valores salvos
        musicaSalva = Musicas.value;
        efeitoSalvo = Efeitos.value;

        Opcoes.SetActive(false);
    }
}