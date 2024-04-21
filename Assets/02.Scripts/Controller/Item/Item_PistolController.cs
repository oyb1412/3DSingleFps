
public class Item_PistolController : ItemController {

    
    public override void Pickup(UnitBase unit) {
        unit.ChangeWeapon(Define.WeaponType.Pistol);
        unit.CollideItem = null;
        if (gameObject == null)
            return;

        Managers.Resources.Destroy(gameObject);
    }
}
