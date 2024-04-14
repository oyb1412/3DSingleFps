
public class Item_PistolController : ItemController {

    public override void Pickup(PlayerController player) {
        player.ChangeWeapon(Define.WeaponType.Pistol);
        Managers.Resources.Destroy(gameObject);
    }
}
