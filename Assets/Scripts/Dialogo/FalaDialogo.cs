using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class FalaDialogo
{
    [SerializeField , HideInInspector]
    private string identificador;

    [SerializeField]
    private Ator ator;

    [SerializeField, TextArea(1, 5)]
    private string texto;

    public Ator Ator
    {
        get
        {
            return this.ator;
        }
    }

    public string Texto
    {
        get
        {
            return this.texto;
        }
    }

    public void AtualizarIdentificador()
    {
        if ((this.ator != null) && (this.texto != null))
        {
            this.identificador = "[" + this.ator.Nome + "] : " + this.texto;
        }
    }
}