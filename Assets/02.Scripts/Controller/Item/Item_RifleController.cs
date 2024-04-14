using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_RifleController : ItemController {
    public override void Pickup(PlayerController player) {
        player.ChangeWeapon(Define.WeaponType.Rifle);
        Managers.Resources.Destroy(gameObject);
    }
}
