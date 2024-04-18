using UnityEngine;

namespace Enemy {
    public class RifleController : WeaponController {
        protected override void Awake() {
            base.Awake();
            Damage = 30;
            Name = "Rifle";
            _shotDelay = 0.1f;
            Type = Define.WeaponType.Rifle;

            WeaponIcon = (Sprite)Managers.Resources.Load<Sprite>("Texture/RifleIcon");
            CreateObject = (GameObject)Managers.Resources.Load<GameObject>("Prefabs/Item/Rifle");
        }

        protected override void Enable() {
            CurrentBullet = 30;
            RemainBullet = 30;
            MaxBullet = 180;
        }
        public override void Shot() {
            base.Shot();
            DefaultShot(transform.forward);
        }
    }

}
