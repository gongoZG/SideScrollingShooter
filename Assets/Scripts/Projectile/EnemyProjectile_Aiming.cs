using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile_Aiming : Projectile
{
    private void Awake() {
        // target = GameObject.FindObjectOfType<Player>();
        target = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void OnEnable() {
        StartCoroutine(nameof(MoveDirectionCoroutine));  // 获取精确的移动方向
        base.OnEnable();
    }

    IEnumerator MoveDirectionCoroutine() {
        yield return null;  // 挂起等待一帧，确保位置准确

        if (target.activeSelf) {
            moveDirection = (target.transform.position - transform.position).normalized;
        }
    }
}
