using UnityEngine;

namespace Enemy {
    public class PistolController : WeaponController {
        protected override void Awake() {
            base.Awake();

            Damage = 25;
            Type = Define.WeaponType.Pistol;
            Name = "Pistol";
            _shotDelay = 0.4f;
            WeaponIcon = (Sprite)Managers.Resources.Load<Sprite>("Texture/PistolIcon");
            CreateObject = (GameObject)Managers.Resources.Load<GameObject>("Prefabs/Item/Pistol");
        }

        protected override void Enable() {
            CurrentBullet = 12;
            RemainBullet = 12;
            MaxBullet = 60;
        }
        public override void Shot() {
            base.Shot();

            DefaultShot(transform.forward);
        }
    }

}
