using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Novo ator", menuName = "Sistema de diálogo/Novo ator")]
public class Ator : ScriptableObject
{
    [SerializeField]
    private string nome;

    [SerializeField]
    private Sprite foto;

    public string Nome
    {
        get 
        {
            return this.nome;
        }
    }

    public Sprite Foto{
        get
        {
            return this.foto;
        }
    }
}
