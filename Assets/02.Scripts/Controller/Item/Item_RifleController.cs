using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_RifleController : ItemController {
  
    public override void Pickup(UnitBase unit) {
        unit.ChangeWeapon(Define.WeaponType.Rifle);
        Managers.Resources.Destroy(gameObject);
        unit.CollideItem = null;
    }
}
