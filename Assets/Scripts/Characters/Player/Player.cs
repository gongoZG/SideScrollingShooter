using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]  // 保证 Player 有 Rigidbody2D 组件
/// <summary>
/// 玩家类，包含玩家的移动、开火、闪避、能量爆发等功能
/// </summary>
public class Player : Character
{

#region Public Attribute
    
    public bool IsFullPower => weaponPower == 2;
    public bool IsFullHealth => health == maxHealth;

#endregion
    
#region Private SerializeField Attribute

    [SerializeField] StatesBar_HUD statesBar_HUD;
    [SerializeField] bool regenerateHealth = true;
    [SerializeField] float healthRegenerateTime;
    [SerializeField, Range(0, 1)] float healthRegeneratePercent;

    [Header("Input")]
    [SerializeField] PlayerInput input;
    
    [Header("Move")]
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float accelerationTime = 3f;
    [SerializeField] float decelerationTime = 3f;
    [SerializeField] float moveRotationAngle = 50f;
    
    [Header("Fire")]
    [SerializeField] GameObject projectile1;
    [SerializeField] GameObject projectile2;
    [SerializeField] GameObject projectile3;
    [SerializeField] GameObject projectileOverdrive;
    [SerializeField] ParticleSystem muzzleVFX;
    [SerializeField, Range(0, 2)] int weaponPower = 0;
    [SerializeField] Transform muzzleTop;
    [SerializeField] Transform muzzleMiddle;
    [SerializeField] Transform muzzleBottom;
    [SerializeField] AudioData projectileSFXData;
    [SerializeField] float fireInterval = 0.2f;

    [Header("Dodge")]
    [SerializeField] AudioData dodgeAudioData;
    [SerializeField, Range(0, 100)] int dodgeEnergyCost = 25;
    [SerializeField] float maxRoll = 720f;
    [SerializeField] float rollSpeed = 360f;
    [SerializeField] Vector3 dodgeScale = new Vector3(0.5f, 0.5f, 0.5f);

    [Header("OverDrive")]
    [SerializeField] float slowMotionDuration = 1f;
    [SerializeField] int overdriveDodgeFactor = 2;
    [SerializeField] float overdriveSpeedFactor = 1.2f;
    [SerializeField] float overdriveFireFactor = 1.2f;

    [Header("MuTeKi")]
    [SerializeField] float MuTeKiTime = 1f;

#endregion

#region Private Variable

    MissleSystem missleSystem;
    new Rigidbody2D rigidbody;  // new 用于创建名字被父类中成员占用的变量
    new Collider2D collider;
    float paddingX;
    float paddingY;
    Coroutine moveCoroutine;
    Coroutine healthRegenerateCoroutine;
    
    WaitForSeconds waitDecelerationTime;
    WaitForSeconds waitForfireInterval;
    WaitForSeconds waitForOverdriveFireInterval;
    WaitForSeconds waitForRegenerateTime;
    WaitForSeconds waitMuTeKiTime;

    bool isDodging;
    bool isOverDriving;

    float currentRoll;
    float dodgeDuration;
    float t;

    Vector2 moveDirection;  // 缓存移动方向
    Vector2 curVelocity;
    Quaternion curRotation;
    WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

#endregion
    
#region Game Running

    private void Awake() {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        missleSystem = GetComponent<MissleSystem>();
        
        dodgeDuration = maxRoll / rollSpeed;
        rigidbody.gravityScale = 0f;

        var size = transform.GetChild(0).GetComponent<Renderer>().bounds.size;
        paddingX = size.x / 2;
        paddingY = size.y / 2;

        waitDecelerationTime = new WaitForSeconds(decelerationTime);
        waitForfireInterval = new WaitForSeconds(fireInterval);
        waitForOverdriveFireInterval = new WaitForSeconds(fireInterval / overdriveFireFactor);
        waitForRegenerateTime = new WaitForSeconds(healthRegenerateTime);
        waitMuTeKiTime = new WaitForSeconds(MuTeKiTime);
    }

    private void Start() {
        input.EnableGameplayInput();
        statesBar_HUD.Initialize(health, maxHealth); 
    }

    protected override void OnEnable() {
        base.OnEnable();
        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
        input.onDodge += Dodge;
        input.onOverDrive += OverDrive;
        input.onLaunchMissle += LaunchMissle;
        PlayerOverDrive.on += OverDriveOn;
        PlayerOverDrive.off += OverDriveOff;
    }

    private void OnDisable() {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
        input.onDodge -= Dodge;
        input.onOverDrive -= OverDrive;
        input.onLaunchMissle -= LaunchMissle;
        PlayerOverDrive.on -= OverDriveOn;
        PlayerOverDrive.off -= OverDriveOff;
    }

#endregion
    
#region Override

    public override void TakeDamage(float damage) {
        base.TakeDamage(damage);
        PowerDown();  // 玩家武器威力下降
        statesBar_HUD.UpdateStates(health, maxHealth);
        TimeController.Instance.BulletTime(slowMotionDuration);  // 受伤时开启子弹时间

        if (gameObject.activeSelf) {
            Move(moveDirection);  // 保证即使速度被子弹抵消也能正常移动
            StartCoroutine(nameof(MuTeKiCoroutine));  // 受伤后短暂无敌
            // 开启回血携程
            if (regenerateHealth) {
                if (healthRegenerateCoroutine != null) {
                    StopCoroutine(healthRegenerateCoroutine);
                }
                healthRegenerateCoroutine = StartCoroutine(
                    HealthRegenerateCoroutine(waitForRegenerateTime, healthRegeneratePercent));
            }
        }  
    }

    // 重写 RestoreHealth，加入 HUD 血条的更新显示
    public override void RestoreHealth(float value) {
        base.RestoreHealth(value);
        statesBar_HUD.UpdateStates(health, maxHealth);
    }

    // 重写 Die，加入游戏结束的事件调用
    public override void Die() {
        GameManager.onGameOver?.Invoke();
        GameManager.GameState = GameState.GameOver;
        statesBar_HUD.UpdateStates(0, maxHealth);
        base.Die();
    }

#endregion

#region MuTeki

    /// <summary>
    /// 通过开启关闭碰撞体来实现短暂无敌
    /// </summary>
    /// <returns></returns>
    IEnumerator MuTeKiCoroutine() {
        collider.isTrigger = true;
        yield return waitMuTeKiTime;
        collider.isTrigger = false;
    }

#endregion

#region Move

    /// <summary>
    /// 玩家移动功能的实现，接受 InputSystem 传入的移动方向
    /// </summary>
    /// <param name="moveInput"></param>
    private void Move(Vector2 moveInput) {
        if (moveCoroutine != null) {
            // StepCoroutine 的重载
            StopCoroutine(moveCoroutine);  // 为避免停止携程时还需要传参，用变量存储并停止
        }
        moveDirection = moveInput.normalized;
        moveCoroutine = StartCoroutine(
            MoveCoroutine(accelerationTime, moveInput.normalized * moveSpeed, 
            Quaternion.AngleAxis(moveRotationAngle * moveInput.y, Vector3.right)));
        StopCoroutine(nameof(DecelerationCoroutine));  // 停用减速携程
        StartCoroutine(nameof(MovePositionLimitCoroutine));
    }

    /// <summary>
    /// 停止移动
    /// </summary>
    private void StopMove() {
        if (moveCoroutine != null) {
            StopCoroutine(moveCoroutine);
        }
        moveDirection = Vector2.zero;  // 移动方向归零, 防止被撞击时不受控制移动
        moveCoroutine = StartCoroutine(MoveCoroutine(decelerationTime, Vector2.zero, Quaternion.identity));
        StartCoroutine(nameof(DecelerationCoroutine));
    }

    /// <summary>
    /// 移动携程，通过帧间的插值来使速度逐渐变化
    /// </summary>
    /// <param name="time"> 加速/减速时间 </param>
    /// <param name="moveVelocity"> 最重要达到的速度 </param>
    /// <param name="moveRotation"> 移动时的旋转角度 </param>
    /// <returns></returns>
    IEnumerator MoveCoroutine(float time, Vector2 moveVelocity, Quaternion moveRotation) {
        t = 0f;
        curVelocity = rigidbody.velocity;
        curRotation = transform.rotation;

        // 两种写法都对
        // while (t < time) {
        //     t += Time.fixedDeltaTime;
        //     // 速度插值
        //     rigidbody.velocity = Vector2.Lerp(
        //         rigidbody.velocity, moveVelocity, t / time);
        //     transform.rotation = Quaternion.Lerp(
        //         transform.rotation, moveRotation, t / time);
        //     yield return null;  // 挂起
        // }

        while (t < 1) {
            t += Time.fixedDeltaTime / time;
            // 速度插值
            rigidbody.velocity = Vector2.Lerp(curVelocity, moveVelocity, t);
            transform.rotation = Quaternion.Lerp(curRotation, moveRotation, t);
            // yield return null;  // 挂起
            // 由于计算时使用的是 fixeddeltatime，因此这里要 waitforFixedUpdate;
            yield return waitForFixedUpdate;
        }
    }

    /// <summary>
    /// 限制玩家位置在视口之内
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovePositionLimitCoroutine() {
        while (true) {
            transform.position = ViewPort.Instance.PlayerMoveablePosition(
                transform.position, paddingX, paddingY);
            yield return null;  // 挂起至下一帧
        }
    }

    /// <summary>
    /// 在玩家松开按键减速时，挂起一段时间再停止位置限制携程，否则玩家会被卡出画面外
    /// </summary>
    /// <returns></returns>
    IEnumerator DecelerationCoroutine() {
        yield return waitDecelerationTime;  // 挂起等待减速时间
        StopCoroutine(nameof(MovePositionLimitCoroutine));
    }

#endregion
    
#region Fire
    void Fire() {
        muzzleVFX.Play();  // 枪口火焰特效
        StartCoroutine(nameof(FireCoroutine));
    }

    void StopFire() {
        muzzleVFX.Stop();
        // 直接传入携程函数无法停止，unity 的 bug
        StopCoroutine(nameof(FireCoroutine));
    }

    public void PowerUp() {
        // 注意这里要先++，--同理
        weaponPower = Mathf.Clamp(++weaponPower, 0, 2);
    }

    public void PowerDown() {
        weaponPower = Mathf.Clamp(--weaponPower, 0, 2);
    }

    /// <summary>
    /// 开火携程，通过开火后挂起一定间隔来实现不同攻速的效果
    /// </summary>
    /// <returns></returns>
    IEnumerator FireCoroutine() {
        while (true) {
            // Instantiate(projectile, muzzle.position, Quaternion.identity);  // 未使用对象池
            // PoolManager.Release(projectile, muzzle.position, Quaternion.identity);  // 使用对象池
            // 挂起直到下一帧，用此功能可以轻松实现持续触发的某个动作，由于设置了 fixed，此语句作用即为每秒钟发射 50 颗子弹
            // yield return null;  
            
            switch (weaponPower)
            {
                case 0:
                    // Instantiate(projectile1, muzzleMiddle.position, Quaternion.identity);
                    PoolManager.Release(isOverDriving ? projectileOverdrive : projectile1, muzzleMiddle.position);
                    break;
                case 1:
                    PoolManager.Release(isOverDriving ? projectileOverdrive : projectile1, muzzleTop.position);
                    PoolManager.Release(isOverDriving ? projectileOverdrive : projectile1, muzzleBottom.position);
                    break;
                case 2:
                    PoolManager.Release(isOverDriving ? projectileOverdrive : projectile2, muzzleTop.position);
                    PoolManager.Release(isOverDriving ? projectileOverdrive : projectile1, muzzleMiddle.position);
                    PoolManager.Release(isOverDriving ? projectileOverdrive : projectile3, muzzleBottom.position);
                    break;
                default:
                    break;
            }

            // yield return new WaitForSeconds(fireInterval);
            // AudioManager.Instance.PlaySFX(projectileSFXData);
            AudioManager.Instance.PlayRandomSFX(projectileSFXData);
            yield return isOverDriving ? waitForOverdriveFireInterval : 
                waitForfireInterval;  // 这样写不用在循环里 new 新的变量
        }
    }

#endregion

#region Dodge

    void Dodge() {
        if (isDodging || !PlayerEnergy.Instance.IsEnough(dodgeEnergyCost)) return;
        StartCoroutine(DodgeCoroutine());
    }

    IEnumerator DodgeCoroutine() {
        isDodging = true;
        AudioManager.Instance.PlaySFX(dodgeAudioData);
        // Use energy
        PlayerEnergy.Instance.Use(dodgeEnergyCost);
        // Player Muteki
        collider.isTrigger = true;  // 通过设置为 trigger 来禁用 OncollsionEnter2D
        // Player Roll
        currentRoll = 0f;
        var scale = transform.localScale;

        // 时间先减速再加速
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);

        // Player scale change
        // Method 1
        // while (currentRoll < maxRoll) {
        //     currentRoll += rollSpeed * Time.deltaTime;
        //     // Vector3.right 代表世界坐标中红色的 x 轴
        //     transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);

        //     // 向里翻滚
        //     if (currentRoll < maxRoll / 2) {
        //         scale.x = Mathf.Clamp(scale.x - Time.deltaTime / dodgeDuration, dodgeScale.x, 1f);
        //         scale.y = Mathf.Clamp(scale.y - Time.deltaTime / dodgeDuration, dodgeScale.y, 1f);
        //         scale.z = Mathf.Clamp(scale.z - Time.deltaTime / dodgeDuration, dodgeScale.z, 1f);
        //     }
        //     else {
        //         scale.x = Mathf.Clamp(scale.x + Time.deltaTime / dodgeDuration, dodgeScale.x, 1f);
        //         scale.y = Mathf.Clamp(scale.y + Time.deltaTime / dodgeDuration, dodgeScale.y, 1f);
        //         scale.z = Mathf.Clamp(scale.z + Time.deltaTime / dodgeDuration, dodgeScale.z, 1f);
        //     }
        //     transform.localScale = scale;

        //     yield return null;
        // }

        // Method 2
        var t1 = 0f;
        var t2 = 0f;

        while (currentRoll < maxRoll) {
            currentRoll += rollSpeed * Time.deltaTime;
            transform.rotation = Quaternion.AngleAxis(currentRoll, Vector3.right);

            if (currentRoll < maxRoll / 2) {
                t1 += Time.deltaTime / dodgeDuration;
                transform.localScale = Vector3.Lerp(transform.localScale, dodgeScale, t1);
            }
            else {
                t2 += Time.deltaTime / dodgeDuration;
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, t2);
            }

            yield return null;
        }

        collider.isTrigger = false;
        isDodging = false;

        // Method 3 Bezier Curve
    }

#endregion

#region OverDrive

    /// <summary>
    /// 能量爆发功能的实现，通过触发 PlayerOverDrive.on 事件来实现
    /// </summary>
    private void OverDrive() {
        if (!PlayerEnergy.Instance.IsEnough(PlayerEnergy.MAX)) return;
        PlayerOverDrive.on.Invoke();
    }

    /// <summary>
    /// 能量爆发开启事件订阅的玩家属性变化的函数
    /// </summary>
    void OverDriveOn() {
        isOverDriving = true;
        dodgeEnergyCost *= overdriveDodgeFactor;
        moveSpeed *= overdriveSpeedFactor;
        TimeController.Instance.BulletTime(slowMotionDuration, slowMotionDuration);
    }

    /// <summary>
    /// 能量爆发结束事件订阅的玩家属性变化的函数
    /// </summary>
    void OverDriveOff() {
        isOverDriving = false;
        dodgeEnergyCost /= overdriveDodgeFactor;
        moveSpeed /= overdriveSpeedFactor;
    }

#endregion

#region LaunchMissle
    void LaunchMissle() {
        missleSystem.Launch(muzzleMiddle);
    }

    public void PickUpMissle() {
        missleSystem.PickUp();
    }

#endregion

#region Gizmos

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(muzzleTop.position, 0.005f);
        Gizmos.DrawWireSphere(muzzleMiddle.position, 0.005f);
        Gizmos.DrawWireSphere(muzzleBottom.position, 0.005f);
    }


#endregion
    
}
