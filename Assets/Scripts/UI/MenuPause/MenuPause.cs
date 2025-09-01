using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{

    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressionado!"); // Adiciona este log

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

    // Torna Resume público para garantir acesso externo se necessário
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Debug.Log("Carregando Menu...");
        Time.timeScale = 1f;
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
