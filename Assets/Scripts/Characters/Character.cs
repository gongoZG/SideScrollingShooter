using System.Collections;
using UnityEngine;

/// <summary>
/// 角色基类，包含游戏中角色的基本属性和方法，比如血量、死亡等
/// </summary>
public class Character : MonoBehaviour
{
    [SerializeField] GameObject deathVFX;  // 死亡特效
    
    [Header("Health")]
    [SerializeField] protected float maxHealth;
    [SerializeField] StatesBar onHeadHealthBar;
    [SerializeField] bool showOnHeadHealthBar = true;
    [SerializeField] AudioData[] deathAudioData;

    protected float health;


    protected virtual void OnEnable() {
        health = maxHealth;
        if (showOnHeadHealthBar) ShowOnHeadHealthBar();
        else HideOnHeadHealthBar();
    }

    /// <summary>
    /// 在角色头顶显示血条
    /// </summary>
    public void ShowOnHeadHealthBar() {
        onHeadHealthBar.gameObject.SetActive(true);
        onHeadHealthBar.Initialize(health, maxHealth);
    }

    /// <summary>
    /// 隐藏角色头顶的血条
    /// </summary>
    public void HideOnHeadHealthBar() {
        onHeadHealthBar.gameObject.SetActive(false);
    }

    public virtual void TakeDamage(float damage) {
        if (health == 0) return;  // 先判断这个会消除下面的 bug
        health -= damage;
        // 不加 && gameObject.activeSelf 可能会在角色死亡后依然调用血条携程，触发bug
        if (showOnHeadHealthBar) {
            onHeadHealthBar.UpdateStates(health, maxHealth);
        }
        if (health <= 0f) {
            Die();
        }
    }

    public virtual void Die() {
        AudioManager.Instance.PlayRandomSFX(deathAudioData);
        health = 0f;
        PoolManager.Release(deathVFX, transform.position);
        gameObject.SetActive(false);
    }

    public virtual void RestoreHealth(float value) {
        if (health == maxHealth) return;
        health = Mathf.Clamp(health + value, 0, maxHealth);

        if (showOnHeadHealthBar) {
            onHeadHealthBar.UpdateStates(health, maxHealth);
        }
    }

    /// <summary>
    /// 持续回血功能
    /// </summary>
    /// <param name="waitTime"> 每次回血间隔时间 </param>
    /// <param name="percent"> 每次回血百分比 </param>
    /// <returns></returns>
    protected IEnumerator HealthRegenerateCoroutine(WaitForSeconds waitTime, float percent) {
        while (health < maxHealth) {
            yield return waitTime;
            RestoreHealth(maxHealth * percent);
        }
    }

    /// <summary>
    /// 持续掉血功能
    /// </summary>
    /// <param name="waitTime"> 每次掉血间隔 </param>
    /// <param name="percent"> 每次掉血百分比 </param>
    /// <returns></returns>
    protected IEnumerator DamageOverTimeCoroutine(WaitForSeconds waitTime, float percent) {
        while (health > 0f) {
            yield return waitTime;
            TakeDamage(maxHealth * percent);
        }
    }

}
