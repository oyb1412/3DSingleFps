using static Define;

public class Item_PistolController : ItemController {
    public override void Pickup(UnitBase unit) {
        base.Pickup(unit);
        unit.ChangeWeapon(WeaponType.Pistol);
    }
}
