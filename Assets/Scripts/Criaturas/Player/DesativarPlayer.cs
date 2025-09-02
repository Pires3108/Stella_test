using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DesativarPlayer : MonoBehaviour
{
    public PlayerController estellaController;
    public GameObject stella;
    public GameObject Transicao;
    public Animator playerAnimator;
    void Start()
    {
        stella.transform.rotation = Quaternion.Euler(0, 0, 0);
        estellaController._isFacingRight = true;
        estellaController.canFlip = false;
        estellaController.enabled = false;
        playerAnimator.enabled = false;
    }

    void AtivarPlayer()
    {
        Transicao.SetActive(false);
        estellaController.canFlip = true;
        estellaController.enabled = true;
        playerAnimator.enabled = true;
    }
}
