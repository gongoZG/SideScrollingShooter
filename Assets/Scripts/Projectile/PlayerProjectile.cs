using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : Projectile
{
    TrailRenderer trail;

    protected virtual void Awake() {
        trail = GetComponentInChildren<TrailRenderer>();

        // 子弹尾迹移动方向修正
        if (moveDirection != Vector2.right) {
            transform.GetChild(0).rotation = Quaternion.FromToRotation(Vector3.right, moveDirection);
        }

    }

    private void OnDisable() {
        trail.Clear();  // Removes all points from the TrailRenderer. Useful for restarting a trail from a new position.
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
        PlayerEnergy.Instance.Obtain(PlayerEnergy.PERCENT);  
    }

}
