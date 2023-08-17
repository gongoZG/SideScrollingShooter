using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPowerPickUp : LootItem
{
    [SerializeField] AudioData fullPowerPickUpSFX;
    [SerializeField] int fullPowerScoreBonus = 200;

    protected override void PickUp() {
        if (player.IsFullPower) {
            pickUpSFX = fullPowerPickUpSFX;
            lootMessage.text = $"SCORE + {fullPowerScoreBonus}";
            ScoreManager.Instance.AddScore(fullPowerScoreBonus);
        }
        else {
            lootMessage.text = "POWER UP!";
            player.PowerUp();
        }
        base.PickUp();
    }
}
