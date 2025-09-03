using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nuvens : MonoBehaviour
{
    [Header("Referências das Imagens")]
    public Image nuvemPerto;
    public Image nuvemDistante;

    private Image nuvemPertoClone;
    private Image nuvemDistanteClone;

    [Header("Velocidades das Nuvens")]
    public float velocidadePerto;
    public float velocidadeDistante;

    void Start()
    {
        // Cria clones das nuvens
        if (nuvemPerto != null)
        {
            nuvemPertoClone = Instantiate(nuvemPerto, nuvemPerto.transform.parent);
            nuvemPertoClone.rectTransform.anchoredPosition = nuvemPerto.rectTransform.anchoredPosition + new Vector2(nuvemPerto.rectTransform.rect.width, 0);
            nuvemPertoClone.transform.SetSiblingIndex(nuvemPerto.transform.GetSiblingIndex()); // Garante que fique atrás
        }
        if (nuvemDistante != null)
        {
            nuvemDistanteClone = Instantiate(nuvemDistante, nuvemDistante.transform.parent);
            nuvemDistanteClone.rectTransform.anchoredPosition = nuvemDistante.rectTransform.anchoredPosition + new Vector2(nuvemDistante.rectTransform.rect.width, 0);
            nuvemDistanteClone.transform.SetSiblingIndex(nuvemDistante.transform.GetSiblingIndex());
        }
    }

    void Update()
    {
        MoverNuvem(nuvemPerto, nuvemPertoClone, velocidadePerto);
        MoverNuvem(nuvemDistante, nuvemDistanteClone, velocidadeDistante);
    }

    void MoverNuvem(Image img1, Image img2, float velocidade)
    {
        if (img1 == null || img2 == null) return;

        img1.rectTransform.anchoredPosition += Vector2.left * velocidade * Time.deltaTime;
        img2.rectTransform.anchoredPosition += Vector2.left * velocidade * Time.deltaTime;

        float largura = img1.rectTransform.rect.width;

        if (img1.rectTransform.anchoredPosition.x < -largura)
            img1.rectTransform.anchoredPosition = img2.rectTransform.anchoredPosition + new Vector2(largura, 0);

        if (img2.rectTransform.anchoredPosition.x < -largura)
            img2.rectTransform.anchoredPosition = img1.rectTransform.anchoredPosition + new Vector2(largura, 0);
    }
}
