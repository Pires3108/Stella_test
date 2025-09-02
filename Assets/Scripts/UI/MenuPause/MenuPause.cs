using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject Transicao;

    void Awake()
    {
        // Garante que o menu de pause esteja desativado ao iniciar
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    void Update()
    {
        // Só permite pausar se não houver transição ativa
        if (Input.GetKeyDown(KeyCode.Escape) && (Transicao == null || Transicao.activeSelf == false))
        {
            Debug.Log("Tecla ESC pressionada"); // Adicionado para depuração

            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Debug.Log("Carregando Menu...");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Saindo do Jogo...");
        SceneManager.LoadScene("Menu");
    }

    public void LoadCredits()
    {
        Debug.Log("Carregando Créditos...");
        SceneManager.LoadScene("Creditos");
    }

    public void LoadOptions()
    {
        Debug.Log("Carregando Opções...");
        Time.timeScale = 1f;
    }
}
