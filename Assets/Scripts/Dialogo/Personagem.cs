using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personagem : MonoBehaviour
{

    [SerializeField]
    private Dialogo dialogo;
    public bool podeInteragir;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && podeInteragir)
        {
            TelaDialogo.Instancia.Exibir(this.dialogo);
        }   
    }

}
