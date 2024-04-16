using UnityEngine;

namespace Enemy {
    public class PistolController : WeaponController {
        protected override void Awake() {
            base.Awake();
            BoundValue = 0.1f;
            CrossValue = 350f;
            Damage = 20;
            Name = "Pistol";
            WeaponIcon = (Sprite)Managers.Resources.Load<Sprite>("Texture/PistolIcon");
            CurrentBullet = 12;
            RemainBullet = 12;
            MaxBullet = 60;
            CreateObject = (GameObject)Managers.Resources.Load<GameObject>("Prefabs/Item/Pistol");
        }
        public override void Shot() {
            base.Shot();
            DefaultShot(transform.forward);
        }
    }

}
