using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ShotgunController : ItemController {
    
    public override void Pickup(UnitBase unit) {
        unit.ChangeWeapon(Define.WeaponType.Shotgun);
        unit.CollideItem = null;
            if(gameObject)
        Managers.Resources.Destroy(gameObject);
    }
}
