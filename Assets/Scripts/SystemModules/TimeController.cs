using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : Singleton<TimeController>
{
    [SerializeField, Range(0, 1)] float bulletTimeScale = 0.1f;
    
    // bool isPaused;  // 改用 GameManager 来控制游戏状态
    
    float defualtFixeddeltaTime;
    float timeScaleBeforePause;  // 记录进入子弹时间的时间刻度，取消暂停时恢复，能使游戏更连贯
    float t;

    protected override void Awake() {
        base.Awake();
        defualtFixeddeltaTime = Time.fixedDeltaTime;
    }

    public void Pause() {
        timeScaleBeforePause = Time.timeScale;
        Time.timeScale = 0f;
        GameManager.GameState = GameState.Paused;
    }

    public void Unpause() {
        Time.timeScale = timeScaleBeforePause;
        GameManager.GameState = GameState.Playing;
    }

    public void BulletTime(float duration) {
        Time.timeScale = bulletTimeScale;
        StartCoroutine(SlowOutCoroutine(duration));
    }

    public void BulletTime(float inDuration, float outDuration) {
        StartCoroutine(SlowInAndOutCoroutine(inDuration, outDuration));
    }

    public void BulletTime(float inDuration, float keepingDuration, float outDuration) {
        StartCoroutine(SlowInKeepAndOutCoroutine(
            inDuration, keepingDuration, outDuration));
    }

    public void SlowIn(float duration) {
        StartCoroutine(SlowInCoroutine(duration));
    }

    public void SlowOut(float duration) {
        StartCoroutine(SlowOutCoroutine(duration));
    }

    IEnumerator SlowInKeepAndOutCoroutine(float inDuration, float keepingDuration, float outDuration) {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        yield return new WaitForSecondsRealtime(keepingDuration);  // 不受 timeScale 影响
        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    IEnumerator SlowInAndOutCoroutine(float inDuration, float outDuration) {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    IEnumerator SlowInCoroutine(float duration) {
        t = 0f;
        while (t < 1f) {
            // 只有当游戏非暂停状态时才继续携程
            if (GameManager.GameState != GameState.Paused) {
                // t += Time.deltaTime / duration;  // 这里使用 deltaTime 的话也会被时间刻度修改，持续时间会长于设定
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(1f, bulletTimeScale, t);
                // 时间恢复的每一帧都修改固定帧时间
                Time.fixedDeltaTime = defualtFixeddeltaTime * Time.timeScale;
            }
        
            yield return null; 
        }
    }

    IEnumerator SlowOutCoroutine(float duration) {
        t = 0f;
        while (t < 1f) {
            if (GameManager.GameState != GameState.Paused) {
                // t += Time.deltaTime / duration;  // 这里使用 deltaTime 的话也会被时间刻度修改，持续时间会长于设定
                t += Time.unscaledDeltaTime / duration;
                Time.timeScale = Mathf.Lerp(bulletTimeScale, 1f, t);
                // 时间恢复的每一帧都修改固定帧时间
                Time.fixedDeltaTime = defualtFixeddeltaTime * Time.timeScale;
            }
            
            yield return null; 
        }
    }

}
