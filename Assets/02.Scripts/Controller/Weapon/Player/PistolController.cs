using UnityEngine;

namespace Player {
    public class PistolController : WeaponController {
        protected override void Awake() {
            base.Awake();
            VerticalBoundValue = 1f;
            HorizontalBoundValue = 1f;
            CrossValue = 350f;
            Damage = 20;
            Name = "Pistol";
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
            for (int i = 0; i < 5; i++)
                Player.ShotEvent.Invoke();
        }
    }
}

