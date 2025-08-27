using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boss Preset", menuName = "Boss Fight/Boss Preset")]
public class BossPreset : ScriptableObject
{
    [Header("Informações do Boss")]
    public string bossName = "Boss";
    public Sprite bossSprite;
    public RuntimeAnimatorController animatorController;

    [Header("Configurações de Combate")]
    public float health = 100f;
    public float chaseSpeed = 3f;
    public float attackDistance = 2f;
    public float maxChaseDistance = 10f;
    public float restDistance = 5f;
    public float restDuration = 3f;
    public float retreatDistance = 3f;
    public float retreatSpeed = 2f;
    public int maxComboLength = 3;

    [Header("Ataques")]
    public List<BossAttack> attacks = new List<BossAttack>();

    [Header("Comportamento")]
    public bool aggressiveMode = true;
    public float stunChance = 0.3f;
    public float stunDuration = 2f;

    [Header("Recompensas")]
    public GameObject victoryReward;
    public int experienceReward = 100;
    public int goldReward = 50;

    // Presets pré-definidos
    public static BossPreset CreateLagartixaPreset()
    {
        var preset = CreateInstance<BossPreset>();
        preset.bossName = "Lagartixa";
        preset.health = 120f;
        preset.chaseSpeed = 3.5f;
        preset.attackDistance = 2f;
        preset.maxChaseDistance = 12f;
        preset.restDistance = 6f;
        preset.restDuration = 2.5f;
        preset.retreatDistance = 3f;
        preset.retreatSpeed = 2.5f;
        preset.maxComboLength = 3;
        preset.aggressiveMode = true;

        // Ataques da Lagartixa
        preset.attacks.Add(new BossAttack
        {
            attackName = "ComboGiro",
            animationTrigger = "ComboGiro",
            attackDuration = 1.5f,
            cooldown = 3f,
            damage = 15f,
            range = 2f
        });

        preset.attacks.Add(new BossAttack
        {
            attackName = "PunchRight",
            animationTrigger = "PunchRight",
            attackDuration = 1f,
            cooldown = 2f,
            damage = 10f,
            range = 1.5f
        });

        preset.attacks.Add(new BossAttack
        {
            attackName = "PunchLeft",
            animationTrigger = "PunchLeft",
            attackDuration = 1f,
            cooldown = 2f,
            damage = 10f,
            range = 1.5f
        });

        return preset;
    }

    public static BossPreset CreateTankBossPreset()
    {
        var preset = CreateInstance<BossPreset>();
        preset.bossName = "Tank Boss";
        preset.health = 200f;
        preset.chaseSpeed = 2f;
        preset.attackDistance = 1.5f;
        preset.maxChaseDistance = 8f;
        preset.restDistance = 4f;
        preset.restDuration = 4f;
        preset.retreatDistance = 2f;
        preset.retreatSpeed = 1.5f;
        preset.maxComboLength = 2;
        preset.aggressiveMode = false;

        // Ataques do Tank Boss
        preset.attacks.Add(new BossAttack
        {
            attackName = "HeavySlam",
            animationTrigger = "HeavySlam",
            attackDuration = 2f,
            cooldown = 4f,
            damage = 25f,
            range = 2.5f
        });

        preset.attacks.Add(new BossAttack
        {
            attackName = "GroundPound",
            animationTrigger = "GroundPound",
            attackDuration = 1.5f,
            cooldown = 3f,
            damage = 20f,
            range = 3f
        });

        return preset;
    }

    public static BossPreset CreateAgileBossPreset()
    {
        var preset = CreateInstance<BossPreset>();
        preset.bossName = "Agile Boss";
        preset.health = 80f;
        preset.chaseSpeed = 5f;
        preset.attackDistance = 1.5f;
        preset.maxChaseDistance = 15f;
        preset.restDistance = 8f;
        preset.restDuration = 1.5f;
        preset.maxComboLength = 4;
        preset.aggressiveMode = true;

        // Ataques do Agile Boss
        preset.attacks.Add(new BossAttack
        {
            attackName = "QuickStrike",
            animationTrigger = "QuickStrike",
            attackDuration = 0.5f,
            cooldown = 1f,
            damage = 8f,
            range = 1.2f
        });

        preset.attacks.Add(new BossAttack
        {
            attackName = "DashAttack",
            animationTrigger = "DashAttack",
            attackDuration = 0.8f,
            cooldown = 2f,
            damage = 12f,
            range = 2f
        });

        preset.attacks.Add(new BossAttack
        {
            attackName = "ComboKick",
            animationTrigger = "ComboKick",
            attackDuration = 1.2f,
            cooldown = 2.5f,
            damage = 15f,
            range = 1.8f
        });

        return preset;
    }
}
