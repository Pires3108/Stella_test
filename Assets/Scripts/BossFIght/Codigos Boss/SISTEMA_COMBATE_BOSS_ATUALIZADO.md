# Sistema de Combate do Boss - Atualizado

## Correções Implementadas

### 1. ✅ Sistema de Flip do Sprite
- **Problema**: O boss não estava virando corretamente quando andava para a direita
- **Solução**: Corrigida a lógica na função `FlipSprite()` e adicionada tolerância (0.1f) para evitar flip constante
- **Resultado**: O boss agora vira corretamente quando anda para a direita (+X) e não fica "tremendo"

### 2. ✅ Sistema de Recuo/Afastamento
- **Problema**: O boss passava pelo player durante a fase de descanso
- **Solução**: Modificada a função `RestAndRetreat()` para calcular a direção de afastamento baseada na posição do player
- **Resultado**: O boss sempre se afasta do player quando está na fase de descanso

### 3. ✅ Sistema de Vida Integrado
- **Problema**: O boss não interagia com o sistema de dano do player (Stella)
- **Solução**: 
  - Modificada a função `DealDamageToPlayer()` para usar o componente `Damageable` do player
  - Adicionada a função `TakeDamageFromPlayer()` para o boss receber dano
  - Atualizado sistema de tags para usar Boss1, Boss2, Boss3, Boss4
  - Implementado sistema de knockback bidirecional
- **Resultado**: Sistema de vida totalmente integrado entre boss e player

### 4. ✅ Sistema de Dano com BoxColliders
- **Problema**: BoxColliders de ataque não causavam dano
- **Solução**: 
  - Criado script `BossAttackCollider.cs` para gerenciar colliders de ataque
  - Integrado com o `BossFightSystem` para ativar/desativar colliders por animação
  - Sistema de prevenção de múltiplos hits
  - Adicionado debug detalhado para identificar problemas de colisão
  - Verificação específica por tag "Player"
- **Resultado**: Sistema de dano funcional usando BoxColliders ativados por animação

## Como Usar

### Configuração do Boss
1. **BossFightSystem**: Configure as referências no Inspector
   - Player: Referência ao GameObject do player
   - Attack Colliders: Lista de colliders de ataque (opcional - será preenchida automaticamente)

2. **BossAttackCollider**: Adicione este script aos objetos filhos que representam as áreas de ataque
   - Configure o `attackName` para corresponder aos nomes dos ataques
   - O script será automaticamente encontrado e configurado

### Estrutura Recomendada
```
Boss (BossFightSystem)
├── Sprite (SpriteRenderer)
├── Collider (Collider2D principal)
├── AttackCollider_ComboGiro (BossAttackCollider)
├── AttackCollider_PunchRight (BossAttackCollider)
└── AttackCollider_PunchLeft (BossAttackCollider)
```

### Configuração das Animações
Para cada ataque, configure na animação do **Boss principal** (não dos colliders filhos):
1. **Ativação do Collider**: Chame `EnableAttackCollider()` no frame apropriado
2. **Desativação do Collider**: Chame `DisableAttackCollider()` no final do ataque

**Alternativa**: Use as funções com nome específico:
- `EnableAttackColliderByName("ComboGiro")` - para ativar collider específico
- `DisableAttackColliderByName("ComboGiro")` - para desativar collider específico

### Exemplo de Uso
```csharp
// Para causar dano ao boss (chamado pelos ataques do player)
BossFightSystem boss = GetComponent<BossFightSystem>();
boss.TakeDamageFromPlayer(10, Vector2.right * 3f);

// Para verificar estado do boss
if (boss.GetCurrentState() == BossState.Stunned)
{
    // Boss está atordoado
}
```

## Funcionalidades Adicionais

### Sistema de Knockback
- Player → Boss: Aplicado via `TakeDamageFromPlayer()`
- Boss → Player: Aplicado via `DealDamageToPlayer()`

### Sistema de Estados
- **Idle**: Aguarda player se aproximar
- **Chasing**: Persegue o player
- **Attacking**: Executa ataques
- **Resting**: Descansa e se afasta
- **Stunned**: Atordoado após receber dano

### Sistema de Combo
- Sequência de ataques configurável
- Combo máximo configurável
- Recuo após completar combo

### Debug
- Logs detalhados para cada ação
- Gizmos visuais para ranges de ataque, perseguição e descanso
- Sistema de debug integrado

## Notas Importantes

1. **Colliders de Ataque**: Devem ser filhos do boss e ter o script `BossAttackCollider`
2. **Animações**: Devem chamar as funções de ativar/desativar colliders
3. **Player**: Deve ter o componente `Damageable` para funcionar corretamente
4. **Rigidbody2D**: Necessário para o sistema de knockback funcionar

## Troubleshooting

### Boss não vira corretamente
- Verifique se o `SpriteRenderer` está atribuído corretamente
- Confirme se a lógica de flip está funcionando (debug logs)

### Boss não se afasta do player
- Verifique se o `restDistance` está configurado adequadamente
- Confirme se o player está sendo detectado corretamente

### Colliders de ataque não funcionam
- Verifique se os objetos têm o script `BossAttackCollider`
- Confirme se as animações estão chamando as funções corretas
- Verifique se os `attackName` correspondem aos nomes dos ataques

### Sistema de vida não funciona
- Verifique se o player tem o componente `Damageable`
- Confirme se as referências estão atribuídas corretamente
