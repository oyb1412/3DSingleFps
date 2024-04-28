using UnityEngine;
using static Define;

namespace Base {
    public class ShotgunController {
        public void SetShotgun(ref int damage, ref WeaponType type, ref string name, ref float delay, ref Sprite icon, ref GameObject go) {
            damage = SHOTGUN_DAMAGE;
            type = WeaponType.Shotgun;
            name = NAME_SHOTGUN;
            delay = SHOTGUN_SHOT_DELAY;
            icon = (Sprite)Managers.Resources.Load<Sprite>(SHOTGUN_ICON_PATH);
            go = (GameObject)Managers.Resources.Load<GameObject>(SHOTGUN_OBJECT_PATH);
        }

        public void SetEnable(ref int currentBullet,ref int remainBullet,ref int maxBullet) {
            currentBullet = SHOTGUN_DEFAULT_BULLET;
            remainBullet = SHOTGUN_DEFAULT_BULLET;
            maxBullet = SHOTGUN_MAX_BULLET;
        }
    }
}
