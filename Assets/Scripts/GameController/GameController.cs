using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public PlayerController playerscript;
    public PlayerInput playerInput;
    public GameObject BarraDeVida;
    public GameObject Dialogo;


    public bool IsDialogActive;

    void Update()
    {
        if (IsDialogActive)
        {
            BarraDeVida.SetActive(false);
            playerInput.enabled = false;
        }
        else
        {
            BarraDeVida.SetActive(true);
            playerInput.enabled = true;
        }
    }
   
}
