using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Slider slider;
    Damageable player;
    // Start is called before the first frame update
    void Start()
    {
        //integrando ao codigo os valores do codigo Damageable
        player = FindObjectOfType<Damageable>();
    }

    // Update is called once per frame
    void Update()
    {   
        //igualando o valor do slider ao valor da vida
        slider.value = player.Health;
    }
}
