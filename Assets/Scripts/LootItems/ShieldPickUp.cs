using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickUp : LootItem
{
    [SerializeField] AudioData fullHealthPickUpSFX;
    [SerializeField] int fullHealthScoreBonus = 200;
    [SerializeField] float shieldBonus = 20f;

    protected override void PickUp() {
        if (player.IsFullHealth) {
            pickUpSFX = fullHealthPickUpSFX;
            lootMessage.text = $"SCORE + {fullHealthScoreBonus}";
            ScoreManager.Instance.AddScore(fullHealthScoreBonus);
        }
        else {
            // Debug.Log("HP UP!");
            // 否则将播放默认音效
            lootMessage.text = $"SHIELD + {shieldBonus}";
            player.RestoreHealth(shieldBonus);
        }
        
        base.PickUp();
    }
}
