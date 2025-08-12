using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxCamada : MonoBehaviour
{
    [Header("Referencia da câmera")]

    public Transform cameraTransform;
    [Header("Velocidade do parallax")]
    [Range(0f, 1f)] public float velocidadeX = 0.2f;
    [Range(0f, 1f)] public float velocidadeY = 0.1f;
    [Header("Movimento Vertical?")]
    public bool moverNoEixoY = false;
    private Material materialInstaciado;
    private Vector2 offsetAtual;
    private Vector3 ultimaPosicaoCamera;

    // Start is called before the first frame update
    void Start()
    {
        if (cameraTransform == null)

        cameraTransform = Camera.main.transform;

        materialInstaciado = new Material(GetComponent<Renderer>().material);

        GetComponent<Renderer>().material = materialInstaciado;

        offsetAtual = Vector2.zero;

        ultimaPosicaoCamera = cameraTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = cameraTransform.position - ultimaPosicaoCamera;

    ultimaPosicaoCamera = cameraTransform.position;

    float offsetX = delta.x * velocidadeX;

    float offsetY = moverNoEixoY ? delta.y * velocidadeY : 0f;

    offsetAtual += new Vector2(offsetX, offsetY);

    materialInstaciado.mainTextureOffset = offsetAtual;

    // Move o quad junto com a câmera (Y opcional)

    float posY = moverNoEixoY ? cameraTransform.position.y : transform.position.y;

    transform.position = new Vector3(cameraTransform.position.x, posY, transform.position.z);
    }
}
