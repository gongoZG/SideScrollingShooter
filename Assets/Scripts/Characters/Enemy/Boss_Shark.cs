using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Boss_Shark 的相关属性
/// </summary>
public class Boss_Shark : Enemy
{
    // [SerializeField] BossHealthBar healthBar;  // 这样写虽然不用 findobject 但是增加了类间的耦合
    BossHealthBar healthBar;
    Canvas healthBarCanvas;

    protected override void Awake() {
        base.Awake();
        healthBar = FindObjectOfType<BossHealthBar>();
        healthBarCanvas = healthBar.GetComponentInChildren<Canvas>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        healthBar.Initialize(health, maxHealth);
        // Debug.Log(healthBar);
        healthBarCanvas.enabled = true;
        // Debug.Log(healthBarCanvas);
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent<Player>(out Player player)) {
            player.Die();
        }
    }

    public override void Die()
    {
        healthBarCanvas.enabled = false;
        base.Die();
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        healthBar.UpdateStates(health, maxHealth);
    }

    protected override void SetHealth() {
        maxHealth += EnemyManager.Instance.WaveNumber * healthFactor;
    }
}
