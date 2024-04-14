using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShotgunController : ItemController {
    public override void Pickup(PlayerController player) {
        player.ChangeWeapon(Define.WeaponType.Shotgun);
        Managers.Resources.Destroy(gameObject);
    }
}
