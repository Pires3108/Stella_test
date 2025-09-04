using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Adicione este using

public class RodarPainel : MonoBehaviour
{
    [SerializeField] private List<GameObject> paineis; // Adicione seus painéis na lista pelo Inspector
    [SerializeField] private GameObject botaoAvancar;
    [SerializeField] private GameObject botaoRecuar;
    private int painelAtual = 0;

    void Start()
    {
        foreach (var painel in paineis)
        {
            painel.transform.position = Vector3.zero; // Define posição (0,0,0)
        }
        AtualizarPaineis();
    }

    public void AvancarPainel()
    {
        if (painelAtual < paineis.Count - 1)
        {
            painelAtual++;
            AtualizarPaineis();
        }
    }

    public void RecuarPainel()
    {
        if (painelAtual > 0)
        {
            painelAtual--;
            AtualizarPaineis();
        }
    }

    private void AtualizarPaineis()
    {
        for (int i = 0; i < paineis.Count; i++)
        {
            paineis[i].SetActive(i == painelAtual);
        }

        // Mostra apenas o botão de avançar se painelAtual for 0,
        // apenas o de recuar se for 2, e ambos se for 1
        botaoAvancar.SetActive(painelAtual == 0 || painelAtual == 1);
        botaoRecuar.SetActive(painelAtual == 1 || painelAtual == 2);
    }
}
