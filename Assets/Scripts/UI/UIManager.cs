using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;
    public Canvas gameCanvas;

    void Awake()
    {
        gameCanvas = FindObjectOfType<Canvas>();
        
    }

    private void OnEnable()
    {
        CharacterEvents.characterDamaged.AddListener(CharacterTookDamage);
        CharacterEvents.characterHealed.AddListener(CharacterHealed);
    }

    private void OnDisable()
    {
        CharacterEvents.characterDamaged.RemoveListener(CharacterTookDamage);
        CharacterEvents.characterHealed.RemoveListener(CharacterHealed);
    }


    void CharacterTookDamage(GameObject character, int damageReceived)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmp_Text = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
            .GetComponent<TMP_Text>();

        tmp_Text.text = damageReceived.ToString();
    }

    void CharacterHealed(GameObject character, int healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmp_Text = Instantiate( healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform)
            .GetComponent<TMP_Text>();

        tmp_Text.text = healthRestored.ToString();
    }
}
