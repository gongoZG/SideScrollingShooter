using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGuidanceSystem : MonoBehaviour
{
    [SerializeField] Projectile projectile;
    [SerializeField] float minBallisticAngle = -90f;
    [SerializeField] float maxBallisticAngle = 90f;
    Vector3 targetDirection;
    float ballisticAngle;

    public IEnumerator HomingCoroutine(GameObject target) {
        ballisticAngle = Random.Range(minBallisticAngle, maxBallisticAngle);
        while (gameObject.activeSelf) {
            // 如果目标存活，旋转方向
            if (target.activeSelf) {
                // 目标到自身的方向向量
                targetDirection = target.transform.position - transform.position;
                // 计算弧度制并换算成角度并绕着 z 轴旋转
                transform.rotation = Quaternion.AngleAxis(
                    Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg, 
                    Vector3.forward
                );
                transform.rotation *= Quaternion.Euler(0f, 0f, ballisticAngle);
            }
            projectile.Move();  // 按设定的移动

            yield return null;
        }
    }
}
