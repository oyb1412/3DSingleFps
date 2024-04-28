using static Define;


public class Item_RifleController : ItemController {
    public override void Pickup(UnitBase unit) {
        unit.ChangeWeapon(WeaponType.Rifle);
        base.Pickup(unit);
    }
}
