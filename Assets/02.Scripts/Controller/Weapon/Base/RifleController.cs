using UnityEngine;
using static Define;

namespace Base {
    public class RifleController {
        public void SetRifle(ref int damage, ref WeaponType type, ref string name, ref float delay, ref Sprite icon, ref GameObject go) {
            damage = RIFLE_DAMAGE;
            type = WeaponType.Rifle;
            name = NAME_RIFLE;
            delay = RIFLE_SHOT_DELAY;
            icon = (Sprite)Managers.Resources.Load<Sprite>(RIFLE_ICON_PATH);
            go = (GameObject)Managers.Resources.Load<GameObject>(RIFLE_OBJECT_PATH);
        }

        public void SetEnable(ref int currentBullet, ref int remainBullet, ref int maxBullet) {
            currentBullet = RIFLE_DEFAULT_BULLET;
            remainBullet = RIFLE_DEFAULT_BULLET;
            maxBullet = RIFLE_MAX_BULLET;
        }
    }
}
