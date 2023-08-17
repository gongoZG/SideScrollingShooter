using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] GameObject hitVFX;  // 命中特效
    [SerializeField] protected float moveSpeed = 10f;
    [SerializeField] float damage;
    [SerializeField] protected Vector2 moveDirection;
    [SerializeField] AudioData[] hitAudioData;
    protected GameObject target;

    protected virtual void OnEnable() {
        StartCoroutine(MoveDirectly());
    }

    IEnumerator MoveDirectly() {
        while (gameObject.activeSelf) {
            Move();
            yield return null;
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D other) {
        // TryGetComponent<T> 用于抓取指定类型的对象，抓取到返回真
        // 消耗性能比 GetComponent 少，适合用于判断语句
        if (other.gameObject.TryGetComponent<Character>(out Character character)) {
            character.TakeDamage(damage);
            var contactPoint = other.GetContact(0);  // 返回碰撞点
            PoolManager.Release(hitVFX, contactPoint.point, 
                Quaternion.LookRotation(contactPoint.normal)  // 特效朝向即为碰撞点的法线方向
            );
            AudioManager.Instance.PlayRandomSFX(hitAudioData);
            gameObject.SetActive(false);  // 返回对象池
        }
    }

    protected void SetTarget(GameObject target) => this.target = target;
    public void Move() => transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
}
