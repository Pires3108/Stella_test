# Sistema de Luta de Boss - Unity

Este sistema fornece uma solução completa e reutilizável para criar bosses em Unity, com IA inteligente, sistema de estados e configuração flexível.

## Características Principais

### 🎯 **Sistema de Estados**
- **Idle**: Boss aguarda o player se aproximar
- **Chasing**: Persegue o player até chegar perto
- **Attacking**: Executa ataques quando próximo (para de se mover)
- **Retreating**: Recua após o ataque
- **Resting**: Descansa após ataques (animação de cansaço)
- **Stunned**: Fica atordoado quando toma dano

### ⚔️ **Sistema de Ataques Configurável**
- Ataques em sequência (ordem da lista)
- Duração e cooldown personalizáveis
- Range de ataque configurável
- Sistema de combos

### 🎮 **Configuração via Inspector**
- Presets de bosses pré-definidos
- Configuração manual de todos os parâmetros
- Sistema de eventos para integração

## Como Usar

### 1. Configuração Básica

1. **Adicione o script `BossFightSystem` ao GameObject do boss**
2. **Configure as referências no Inspector:**
   - `Player`: GameObject do jogador
   - `Animator`: Componente Animator do boss
   - `Rigidbody2D`: Componente Rigidbody2D do boss
   - `SpriteRenderer`: Componente SpriteRenderer do boss

### 2. Configuração de Ataques

#### Opção A: Usando Presets
1. Crie um `BossPreset` (Assets > Create > Boss Fight > Boss Preset)
2. Configure os ataques no preset
3. Arraste o preset para o campo `Boss Preset` no `BossFightSystem`

#### Opção B: Configuração Manual
1. Configure os ataques diretamente na lista `Available Attacks`
2. Para cada ataque, defina:
   - `Attack Name`: Nome do ataque
   - `Animation Trigger`: Trigger da animação
   - `Attack Duration`: Duração da animação
   - `Damage`: Dano causado
   - `Range`: Alcance do ataque
   - **Nota**: Os ataques serão executados em sequência, na ordem da lista

### 3. Animações Necessárias

O sistema espera as seguintes animações/triggers:

**Estados:**
- `isWalking` (bool)
- `isIdle` (bool)
- `isResting` (bool)

**Ataques:**
- Triggers personalizados para cada ataque (ex: "ComboGiro", "PunchRight")
- **Importante**: Os triggers são automaticamente resetados após cada ataque

**Especiais:**
- `Stunned` (trigger)
- `Death` (trigger)

### 4. Comportamento de Combate

O boss agora segue um ciclo mais realista:
1. **Persegue** o player até chegar perto
2. **Para de se mover** durante o ataque
3. **Executa o ataque** (em sequência da lista)
4. **Recua** após o ataque
5. **Se afasta** para a distância de descanso
6. **Descansa** antes do próximo ciclo

### 4. Exemplo de Configuração

```csharp
// Exemplo de configuração via código
BossFightSystem boss = GetComponent<BossFightSystem>();

// Adicionar ataque
BossAttack newAttack = new BossAttack
{
    attackName = "SuperPunch",
    animationTrigger = "SuperPunch",
    attackDuration = 2f,
    damage = 25f,
    range = 3f,
    weight = 0.5f
};
boss.AddAttack(newAttack);

// Conectar eventos
boss.OnBossDeath += () => Debug.Log("Boss derrotado!");
boss.OnHealthChanged += (health) => UpdateHealthBar(health);
```

## Presets Disponíveis

### Lagartixa Boss
- **Vida**: 120
- **Velocidade**: 3.5
- **Ataques**: ComboGiro, PunchRight, PunchLeft
- **Comportamento**: Agressivo

### Tank Boss
- **Vida**: 200
- **Velocidade**: 2.0
- **Ataques**: HeavySlam, GroundPound
- **Comportamento**: Defensivo

### Agile Boss
- **Vida**: 80
- **Velocidade**: 5.0
- **Ataques**: QuickStrike, DashAttack, ComboKick
- **Comportamento**: Muito agressivo

## Eventos Disponíveis

```csharp
// Eventos que você pode conectar
boss.OnAttackStart += (attack) => Debug.Log($"Iniciou: {attack.attackName}");
boss.OnAttackEnd += (attack) => Debug.Log($"Terminou: {attack.attackName}");
boss.OnBossDeath += () => Debug.Log("Boss morreu!");
boss.OnHealthChanged += (healthPercentage) => UpdateUI(healthPercentage);
```

## Métodos Públicos

```csharp
// Controle de vida
boss.TakeDamage(10f);
boss.Heal(20f);
boss.SetHealth(50f);

// Controle de estado
boss.ForceState(BossState.Stunned);
boss.SetAggressiveMode(true);

// Informações
float health = boss.GetHealthPercentage();
BossState state = boss.GetCurrentState();
string nextAttack = boss.GetNextAttackName();
int attackIndex = boss.GetCurrentAttackIndex();

// Reset
boss.ResetBoss();
```

## Debug e Visualização

O sistema inclui gizmos para debug:
- **Vermelho**: Range de ataque
- **Amarelo**: Range de perseguição
- **Verde**: Range de descanso (distância mínima)
- **Azul**: Direção do boss

## Dicas de Uso

1. **Arena**: Configure limites para evitar que o boss saia da área de combate
2. **Animações**: Certifique-se de que todas as animações estão configuradas corretamente
3. **Performance**: O sistema usa coroutines para eficiência
4. **Reutilização**: Use presets para criar diferentes tipos de bosses rapidamente

## Exemplo Completo

Veja o script `BossFightExample.cs` para um exemplo completo de como integrar o sistema com UI e controles de teste.

## Solução de Problemas

### Erros de Compilação Comuns

1. **Erro CS0111: Type 'BossFightSystem' already defines a member called 'SetAggressiveMode'**
   - **Solução**: Este erro foi corrigido na versão atual. Se persistir, verifique se não há métodos duplicados no script.

2. **Warnings CS0436: Type conflicts with Unity.VisualScripting**
   - **Solução**: As classes foram renomeadas para evitar conflitos. Use o namespace `GameStateMachine` em vez de `Unity.VisualScripting`.

3. **Erro de referência nula no Animator**
   - **Solução**: Certifique-se de que o campo `Animator` está configurado no Inspector.

4. **'BossAttack' is missing the class attribute 'ExtensionOfNativeClass'**
   - **Solução**: A classe `BossAttack` foi movida para um arquivo separado (`BossAttack.cs`) para evitar conflitos de serialização.

5. **Boss não persegue o player**
   - **Solução**: Adicionadas verificações de segurança e logs de debug. Use o `BossDebugger` para identificar problemas.

6. **Unity trava durante o jogo**
   - **Solução**: Adicionadas verificações de null reference e proteções contra loops infinitos.

### Estrutura de Arquivos

O sistema agora está organizado em arquivos separados:
- `BossFightSystem.cs` - Sistema principal de luta
- `BossAttack.cs` - Classe de ataques
- `BossState.cs` - Enum de estados
- `BossPreset.cs` - Presets de bosses
- `BossFightExample.cs` - Exemplo de uso
- `BossTest.cs` - Script de teste
- `BossDebugger.cs` - Script de debug e diagnóstico

### Debug e Diagnóstico

#### Usando o BossDebugger

1. **Adicione o script `BossDebugger` ao GameObject do boss**
2. **Configure a referência ao `BossFightSystem`**
3. **Ative as opções de debug desejadas:**
   - `Show Debug Info`: Mostra informações na tela
   - `Log State Changes`: Loga mudanças de estado
   - `Log Distance`: Loga distância ao player

#### Verificações Importantes

1. **Player configurado**: Certifique-se de que o campo `Player` está preenchido
2. **Ranges corretos**: Verifique se `Attack Distance` < `Max Chase Distance`
3. **Velocidade adequada**: `Chase Speed` deve ser > 0
4. **Ataques configurados**: Pelo menos um ataque deve estar na lista

#### Logs de Debug

O sistema agora gera logs detalhados:
- `"Boss: Player detectado, iniciando perseguição"`
- `"Boss: Perseguindo - Distância: X.XX"`
- `"Boss: Próximo o suficiente para atacar"`
- `"Boss: Executando ataque: NomeDoAtaque (Sequência: X/Y)"`
- `"Boss: Recuando após ataque"`
- `"Boss: Recuo terminado após X.XX segundos"`
- `"Boss: Iniciando descanso e afastamento"`
- `"Boss: Distância de descanso atingida: X.XX"`
- `"Boss: Afastamento terminado após X.XX segundos"`
- `"Boss: Descanso terminado"`

### Teste do Sistema

Use o script `BossTest.cs` para testar o sistema:
- **T**: Causar dano ao boss
- **Y**: Curar o boss
- **R**: Resetar o boss
- **I**: Mostrar informações

## Suporte

Para dúvidas ou sugestões, consulte a documentação ou entre em contato com a equipe de desenvolvimento.
