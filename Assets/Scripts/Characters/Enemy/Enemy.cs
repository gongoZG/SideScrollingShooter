using UnityEngine;

/// <summary>
/// 敌人类，继承自 Character
/// </summary>
public class Enemy : Character
{
    [SerializeField] int scorePoint = 100;
    [SerializeField] int deathEnergyBouns = 3;
    [SerializeField] protected int healthFactor;

    LootSpawner lootSpawner;

    protected virtual void Awake() {
        lootSpawner = GetComponent<LootSpawner>();
    }

    protected override void OnEnable() {
        SetHealth();
        base.OnEnable();
    }

    protected virtual void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.TryGetComponent<Player>(out Player player)) {
            Die();
            player.Die();
        }
    }

    /// <summary>
    /// 敌人死亡时触发的委托，增加分数，增加能量，从敌人列表中移除，生成战利品
    /// </summary>
    public override void Die()
    {
        ScoreManager.Instance.AddScore(scorePoint);
        PlayerEnergy.Instance.Obtain(deathEnergyBouns);
        EnemyManager.Instance.RemoveFromList(gameObject);
        lootSpawner.Spawn(transform.position);  // 生成战利品
        base.Die();
    }

    /// <summary>
    /// 设置敌人的血量，随着波数增加而增加
    /// </summary>
    protected virtual void SetHealth() {
        maxHealth += (int)(EnemyManager.Instance.WaveNumber / healthFactor);
    }
}
