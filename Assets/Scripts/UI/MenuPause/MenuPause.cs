using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject Transicao;
    public GameObject HUDS;

    public GameObject OpcoesMenuUI;

    void Awake()
    {
        // Garante que o menu de pause esteja desativado ao iniciar
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
        OpcoesMenuUI.SetActive(false);
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
        HUDS.SetActive(true);

        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
        HUDS.SetActive(false);

        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void Opcoes()
    {
        OpcoesMenuUI.SetActive(true);
    }
}
