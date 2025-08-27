using UnityEngine;

public class BossTest : MonoBehaviour
{
    [Header("Teste do Sistema de Boss")]
    public BossFightSystem bossFightSystem;
    public KeyCode testDamageKey = KeyCode.T;
    public KeyCode testHealKey = KeyCode.Y;
    public KeyCode testResetKey = KeyCode.R;

    void Update()
    {
        if (bossFightSystem == null) return;

        // Teste de dano
        if (Input.GetKeyDown(testDamageKey))
        {
            bossFightSystem.TakeDamage(10f);
            Debug.Log("Teste: Causou 10 de dano ao boss");
        }

        // Teste de cura
        if (Input.GetKeyDown(testHealKey))
        {
            bossFightSystem.Heal(20f);
            Debug.Log("Teste: Curou 20 de vida do boss");
        }

        // Teste de reset
        if (Input.GetKeyDown(testResetKey))
        {
            bossFightSystem.ResetBoss();
            Debug.Log("Teste: Boss resetado");
        }

        // Mostra informações do boss
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log($"Boss Info - Vida: {bossFightSystem.GetHealthPercentage():P0}, Estado: {bossFightSystem.GetCurrentState()}");
        }
    }

    void OnGUI()
    {
        if (bossFightSystem == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("=== Teste do Sistema de Boss ===");
        GUILayout.Label($"Vida: {bossFightSystem.GetHealthPercentage():P0}");
        GUILayout.Label($"Estado: {bossFightSystem.GetCurrentState()}");
        GUILayout.Space(10);
        GUILayout.Label("Controles:");
        GUILayout.Label($"{testDamageKey} - Causar Dano (10)");
        GUILayout.Label($"{testHealKey} - Curar (20)");
        GUILayout.Label($"{testResetKey} - Resetar Boss");
        GUILayout.Label("I - Mostrar Info");
        GUILayout.EndArea();
    }
}
