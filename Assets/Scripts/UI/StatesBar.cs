using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StatesBar : MonoBehaviour
{
    [SerializeField] Image fillImageBack;
    [SerializeField] Image fillImageFront;
    [SerializeField] float fillSpeed = 0.1f;
    [SerializeField] bool delayFill = true;
    [SerializeField] float fillDelay = 0.5f;  // 血条 buffer 是否延迟变化

    protected float currentFillAmount;
    protected float targetFillAmount;
    float t;  // 防止插值中频繁地回收
    WaitForSeconds waitForDelayFill;  // 延迟填充
    Coroutine bufferedfillingCoroutine;
    Canvas canvas;

    private void Awake() {
        // 只有有canvas组件的才将相机设置为主相机
        if (TryGetComponent<Canvas>(out Canvas canvas)) {
            canvas.worldCamera = Camera.main;  // canvas 绑定主摄像机
        }
        waitForDelayFill = new WaitForSeconds(fillDelay);
    }

    private void OnDisable() {
        StopAllCoroutines();
    }

    public virtual void Initialize(float currentValue, float maxValue) {
        currentFillAmount = currentValue / maxValue;
        targetFillAmount = currentFillAmount;
        fillImageBack.fillAmount = currentFillAmount;
        fillImageFront.fillAmount = currentFillAmount;
    }

    public virtual void UpdateStates(float currentValue, float maxValue) {
        targetFillAmount = currentValue / maxValue;
        if (bufferedfillingCoroutine != null) {
            StopCoroutine(bufferedfillingCoroutine);
        }
        if (currentFillAmount > targetFillAmount) {
            // if states reduce: 1. front -> target 2. back slowly reduce
            fillImageFront.fillAmount = targetFillAmount;
            bufferedfillingCoroutine = StartCoroutine(
                BufferedFillingCoroutine(fillImageBack));
            
            return;
        }
        if (currentFillAmount < targetFillAmount) {
            // if states increase: 1. back -> target 2. front slowly increase
            fillImageBack.fillAmount = targetFillAmount;
            bufferedfillingCoroutine = StartCoroutine(
                BufferedFillingCoroutine(fillImageFront)
            );
        }
    }

    IEnumerator BufferedFillingCoroutine(Image image) {
        if (delayFill) {
            yield return waitForDelayFill;
        }

        t = 0f;
        while (t < 1f) {
            t += Time.deltaTime * fillSpeed;

            currentFillAmount = Mathf.Lerp(currentFillAmount, targetFillAmount, t);
            image.fillAmount = currentFillAmount;

            yield return null;  // 挂起，实现血条buffer的持续变化
        }
    }
}
