using System.Collections;
using UnityEngine;

/// <summary>
/// 玩家能量系统，包含能量的获取和使用以及 overdrive 的开关
/// </summary>
public class PlayerEnergy : Singleton<PlayerEnergy>
{
    [SerializeField] EnergyBar energyBar;
    [SerializeField] float overdriveInerval = 0.1f;
    public const int MAX = 100;
    public const int PERCENT = 1;  // 击中获得能量
    int energy;
    WaitForSeconds waitForOverdriveInterval;
    bool available = true;  // 是否可以获取能量

    protected override void Awake() {
        base.Awake();
        waitForOverdriveInterval = new WaitForSeconds(overdriveInerval);
    }

    private void OnEnable() {
        PlayerOverDrive.on += PlayerOverDriveOn;
        PlayerOverDrive.off += PlayerOverDriveOff;
    }

    private void OnDisable() {
        PlayerOverDrive.on -= PlayerOverDriveOn;
        PlayerOverDrive.off -= PlayerOverDriveOff;
    }

    private void Start() {
        Obtain(MAX);
        energyBar.Initialize(energy, MAX);
    }

    public bool IsEnough(int value) => energy >= value;

    public void Obtain(int value) {
        if (energy >= MAX || !available || !gameObject.activeSelf) return;
        energy = Mathf.Clamp(energy + value, 0, MAX);
        energyBar.UpdateStates(energy, MAX);
    } 

    public void Use(int value) {
        energy -= value;
        energyBar.UpdateStates(energy, MAX);

        // 能量为零时触发 off 委托, 关闭 overdrive
        if (energy == 0 && !available) {
            PlayerOverDrive.off.Invoke();
        }
    }

    /// <summary>
    /// 开启 overdrive 时触发的委托，停止能量获取及逐渐消耗能量
    /// </summary>
    void PlayerOverDriveOn() {
        available = false;
        StartCoroutine(nameof(KeepUsingcoroutine));
    }

    /// <summary>
    /// 关闭 overdrive 时触发的委托，恢复能量获取
    /// </summary>
    void PlayerOverDriveOff() {
        available = true;
        StopCoroutine(nameof(KeepUsingcoroutine));
    }

    /// <summary>
    /// 能量爆发时逐渐消耗能量的携程
    /// </summary>
    /// <returns></returns>
    IEnumerator KeepUsingcoroutine() {
        while (gameObject.activeSelf && energy > 0) {
            yield return waitForOverdriveInterval;
            Use(PERCENT);
        }
    }
}
