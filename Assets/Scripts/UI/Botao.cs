using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Botao : MonoBehaviour
{
    [SerializeField]
    private Button BotaoJogar;

    public bool andar_cima;
    public bool andar_baixo;
    public bool andar_esquerda;
    public bool andar_direita;
    GameObject MenuGameUI;

    private void Awake()
    {
        BotaoJogar.onClick.AddListener(OnButtonPlayClickJogar);
    }

    private void OnButtonPlayClickJogar()
    {
        Debug.Log("JOGAR");
        
    }

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void MovimentoBotaoCima()
    {
        andar_cima = true;
    }

    public void MovimentoBotaoBaixo()
    {
        andar_baixo = true;
    }

    public void MovimentoBotaoEsquerda()
    {
        andar_esquerda = true;
    }
    public void MovimentoBotaoDireita()
    {
        andar_direita = true;
    }

    public void pararMovimento()
    {
        andar_cima = false;
        andar_baixo = false;
        andar_esquerda = false;
        andar_direita = false;
    }
}
