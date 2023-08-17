using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 玩家 overdrive 系统，包含开启和关闭 overdrive 时的委托
/// </summary>
public class PlayerOverDrive : MonoBehaviour
{
    public static UnityAction on = delegate {};
    public static UnityAction off = delegate {};

    [SerializeField] GameObject triggerVFX;
    [SerializeField] GameObject engineVFXNormal;
    [SerializeField] GameObject engineVFXOverdrive;

    [SerializeField] AudioData onSFX;
    [SerializeField] AudioData offSFX;

    private void Awake() {
        on += On;
        off += Off;
    }

    private void OnDestroy() {
        on -= On;
        off -= Off;
    }

    /// <summary>
    /// 能量爆发开启时的特效切换及音效播放
    /// </summary>
    void On() {
        triggerVFX.SetActive(true);
        engineVFXNormal.SetActive(false);
        engineVFXOverdrive.SetActive(true);
        AudioManager.Instance.PlayRandomSFX(onSFX);
    }

    /// <summary>
    /// 能量爆发关闭时的特效切换及音效播放
    /// </summary>
    void Off() {
        // triggerVFX.SetActive(false);
        engineVFXNormal.SetActive(true);
        engineVFXOverdrive.SetActive(false);
        AudioManager.Instance.PlayRandomSFX(offSFX);
    }

}
