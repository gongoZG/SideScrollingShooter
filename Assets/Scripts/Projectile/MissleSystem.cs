using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissleSystem : MonoBehaviour
{
    [SerializeField] int defaultamount = 5;
    [SerializeField] float cooldownTime = 1f;
    [SerializeField] GameObject misslePrefab = null;
    [SerializeField] AudioData launchSFX = null;
    int amount;
    bool isReady = true;
    WaitForSeconds waitForMissleCooldown;

    private void Awake() {
        amount = defaultamount;
        waitForMissleCooldown = new WaitForSeconds(cooldownTime);
    }

    private void Start() {
        MissleDisplay.UpdateAmountText(amount);
    }

    public void PickUp() {
        amount++;
        MissleDisplay.UpdateAmountText(amount);

        // 处理用完导弹又捡到的特殊情况
        if (amount == 1) {
            MissleDisplay.UpdateCooldownImage(0f);
            isReady = true;
        }
    }

    public void Launch(Transform muzzleTransform) {
        if (amount == 0 || !isReady) return;  // TODO: SFX && VFX
        isReady = false;
        // Release a missle clone from object pool
        PoolManager.Release(misslePrefab, muzzleTransform.position);
        // Play missle launch SFX
        AudioManager.Instance.PlayRandomSFX(launchSFX);
        amount--;
        MissleDisplay.UpdateAmountText(amount);

        if (amount == 0) {
            MissleDisplay.UpdateCooldownImage(1f);
        }
        else {
            StartCoroutine(CooldownCoroutine());
        }
    }

    IEnumerator CooldownCoroutine() {
        var cooldownValue = cooldownTime;

        while (cooldownValue > 0f) {
            MissleDisplay.UpdateCooldownImage(cooldownValue / cooldownTime);
            cooldownValue = Mathf.Max(cooldownValue - Time.deltaTime, 0f);

            yield return null;  // 挂起等待一帧
        }     

        isReady = true;
    }
}
