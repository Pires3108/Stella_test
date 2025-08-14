using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtivarBarra : MonoBehaviour
{
    public GameObject BarraDeVida;
    // Start is called before the first frame update
    public void AtivarBarraVida()
    {
        BarraDeVida.SetActive(true);
    }
}
