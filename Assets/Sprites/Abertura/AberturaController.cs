using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AberturaController : MonoBehaviour
{
    public Animator animatorAbertura;
    public bool isLoading = true; // está carretando a animação
    public float delay;// Tempo de espera antes de trocar a cena

    void Start()
    {
        animatorAbertura.Play("AberturaAnimacao");
        StartCoroutine(PodeTrocarScenne());
    }

    void Update()
    {
        if (isLoading == false)
        {
            SceneManager.LoadScene("Menu"); // Carrega a cena MenuPrincipal
        }

    }

    IEnumerator PodeTrocarScenne()
    {
        yield return new WaitForSeconds(delay); // Espera 2 segundos antes de desligar a animação
        isLoading = false; // não está carregando a animaão
    }
}
