using UnityEngine;
using static Define;

namespace Base {
    public class PistolController {
        public void SetPistol(ref int damage, ref WeaponType type, ref string name, ref float delay, ref Sprite icon, ref GameObject go) {
            damage = PISTOL_DAMAGE;
            type = WeaponType.Pistol;
            name = NAME_PISTOL;
            delay = PISTOL_SHOT_DELAY;
            icon = (Sprite)Managers.Resources.Load<Sprite>(PISTOL_ICON_PATH);
            go = null;
        }

        public void SetEnable(ref int currentBullet,ref int remainBullet,ref int maxBullet) {
            currentBullet = PISTOL_DEFAULT_BULLET;
            remainBullet = PISTOL_DEFAULT_BULLET;
            maxBullet = PISTOL_MAX_BULLET;
        }
    }
}
