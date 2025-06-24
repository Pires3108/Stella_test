using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class FalaDialogo 
{

    [SerializeField]
    private Ator ator;

    [SerializeField]
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
}