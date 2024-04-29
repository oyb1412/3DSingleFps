using static Define;

public class Item_ShotgunController : ItemController {
    public override void Pickup(UnitBase unit) {
        unit.ChangeWeapon(WeaponType.Shotgun);
        base.Pickup(unit);
    }
}
