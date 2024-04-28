using static Define;

public class Item_ShotgunController : ItemController {
    public override void Pickup(UnitBase unit) {
        base.Pickup(unit);
        unit.ChangeWeapon(WeaponType.Shotgun);
    }
}
