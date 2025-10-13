using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerFight : MonoBehaviour
{
    public Slider barraBoss;
    public Animator AnimBarraBoss;
    public BossFightSystem bossFightSystem; // Referência ao sistema do boss
    
    // Start is called before the first frame update
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Ativa a barra de vida
            barraBoss.gameObject.SetActive(true);
            
            // Configura a barra de vida com os valores corretos do boss
            if (bossFightSystem != null)
            {
                bossFightSystem.SetupHealthBar();
            }
            
            // Toca a animação da barra
            AnimBarraBoss.Play("BarraDeVidaBoss");
        }
    }
}
