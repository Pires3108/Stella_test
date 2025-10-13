using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Estamina : MonoBehaviour
{

    public float Energy = 100f; // Valor inicial de energia
    public float MaxEnergy = 100f; // Energia máxima
    
    /// <summary>
    /// Aumenta a energia máxima e restaura a energia atual ao máximo
    /// </summary>
    /// <param name="increaseAmount">Quantidade a ser aumentada</param>
    public void IncreaseMaxEnergy(float increaseAmount)
    {
        MaxEnergy += increaseAmount;
        Energy = MaxEnergy; // Restaura energia ao máximo
        Debug.Log($"Energia máxima aumentada para {MaxEnergy}");
    }
    void Start()
    {
       Energy = MaxEnergy; // Inicializa a energia com o valor máximo 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
