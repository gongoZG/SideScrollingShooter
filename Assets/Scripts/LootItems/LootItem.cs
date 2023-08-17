using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootItem : MonoBehaviour
{
    [SerializeField] float minSpeed = 5f;
    [SerializeField] float maxSpeed = 15f;
    [SerializeField] protected AudioData defaultPickUpSFX;
    
    protected Player player;
    protected AudioData pickUpSFX;
    Animator animator;
    const string PICKUP = "PickUp";
    protected Text lootMessage;

    private void Awake() {
        animator = GetComponent<Animator>();
        player = FindObjectOfType<Player>();
        // 传入布尔值的重载，可以获取到子物体中被禁用的组件
        lootMessage = GetComponentInChildren<Text>(true);
        pickUpSFX = defaultPickUpSFX;
    }

    private void OnEnable() {
        StartCoroutine(nameof(MoveCoroutine));
    }

    private void OnTriggerEnter2D(Collider2D other) {
        PickUp();
    }

    protected virtual void PickUp() {
        StopAllCoroutines();
        animator.Play(PICKUP);
        AudioManager.Instance.PlayRandomSFX(pickUpSFX);
    }

    IEnumerator MoveCoroutine() {
        float speed = Random.Range(minSpeed, maxSpeed);
        Vector3 direction = Vector3.left;

        while (true) {
            if (player.isActiveAndEnabled) {
                direction = (player.transform.position - transform.position).normalized;
            }
            transform.Translate(direction * speed * Time.deltaTime);
            yield return null;
        }
    }
}
