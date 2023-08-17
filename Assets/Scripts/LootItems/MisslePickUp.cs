using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MisslePickUp : LootItem
{
    protected override void PickUp() {
        // 这里由于 lootItem 基类中声明了 player，因此只能将该功能封装在 Player 内部才能调用
        player.PickUpMissle();  
        base.PickUp();
    }
}
