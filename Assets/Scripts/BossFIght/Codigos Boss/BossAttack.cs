using UnityEngine;

[System.Serializable]
public class BossAttack
{
    public string attackName;
    public string animationTrigger;
    public float attackDuration = 1f;
    public float cooldown = 2f;
    public float damage = 10f;
    public float range = 2f;
    public bool requiresCloseRange = true;
}
