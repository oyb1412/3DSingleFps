
public class Item_PistolController : ItemController {

    
    public override void Pickup(UnitBase unit) {
        unit.ChangeWeapon(Define.WeaponType.Pistol);
        Managers.Resources.Destroy(gameObject);
        unit.CollideItem = null;

    }
}
