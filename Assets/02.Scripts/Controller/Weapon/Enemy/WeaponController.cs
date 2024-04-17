using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

namespace Enemy {
    public abstract class WeaponController : Base.WeaponController {

        public EnemyController Enemy { get { return _unit as EnemyController; } }
        [SerializeField] private float _bulletAngle;

        protected abstract override void Enable();

        protected override void DefaultShot(Vector3 angle) {
            if (!Enemy.TargetUnit)
                return;

            var ran1 = Random.Range(-_bulletAngle, _bulletAngle);
            var ran2 = Random.Range(-_bulletAngle, _bulletAngle);

            Quaternion pelletRotation = Quaternion.Euler(ran1, ran2, 0);
            Vector3 pelletDirection = pelletRotation * angle;

            Debug.DrawRay(_firePoint.position, pelletDirection * 100f, Color.green, 1f);
            bool isHit = Physics.Raycast(_firePoint.position, pelletDirection, out var hit, float.MaxValue, _layerMask);

            if (!isHit)
                return;

            int layer = hit.collider.gameObject.layer;
            if (layer == (int)LayerType.Obstacle ||
               layer == (int)LayerType.Wall) {
                GameObject impact = Managers.Resources.Instantiate("Effect/Impact", null);
                impact.transform.position = hit.point;
                impact.transform.LookAt(_firePoint.position);
                Destroy(impact, 1f);
                return;
            } else if (layer == (int)LayerType.Ground) {
                GameObject impact = Managers.Resources.Instantiate("Effect/Impact", null);
                impact.transform.position = hit.point;
                impact.transform.eulerAngles = new Vector3(-90f, 0f, 0f);
                Destroy(impact, 1f);
                return;
            }

            if (layer == (int)LayerType.Head) {
                hit.collider.GetComponentInParent<ITakeDamage>().TakeDamage(Damage * 2, Enemy.transform, hit.collider.GetComponentInParent<UnitBase>().transform);
                GameObject blood = Managers.Resources.Instantiate("Effect/Blood", null);
                blood.transform.position = hit.point;
                blood.transform.LookAt(_firePoint.position);
                Destroy(blood, 1f);
                return;
            }
            else if(layer == (int)LayerType.Body) {
                hit.collider.GetComponentInParent<ITakeDamage>().TakeDamage(Damage , Enemy.transform, hit.collider.GetComponentInParent<UnitBase>().transform);
                GameObject blood = Managers.Resources.Instantiate("Effect/Blood", null);
                blood.transform.position = hit.point;
                blood.transform.LookAt(_firePoint.position);
                Destroy(blood, 1f);
                return;
            }
            
        }

        public override void Shot() {
            if (!TryShot(Enemy))
                return;

            StartCoroutine(CoRotate());
            base.Shot();
        }

        private IEnumerator CoRotate() {
            float startTime = Time.time;

            while (Time.time < startTime + 1f) {
                if(Enemy.TargetUnit == null) {
                    break;
                }
                Quaternion targetQ = Quaternion.LookRotation(Enemy.TargetUnit.transform.position - Enemy.transform.position);
                Enemy.transform.rotation = Quaternion.Slerp(Enemy.transform.rotation, targetQ, 20f * Time.deltaTime);
                yield return null;
            }
        }

        public override bool TryShot(UnitBase unit) {
            if (!Enemy.TargetUnit) {
                Enemy.ChangeState(UnitState.Shot, false);
                Enemy.ChangeState(UnitState.Idle);
                return false;
            }

            return base.TryShot(unit);
        }

        public void EneAnimation(string name, bool trigger) {
            Enemy.ChangeState(UnitState.Shot, trigger);
        }

    }

}
