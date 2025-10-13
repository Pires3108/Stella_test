using System.Collections;
using UnityEngine;

/// <summary>
/// Gerencia o movimento, perseguição e recuo do boss
/// </summary>
public class BossMovement : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    public float chaseSpeed = 3f;
    public float attackDistance = 2f;
    public float maxChaseDistance = 10f;
    public float restDistance = 5f;
    public float restDuration = 3f;
    public float retreatDistance = 3f;
    public float retreatSpeed = 2f;
    
    [Header("Referências")]
    public GameObject player;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    
    // Estado interno
    private bool isFacingRight = true;
    private Vector2 startPosition;
    private bool isInterrupted = false;
    
    // Eventos
    public event System.Action OnChaseStarted;
    public event System.Action OnChaseEnded;
    public event System.Action OnRetreatStarted;
    public event System.Action OnRetreatEnded;
    public event System.Action OnRestStarted;
    public event System.Action OnRestEnded;
    
    // Propriedades públicas
    public bool IsFacingRight => isFacingRight;
    public bool IsInterrupted => isInterrupted;
    
    void Start()
    {
        startPosition = transform.position;
    }
    
    /// <summary>
    /// Verifica se o player está dentro do alcance de perseguição
    /// </summary>
    public bool IsPlayerInChaseRange()
    {
        if (player == null) return false;
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance <= maxChaseDistance;
    }
    
    /// <summary>
    /// Verifica se o player está dentro do alcance de ataque
    /// </summary>
    public bool IsPlayerInAttackRange()
    {
        if (player == null) return false;
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance <= attackDistance;
    }
    
    /// <summary>
    /// Verifica se o boss está na distância de descanso
    /// </summary>
    public bool IsAtRestDistance()
    {
        if (player == null) return true;
        float distance = Vector2.Distance(transform.position, player.transform.position);
        return distance >= restDistance;
    }
    
    /// <summary>
    /// Inicia a perseguição do player
    /// </summary>
    public IEnumerator ChasePlayer()
    {
        if (animator != null)
            animator.SetBool("isWalking", true);
        
        OnChaseStarted?.Invoke();
        
        float chaseTime = 0f;
        float maxChaseTime = 10f;
        
        // Debug.Log($"Boss: Iniciando perseguição. Distância atual: {Vector2.Distance(transform.position, player.transform.position)}");
        
        while (chaseTime < maxChaseTime && !isInterrupted)
        {
            if (player == null) break;
            
            Vector2 direction = (player.transform.position - transform.position).normalized;
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
            
            // Para de perseguir se chegou perto o suficiente
            if (distanceToPlayer <= attackDistance)
            {
                // Debug.Log("Boss: Chegou perto o suficiente para atacar");
                break;
            }
            
            // Para de perseguir se o player fugiu muito
            if (distanceToPlayer > maxChaseDistance)
            {
                // Debug.Log("Boss: Player fugiu muito, parando perseguição");
                break;
            }
            
            // Flip sprite baseado na direção
            UpdateFacingDirection(direction);
            
            // Move em direção ao player
            MoveTowards(direction, chaseSpeed);
            
            chaseTime += Time.deltaTime;
            yield return null;
        }
        
        if (animator != null)
            animator.SetBool("isWalking", false);
        
        OnChaseEnded?.Invoke();
        // Debug.Log($"Boss: Perseguição terminada após {chaseTime:F2} segundos");
    }
    
    /// <summary>
    /// Recua após um ataque
    /// </summary>
    public IEnumerator RetreatAfterAttack()
    {
        // Debug.Log("Boss: Recuando após ataque");
        
        isInterrupted = false;
        OnRetreatStarted?.Invoke();
        
        if (animator != null)
            animator.SetBool("isWalking", true);
        
        float retreatTime = 0f;
        float maxRetreatTime = 2f;
        
        // Calcula direção de recuo
        Vector2 retreatDirection = CalculateRetreatDirection();
        
        while (retreatTime < maxRetreatTime && !isInterrupted)
        {
            // Vira o boss na direção que está se movendo
            UpdateFacingDirection(retreatDirection);
            
            // Move na direção de recuo
            MoveTowards(retreatDirection, retreatSpeed);
            
            retreatTime += Time.deltaTime;
            yield return null;
        }
        
        if (animator != null)
            animator.SetBool("isWalking", false);
        
        OnRetreatEnded?.Invoke();
        
        if (isInterrupted)
        {
            // Debug.Log("Boss: Recuo interrompido por dano!");
        }
        else
        {
            // Debug.Log($"Boss: Recuo terminado após {retreatTime:F2} segundos");
        }
    }
    
    /// <summary>
    /// Descansa e se afasta do player
    /// </summary>
    public IEnumerator RestAndRetreat()
    {
        // Debug.Log("Boss: Iniciando descanso e afastamento");
        
        isInterrupted = false;
        OnRestStarted?.Invoke();
        
        if (animator != null)
            animator.SetBool("isWalking", true);
        
        float retreatTime = 0f;
        float maxRetreatTime = 3f;
        
        // Afasta até atingir a distância de descanso
        while (retreatTime < maxRetreatTime && !isInterrupted)
        {
            if (player != null && !IsAtRestDistance())
            {
                Vector2 directionAwayFromPlayer = (transform.position - player.transform.position).normalized;
                UpdateFacingDirection(directionAwayFromPlayer);
                MoveTowards(directionAwayFromPlayer, retreatSpeed);
            }
            
            retreatTime += Time.deltaTime;
            yield return null;
        }
        
        if (animator != null)
            animator.SetBool("isWalking", false);
        
        if (isInterrupted)
        {
            // Debug.Log("Boss: Afastamento interrompido por dano! Pulando tempo de descanso...");
            OnRestEnded?.Invoke();
            yield break;
        }
        
        // Debug.Log($"Boss: Afastamento terminado após {retreatTime:F2} segundos");
        
        // Aguarda o tempo de descanso
        // Debug.Log($"Boss: Aguardando {restDuration} segundos de descanso");
        yield return new WaitForSeconds(restDuration);
        
        OnRestEnded?.Invoke();
        // Debug.Log("Boss: Descanso terminado");
    }
    
    /// <summary>
    /// Move o boss em uma direção específica
    /// </summary>
    private void MoveTowards(Vector2 direction, float speed)
    {
        if (rb != null)
        {
            Vector2 newPosition = new Vector2(
                rb.position.x + direction.x * speed * Time.fixedDeltaTime,
                rb.position.y
            );
            rb.MovePosition(newPosition);
        }
    }
    
    /// <summary>
    /// Atualiza a direção que o boss está olhando
    /// </summary>
    private void UpdateFacingDirection(Vector2 direction)
    {
        if (direction.x > 0.1f && !isFacingRight)
        {
            FlipSprite(true);
        }
        else if (direction.x < -0.1f && isFacingRight)
        {
            FlipSprite(false);
        }
    }
    
    /// <summary>
    /// Calcula a direção de recuo baseada na posição do player
    /// </summary>
    private Vector2 CalculateRetreatDirection()
    {
        if (player != null)
        {
            return (transform.position - player.transform.position).normalized;
        }
        else
        {
            // Fallback: recua na direção oposta ao que está olhando
            return isFacingRight ? Vector2.left : Vector2.right;
        }
    }
    
    /// <summary>
    /// Vira o sprite do boss usando scale negativa
    /// </summary>
    public void FlipSprite(bool faceRight)
    {
        isFacingRight = faceRight;
        if (spriteRenderer != null)
        {
            // Usa scale negativa ao invés de flipX
            Vector3 scale = spriteRenderer.transform.localScale;
            scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            spriteRenderer.transform.localScale = scale;
        }
    }
    
    /// <summary>
    /// Interrompe o movimento atual
    /// </summary>
    public void InterruptMovement()
    {
        isInterrupted = true;
        // Debug.Log("Boss: Movimento interrompido por dano!");
    }
    
    /// <summary>
    /// Reseta o movimento para o estado inicial
    /// </summary>
    public void ResetMovement()
    {
        isInterrupted = false;
        transform.position = startPosition;
        
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
        }
    }
    
    /// <summary>
    /// Aplica configurações de um preset
    /// </summary>
    public void ApplyPreset(BossPreset preset)
    {
        chaseSpeed = preset.chaseSpeed;
        attackDistance = preset.attackDistance;
        maxChaseDistance = preset.maxChaseDistance;
        restDistance = preset.restDistance;
        restDuration = preset.restDuration;
        retreatDistance = preset.retreatDistance;
        retreatSpeed = preset.retreatSpeed;
    }
}
