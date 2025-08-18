using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Estamina : MonoBehaviour
{

    public float Energy = 100f; // Valor inicial de energia
    public float MaxEnergy = 100f; // Energia máxima
    void Start()
    {
       Energy = MaxEnergy; // Inicializa a energia com o valor máximo 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
