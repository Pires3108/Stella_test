using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Novo diálogo", menuName = "Sistema de diálogo/Novo diálogo")]
public class Dialogo : ScriptableObject
{

    [SerializeField]
    private FalaDialogo[] falasDialogo;

    private int indiceFalaAtual;



    public FalaDialogo FalaAtual
    {
        get
        {
            if (this.indiceFalaAtual < this.falasDialogo.Length)
            {
                return this.falasDialogo[this.indiceFalaAtual];
            }

            return null;
        }
    }

    public void Iniciar()
    {
        this.indiceFalaAtual = 0;
    }

    public void AvancarFala()
    {
        if (TemProximaFala())
        {
            this.indiceFalaAtual++;
        }

    }

    public bool TemProximaFala()
    {
        if (this.indiceFalaAtual < (this.falasDialogo.Length - 1))
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
