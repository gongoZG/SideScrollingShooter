using System.Collections;
using UnityEngine;

/// <summary>
/// 敌人的行动控制脚本，主要实现随机移动及随机开火
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] float moveSpeed =2f;
    [SerializeField] float moveRotationAngle = 25f;
    
    [Header("Fire")]
    [SerializeField] protected GameObject[] projectiles;
    [SerializeField] protected Transform muzzle;
    [SerializeField] protected ParticleSystem muzzleVFX;
    [SerializeField] protected float minFireInterval;
    [SerializeField] protected float maxFireInterval;
    [SerializeField] protected AudioData[] fireAudioData;

    protected float paddingX;
    protected float paddingY;
    protected Vector3 targetPosition;

    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    
    protected virtual void Awake() {
        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2;
        paddingY = size.y / 2;
    }

    // 未被玩家消灭时就持续启用移动及开火携程
    protected virtual void OnEnable() {
        StartCoroutine(nameof(RandomlyMovingCoroutine));
        StartCoroutine(nameof(RandomlyfireCoroutine));
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    /// <summary>
    /// 随即移动携程，随机选取右半屏幕的目标点并朝着该点移动
    /// </summary>
    /// <returns></returns>
    IEnumerator RandomlyMovingCoroutine() {
        transform.position = ViewPort.Instance.RandomEnemySpawnPosition(paddingX, paddingY);
        targetPosition = ViewPort.Instance.RandomRightHalfPosition(paddingX, paddingY);

        while (gameObject.activeSelf) {
            // 由于子弹时间会影响 fixedDeltaTime，因此每帧都要重新计算移动距离
            if (Vector3.Distance(transform.position, targetPosition) > moveSpeed * Time.fixedDeltaTime) {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.fixedDeltaTime);
                transform.rotation = Quaternion.AngleAxis(
                    (targetPosition - transform.position).normalized.y * moveRotationAngle, // 确定旋转方向
                    Vector3.right  // 表示绕 x 轴旋转
                );
            }
            else {
                targetPosition = ViewPort.Instance.RandomRightHalfPosition(paddingX, paddingY);
            }
            yield return waitForFixedUpdate;  // 挂起等待固定帧
        }
    }

    /// <summary>
    /// 间隔随机的时间后开火
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator RandomlyfireCoroutine() {
        while (gameObject.activeSelf) {
            // 这里想达到随机效果就得每次都 new
            yield return new WaitForSeconds(Random.Range(minFireInterval, maxFireInterval));

            if (GameManager.GameState == GameState.GameOver) yield break;  // 停止携程

            foreach (var projectile in projectiles) {
                PoolManager.Release(projectile, muzzle.position);
            }
            AudioManager.Instance.PlayRandomSFX(fireAudioData);
            muzzleVFX.Play();  // 普通敌人的开火特效不是循环播放
        }
    }

}
