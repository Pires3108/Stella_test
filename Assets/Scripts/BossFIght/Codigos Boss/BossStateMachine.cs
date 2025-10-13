using System;
using UnityEngine;

/// <summary>
/// Gerencia os estados do boss e suas transições
/// </summary>
public class BossStateMachine : MonoBehaviour
{
    [Header("Configurações de Estado")]
    public float stunDuration = 2f;
    
    // Estado atual
    private BossState currentState = BossState.Idle;
    private float stateTimer = 0f;
    
    // Eventos de mudança de estado
    public event Action<BossState, BossState> OnStateChanged;
    public event Action<BossState> OnStateEnter;
    public event Action<BossState> OnStateExit;
    
    // Propriedades públicas
    public BossState CurrentState => currentState;
    public float StateTimer => stateTimer;
    
    void Update()
    {
        stateTimer += Time.deltaTime;
        UpdateCurrentState();
    }
    
    /// <summary>
    /// Muda o estado do boss
    /// </summary>
    public void ChangeState(BossState newState)
    {
        if (currentState == newState) return;
        
        BossState previousState = currentState;
        
        // Debug.Log($"Boss: Mudando estado de {currentState} para {newState}");
        
        // Sai do estado atual
        OnStateExit?.Invoke(currentState);
        
        // Muda para o novo estado
        currentState = newState;
        stateTimer = 0f;
        
        // Entra no novo estado
        OnStateEnter?.Invoke(currentState);
        OnStateChanged?.Invoke(previousState, newState);
    }
    
    /// <summary>
    /// Força um estado específico (para controle externo)
    /// </summary>
    public void ForceState(BossState newState)
    {
        ChangeState(newState);
    }
    
    /// <summary>
    /// Atualiza o estado atual (transições automáticas)
    /// </summary>
    private void UpdateCurrentState()
    {
        switch (currentState)
        {
            case BossState.Stunned:
                if (stateTimer >= stunDuration)
                {
                    // Debug.Log($"Boss: Saindo do estado Stunned após {stateTimer:F2} segundos");
                    ChangeState(BossState.Idle);
                }
                break;
        }
    }
    
    /// <summary>
    /// Reseta o estado para Idle
    /// </summary>
    public void ResetState()
    {
        ChangeState(BossState.Idle);
    }
    
    /// <summary>
    /// Verifica se o boss está em um estado específico
    /// </summary>
    public bool IsInState(BossState state)
    {
        return currentState == state;
    }
    
    /// <summary>
    /// Verifica se o boss está em qualquer um dos estados fornecidos
    /// </summary>
    public bool IsInAnyState(params BossState[] states)
    {
        foreach (var state in states)
        {
            if (currentState == state) return true;
        }
        return false;
    }
}
