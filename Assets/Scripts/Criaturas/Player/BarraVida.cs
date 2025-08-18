using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Slider slider;
    public Slider sliderEstamina; // Adicione no Inspector
    Damageable player;
    Estamina StellaEstamina;
    // Start is called before the first frame update
    void Start()
    {
        //integrando ao codigo os valores do codigo Damageable
        player = GameObject.Find("Stella").GetComponent<Damageable>();
        StellaEstamina = GameObject.Find("Stella").GetComponent<Estamina>();
    }

    // Update is called once per frame
    void Update()
    {
        //igualando o valor do slider ao valor da vida
        slider.value = player.Health;
        sliderEstamina.value = StellaEstamina.Energy;
    }
}
