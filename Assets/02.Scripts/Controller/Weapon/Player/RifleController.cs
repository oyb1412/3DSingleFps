using UnityEngine;

namespace Player {
    public class RifleController : WeaponController {
        protected override void Awake() {
            base.Awake();
            VerticalBoundValue = 0.75f;
            HorizontalBoundValue = 1f;
            CrossValue = 150f;
            Damage = 10;
            Name = "Rifle";
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
            Player.ShotEvent.Invoke();
        }
    }
}

