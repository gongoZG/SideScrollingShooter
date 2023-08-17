using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeam : MonoBehaviour
{
    [SerializeField] float damage = 50f;
    [SerializeField] GameObject hitVFX;

    // 为什么这里不用 enter 而必须用 stay
    private void OnCollisionStay2D(Collision2D other) {
        if (other.gameObject.TryGetComponent<Player>(out Player player)) {
            player.TakeDamage(damage);
            var contactPoint = other.GetContact(0);  // 返回碰撞点
            PoolManager.Release(hitVFX, contactPoint.point, 
                Quaternion.LookRotation(contactPoint.normal)  // 特效朝向即为碰撞点的法线方向
            );
        }
    }
}
