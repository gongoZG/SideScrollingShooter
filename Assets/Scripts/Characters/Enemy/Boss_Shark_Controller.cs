using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Boss_Shark 的控制器，包含独特的发射子弹方式与发射激光等功能
/// </summary>
public class Boss_Shark_Controller : EnemyController
{

#region Private SerializeField Attribute

    [SerializeField] float continousFireDuration = 1.5f;

    [Header("Player Detection")]
    [SerializeField] Transform playerDetectionTransform;
    [SerializeField] Vector3 playerDetectionSize;
    [SerializeField] LayerMask playerLayer;

    [Header("Beam")]
    [SerializeField] float beamCooldownTime = 12f;
    [SerializeField] AudioData beamChargingSFX;
    [SerializeField] AudioData beamLaunchSFX;

#endregion
    
#region Private Variable

    bool isBeamReady;
    WaitForSeconds waitForContinuousFireInterval;
    WaitForSeconds waitForFireInterval;
    WaitForSeconds waitBeamCooldownTime;

    List<GameObject> magazine;
    AudioData fireSFX;
    Animator animator;
    const string LAUNCHBEAM = "launchBeam";
    Transform playerTransform;  // 可以序列化再将玩家拖入赋值，但也会增加类间的耦合

#endregion
    
#region Game Running

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();

        waitForContinuousFireInterval = new WaitForSeconds(minFireInterval);
        waitForFireInterval = new WaitForSeconds(maxFireInterval);
        waitBeamCooldownTime = new WaitForSeconds(beamCooldownTime);

        magazine = new List<GameObject>(projectiles.Length);

        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

    }

    protected override void OnEnable() {
        isBeamReady = false;
        muzzleVFX.Stop();
        StartCoroutine(nameof(BeamCooldownCoroutine));
        base.OnEnable();
    }

#endregion

#region Fire

    /// <summary>
    /// 根据玩家是否在 Boss 前方来选择子弹与音效
    /// </summary>
    void LoadProjectiles() {
        magazine.Clear();
        // 检测玩家是否在 Boss 前方
        if (Physics2D.OverlapBox(playerDetectionTransform.position, playerDetectionSize, 0f, playerLayer)) {
            magazine.Add(projectiles[0]);
            fireSFX = fireAudioData[0];
        }
        else {
            if (Random.value < 0.5f) {
                magazine.Add(projectiles[1]);
                fireSFX = fireAudioData[1];
            }
            else {
                for (int i = 2; i < projectiles.Length; i++) {
                    magazine.Add(projectiles[i]);
                }
                fireSFX = fireAudioData[2];
            }
        }
    }

    /// <summary>
    /// 随机开火携程，开火结束后判断是否开启激光
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator RandomlyfireCoroutine() {
        while (isActiveAndEnabled) {
            if (GameManager.GameState == GameState.GameOver) yield break;

            // 相当于在开火结束之后判断
            if (isBeamReady) {
                ActivateBeamWeapon();
                StartCoroutine(nameof(ChasingPlayerCoroutine));
                yield break;  // 停止开火携程
            }

            yield return waitForFireInterval;
            yield return StartCoroutine(nameof(ContinuousFireCoroutine));
        }
    }

    /// <summary>
    /// boss的连续开火携程，每次调用时连续地发射子弹
    /// </summary>
    /// <returns></returns>
    IEnumerator ContinuousFireCoroutine() {
        LoadProjectiles();  // 装弹
        muzzleVFX.Play();

        float continuousFireTimer = 0f;

        while (continuousFireTimer < continousFireDuration) {
            foreach (var projectile in magazine) {
                PoolManager.Release(projectile, muzzle.position);
            }
            continuousFireTimer += minFireInterval;  // 负责数值的判断条件
            AudioManager.Instance.PlayRandomSFX(fireSFX);

            yield return waitForContinuousFireInterval;  // 负责实际的等待时间
        }

        muzzleVFX.Stop();
    }

#endregion
    
#region Beam

    /// <summary>
    /// 激光冷却携程
    /// </summary>
    /// <returns></returns>
    IEnumerator BeamCooldownCoroutine() {
        yield return waitBeamCooldownTime;
        isBeamReady = true;
    }

    /// <summary>
    /// 激活激光武器，播放动画
    /// </summary>
    void ActivateBeamWeapon() {
        isBeamReady = false;
        animator.SetTrigger(LAUNCHBEAM);
        AudioManager.Instance.PlayRandomSFX(beamChargingSFX);
    }

    /// <summary>
    /// 通过动画事件调用，播放激光发射音效
    /// </summary>
    void AE_LaunchBeamSFX() {
        AudioManager.Instance.PlayRandomSFX(beamLaunchSFX);
    }

    /// <summary>
    /// 通过动画事件调用，停止激光，开启随机开火
    /// </summary>
    void AE_StopBeam() {
        StopCoroutine(nameof(ChasingPlayerCoroutine));
        StartCoroutine(nameof(BeamCooldownCoroutine));
        StartCoroutine(nameof(RandomlyfireCoroutine));
    }

#endregion

#region Cheasing

    /// <summary>
    /// 追逐玩家的功能，通过改变 EnemyController 中的 targetPosition 来实现
    /// </summary>
    /// <returns></returns>
    IEnumerator ChasingPlayerCoroutine() {
        while (isActiveAndEnabled) {
            // 只在屏幕右边缘进行上下的移动
            targetPosition.x = ViewPort.Instance.MaxX - paddingX;
            targetPosition.y = playerTransform.position.y;

            yield return null;
        }
    }

#endregion

#region Gizmos

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(playerDetectionTransform.position, playerDetectionSize);
    }

#endregion
    
}
