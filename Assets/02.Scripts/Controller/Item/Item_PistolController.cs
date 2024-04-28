using static Define;

public class Item_PistolController : ItemController {
    public override void Pickup(UnitBase unit) {
        unit.ChangeWeapon(WeaponType.Pistol);
        base.Pickup(unit);
    }
}
