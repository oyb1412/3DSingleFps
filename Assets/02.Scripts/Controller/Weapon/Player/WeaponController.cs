using UnityEngine;

namespace Player {
    public abstract class WeaponController : Base.WeaponController {
        public float HorizontalBoundValue { get; protected set; }
        public float VerticalBoundValue { get; protected set; }
        public float CrossValue { get; protected set; }

        public PlayerController Player { get { return _unit as PlayerController; } }

        protected override void Awake() {
            base.Awake();
        }

        public override void Reload() {
            base.Reload();
            Player.BulletEvent.Invoke(CurrentBullet, MaxBullet, RemainBullet);
        }

        protected abstract override void Enable();

        protected override void DefaultShot(Vector3 angle) {

            Debug.DrawRay(_firePoint.position, angle * 100f, Color.green, 1f);
            bool isHit = Physics.Raycast(_firePoint.position, angle, out var hit, float.MaxValue, _layerMask);

            DefaultShot(isHit, hit, Player);

            //if (!isHit)
            //    return;

            //int layer = hit.collider.gameObject.layer;

            //if (layer == (int)LayerType.Head &&
            //    hit.collider.GetComponentInParent<UnitBase>().gameObject != Player.gameObject) {
            //    Player.HeadshotEvent.Invoke();
            //    hit.collider.GetComponentInParent<ITakeDamage>().TakeDamage(Damage * 3, Player.transform, hit.collider.GetComponentInParent<UnitBase>().transform);
            //    GameObject blood = CreateEffect(_bloodEffect, hit.point);
            //    blood.transform.LookAt(_firePoint.position);
            //    Destroy(blood, 1f);
            //    return;

            //} else if (layer == (int)LayerType.Body &&
            //    hit.collider.GetComponentInParent<UnitBase>().gameObject != Player.gameObject) {
            //    Player.BodyshotEvent.Invoke();
            //    hit.collider.GetComponentInParent<ITakeDamage>().TakeDamage(Damage, Player.transform, hit.collider.GetComponentInParent<UnitBase>().transform);
            //    GameObject blood = CreateEffect(_bloodEffect, hit.point);
            //    blood.transform.LookAt(_firePoint.position);
            //    Destroy(blood, 1f);
            //    return;
            //} else if (layer == (int)LayerType.Obstacle ||
            //     layer == (int)LayerType.Wall) {
            //    GameObject impact = CreateEffect(_impactEffect, hit.point);
            //    impact.transform.LookAt(_firePoint.position);
            //    Destroy(impact, 1f);
            //    return;
            //} else if (layer == (int)LayerType.Ground) {
            //    GameObject impact = CreateEffect(_impactEffect, hit.point);
            //    impact.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
            //    Destroy(impact, 1f);
            //    return;
            //}
        }
        public override void Shot() {
            base.Shot();

            Player.StartCoroutine(Player.COBound());
            Player.BulletEvent.Invoke(CurrentBullet, MaxBullet, RemainBullet);
        }

    }
}

