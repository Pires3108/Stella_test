using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TelaDialogo : MonoBehaviour
{
    [Header("Imports")]
    [SerializeField]
    private Image Foto;

    [SerializeField]
    private TextMeshProUGUI Nome;

    [SerializeField]
    private TextMeshProUGUI Fala;

    private Dialogo dialogo;

    private static TelaDialogo instancia;
    public GameController gameController;


    private void Awake()
    {
        Esconder();
        instancia = this;
    }

    public static TelaDialogo Instancia
    {
        get
        {
            return instancia;
        }
    }

    public void Exibir(Dialogo dialogo)
    {
        gameController.IsDialogActive = true;
        this.dialogo = dialogo;
        this.dialogo.Iniciar();

        ExibirFalaAtual();
        this.gameObject.SetActive(true);
    }

    private void Esconder()
    {
        this.gameObject.SetActive(false);
        gameController.IsDialogActive = false;
    }

    public void Avancar()
    {

        if (this.dialogo.TemProximaFala())
        {
            this.dialogo.AvancarFala();
            ExibirFalaAtual();
            Debug.Log("Avançar diálogo");
        }

        else
        {
            Esconder();
            Debug.Log("Dialogo finalizado");
        }
    }

    private void ExibirFalaAtual()
    {
        FalaDialogo falaAtual = this.dialogo.FalaAtual;
        Ator ator = falaAtual.Ator;

        this.Foto.sprite = ator.Foto;
        this.Nome.text = ator.Nome;
        this.Fala.text = falaAtual.Texto;
    }
}
