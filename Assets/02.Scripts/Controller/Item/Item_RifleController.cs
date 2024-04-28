using static Define;


public class Item_RifleController : ItemController {
    public override void Pickup(UnitBase unit) {
        base.Pickup(unit);
        unit.ChangeWeapon(WeaponType.Rifle);
    }
}
