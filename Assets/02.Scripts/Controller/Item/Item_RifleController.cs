using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_RifleController : ItemController {
  
    public override void Pickup(UnitBase unit) {
        unit.ChangeWeapon(Define.WeaponType.Rifle);
        unit.CollideItem = null;
        if (gameObject)
            Managers.Resources.Destroy(gameObject);
    }
}
