using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PUP : MonoBehaviour
{
    [Header("PUP Imports")]
    public GameObject pup;
    public Animator anim;
    public CircleCollider2D col;

    [Header("Imports")]
    public GameObject player;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Stella");
        pup = this.gameObject;
    }

    public void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            delayDestroy();
        }
    }

    IEnumerator delayDestroy()
    {
        anim.Play("destroy");
        yield return new WaitForSeconds(0.1f);
        Destroy(pup);
    }
}
