using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    [Header("Canvas")]
    public Canvas gameCanvas;

    [Header("Dano e Cura")]
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;

/*    [Header("PUP´s")]
    public GameObject staminaAddedTextPrefab;
    public GameObject healthAddedTextPrefab;
    public GameObject damageAddedTextPrefab;

    [Header("Imports Codes")]
    public Damageable damageable;
    public PlayerController playerController;

    [Header("GameObjects")]
    public GameObject Pup;
*/


    void Awake()
    {
        if (gameCanvas == null)
        {
            gameCanvas = GameObject.Find("TextoDano").GetComponent<Canvas>();
        }
        else
        {
            gameCanvas = GameObject.Find("SpawnTextos").GetComponent<Canvas>();
        }
    }



    private void OnEnable()
    {
        CharacterEvents.characterDamaged.AddListener(CharacterTookDamage);
        CharacterEvents.characterHealed.AddListener(CharacterHealed);

        /*
        CharacterEvents.staminaAdded.AddListener(StaminaAdded);
        CharacterEvents.healthAdded.AddListener(HealthAdded);
        CharacterEvents.damageAdded.AddListener(DamageAdded);

        */
    }

    private void OnDisable()
    {
        CharacterEvents.characterDamaged.RemoveListener(CharacterTookDamage);
        CharacterEvents.characterHealed.RemoveListener(CharacterHealed);

        /*
        CharacterEvents.staminaAdded.RemoveListener(StaminaAdded);
        CharacterEvents.healthAdded.RemoveListener(HealthAdded);
        CharacterEvents.damageAdded.RemoveListener(DamageAdded);
        */
    }

    void CharacterTookDamage(GameObject character, int damageReceived)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        Vector3 offset = new Vector3(0, 150f, 0); // 30 pixels up

        TMP_Text tmp_Text = Instantiate(
            damageTextPrefab,
            spawnPosition + offset,
            Quaternion.identity,
            gameCanvas.transform
        ).GetComponent<TMP_Text>();

        tmp_Text.text = damageReceived.ToString();
        Destroy(tmp_Text.gameObject, 1.0f); // Destroy after 1 second
    }

    void CharacterHealed(GameObject character, int healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        Vector3 offset = new Vector3(0, 30f, 0); // 30 pixels up

        TMP_Text tmp_Text = Instantiate(
            healthTextPrefab,
            spawnPosition + offset,
            Quaternion.identity,
            gameCanvas.transform
        ).GetComponent<TMP_Text>();

        tmp_Text.text = healthRestored.ToString();
        Destroy(tmp_Text.gameObject, 1.0f); // Destroy after 1 second
    }

    /*
    void StaminaAdded(GameObject character, int staminaAdded)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        Vector3 offset = new Vector3(0, 30f, 0); // 30 pixels up

        TMP_Text tmp_Text = Instantiate(
            staminaAddedTextPrefab,
            spawnPosition + offset,
            Quaternion.identity,
            gameCanvas.transform
        ).GetComponent<TMP_Text>();

        tmp_Text.text = staminaAdded.ToString();
        Destroy(tmp_Text.gameObject, 1.0f); // Destroy after 1 second
    }

    void HealthAdded(GameObject character, int healthAdded)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        Vector3 offset = new Vector3(0, 30f, 0); // 30 pixels up

        TMP_Text tmp_Text = Instantiate(
            healthAddedTextPrefab,
            spawnPosition + offset,
            Quaternion.identity,
            gameCanvas.transform
        ).GetComponent<TMP_Text>();

        tmp_Text.text = healthAdded.ToString();
        Destroy(tmp_Text.gameObject, 1.0f); // Destroy after 1 second
    }

    void DamageAdded(GameObject character, int damageAdded)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        Vector3 offset = new Vector3(0, 30f, 0); // 30 pixels up

        TMP_Text tmp_Text = Instantiate(
            damageAddedTextPrefab,
            spawnPosition + offset,
            Quaternion.identity,
            gameCanvas.transform
        ).GetComponent<TMP_Text>();

        tmp_Text.text = damageAdded.ToString();
        Destroy(tmp_Text.gameObject, 1.0f); // Destroy after 1 second
    } 
    */
}