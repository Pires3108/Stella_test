using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class GameController : MonoBehaviour
{
    public PlayerController playerscript;
    public PlayerInput playerInput;
    public GameObject BarraDeVida;
    public GameObject BarradeEstamina;
    public GameObject BarraBoss;
    public GameObject Dialogo;
    public GameObject BossFight;
    public String CenaAtual;

    public bool IsInFight;


    public bool IsDialogActive;

    void Awake()
    {
        if(BarraBoss != null)
        {
            BarraBoss.SetActive(false);
        }
        if(BarraDeVida != null)
        {
            BarraDeVida.SetActive(false);
        }
        
        IsInFight = false;
    }
    void Update()
    {
        if (IsDialogActive)
        {
            BarraDeVida.SetActive(false);
            playerInput.enabled = false;
        }
        else if (!IsInFight)
        {
            BarraDeVida.SetActive(true);
            playerInput.enabled = true;
        }

    }

}
