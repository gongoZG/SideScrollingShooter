using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMissle : PlayerProjectileOverdrive
{
    [SerializeField] AudioData targetAcquiredVoice = null;

    [Header("Speed Change")]
    [SerializeField] float lowSpeed = 8f;
    [SerializeField] float highSpeed = 35f;
    [SerializeField] float variableSpeedDelay = 0.5f;
    
    [Header("Explosion")]
    [SerializeField] GameObject explosionVFX = null;
    [SerializeField] AudioData explosionSFX = null;
    [SerializeField] float explosionRadius = 3f;
    [SerializeField] LayerMask enemyLayerMask = default;
    [SerializeField] float explosionDamage = 50f;

    WaitForSeconds waitVariableSpeedDelay;

    protected override void Awake() {
        base.Awake();
        waitVariableSpeedDelay = new WaitForSeconds(variableSpeedDelay);
    }

    protected override void OnEnable() {
        base.OnEnable();
        StartCoroutine(nameof(variableSpeedCoroutine));
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
        // Spawn a explosion VFX
        PoolManager.Release(explosionVFX, transform.position);
        // Play explosion SFX
        AudioManager.Instance.PlayRandomSFX(explosionSFX);
        // enemies in explosion take AOE damage
        var colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayerMask);

        foreach (var collider in colliders) {
            if (collider.TryGetComponent<Enemy>(out Enemy enemy)) {
                enemy.TakeDamage(explosionDamage);
            }
        }

    }

    IEnumerator variableSpeedCoroutine() {
        moveSpeed = lowSpeed;
        yield return waitVariableSpeedDelay;
        moveSpeed = highSpeed;

        if (target != null) {
            AudioManager.Instance.PlayRandomSFX(targetAcquiredVoice);
        }
    }

    // 画出爆炸范围
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
