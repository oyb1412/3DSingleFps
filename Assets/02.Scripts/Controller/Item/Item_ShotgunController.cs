using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShotgunController : ItemController {
    
    public override void Pickup(UnitBase unit) {
        unit.ChangeWeapon(Define.WeaponType.Shotgun);
        Managers.Resources.Destroy(gameObject);
        unit.CollideItem = null;

    }
}
