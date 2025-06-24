using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Personagem : MonoBehaviour
{

    [SerializeField]
    private Dialogo dialogo;

    private void OnMouseDown()
    {
        TelaDialogo.Instancia.Exibir(this.dialogo);
    }

}
