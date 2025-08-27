using UnityEngine;

public class BossDebugger : MonoBehaviour
{
    [Header("Debug do Boss")]
    public BossFightSystem bossFightSystem;
    public bool showDebugInfo = true;
    public bool logStateChanges = true;
    public bool logDistance = true;
    
    [Header("Configurações de Debug")]
    public float debugUpdateInterval = 0.5f;
    private float lastDebugUpdate = 0f;

    void Update()
    {
        if (!showDebugInfo || bossFightSystem == null) return;

        lastDebugUpdate += Time.deltaTime;
        if (lastDebugUpdate >= debugUpdateInterval)
        {
            lastDebugUpdate = 0f;
            UpdateDebugInfo();
        }
    }

    void UpdateDebugInfo()
    {
        if (bossFightSystem.player == null)
        {
            Debug.LogError("BossDebugger: Player não encontrado no BossFightSystem!");
            return;
        }

        float distanceToPlayer = Vector2.Distance(bossFightSystem.transform.position, bossFightSystem.player.transform.position);
        
        if (logDistance)
        {
            Debug.Log($"BossDebugger: Distância ao player: {distanceToPlayer:F2} | " +
                     $"Attack Distance: {bossFightSystem.attackDistance} | " +
                     $"Max Chase Distance: {bossFightSystem.maxChaseDistance} | " +
                     $"Estado: {bossFightSystem.GetCurrentState()}");
        }

        // Verifica se as configurações estão corretas
        if (distanceToPlayer <= bossFightSystem.maxChaseDistance && 
            bossFightSystem.GetCurrentState() == BossState.Idle)
        {
            Debug.LogWarning("BossDebugger: Player está no range mas boss está idle!");
        }

        if (distanceToPlayer <= bossFightSystem.attackDistance && 
            bossFightSystem.GetCurrentState() == BossState.Chasing)
        {
            Debug.LogWarning("BossDebugger: Player está no range de ataque mas boss ainda está perseguindo!");
        }
    }

    void OnGUI()
    {
        if (!showDebugInfo || bossFightSystem == null) return;

        GUILayout.BeginArea(new Rect(Screen.width - 350, 10, 340, 200));
        GUILayout.Label("=== Debug do Boss ===");
        
        if (bossFightSystem.player != null)
        {
            float distance = Vector2.Distance(bossFightSystem.transform.position, bossFightSystem.player.transform.position);
            GUILayout.Label($"Distância ao Player: {distance:F2}");
            GUILayout.Label($"Attack Distance: {bossFightSystem.attackDistance}");
            GUILayout.Label($"Max Chase Distance: {bossFightSystem.maxChaseDistance}");
        }
        
        GUILayout.Label($"Estado: {bossFightSystem.GetCurrentState()}");
        GUILayout.Label($"Vida: {bossFightSystem.GetHealthPercentage():P0}");
        GUILayout.Label($"Velocidade: {bossFightSystem.chaseSpeed}");
        GUILayout.Label($"Ataques: {bossFightSystem.availableAttacks.Count}");
        GUILayout.Label($"Próximo Ataque: {bossFightSystem.GetNextAttackName()}");
        GUILayout.Label($"Sequência: {bossFightSystem.GetCurrentAttackIndex() + 1}/{bossFightSystem.GetTotalAttacks()}");
        
        GUILayout.EndArea();
    }

    // Método para testar o sistema
    [ContextMenu("Testar Sistema do Boss")]
    public void TestBossSystem()
    {
        if (bossFightSystem == null)
        {
            Debug.LogError("BossDebugger: BossFightSystem não configurado!");
            return;
        }

        Debug.Log("=== Teste do Sistema do Boss ===");
        Debug.Log($"Player configurado: {bossFightSystem.player != null}");
        Debug.Log($"Animator configurado: {bossFightSystem.animator != null}");
        Debug.Log($"Rigidbody2D configurado: {bossFightSystem.rb != null}");
        Debug.Log($"SpriteRenderer configurado: {bossFightSystem.spriteRenderer != null}");
        Debug.Log($"Ataques configurados: {bossFightSystem.availableAttacks.Count}");
        Debug.Log($"Estado atual: {bossFightSystem.GetCurrentState()}");
        
        if (bossFightSystem.player != null)
        {
            float distance = Vector2.Distance(bossFightSystem.transform.position, bossFightSystem.player.transform.position);
            Debug.Log($"Distância ao player: {distance:F2}");
            Debug.Log($"Dentro do range de perseguição: {distance <= bossFightSystem.maxChaseDistance}");
            Debug.Log($"Dentro do range de ataque: {distance <= bossFightSystem.attackDistance}");
        }
    }
}
