using System.Collections;
using UnityEngine;
using static Define;

namespace Enemy {
    public abstract class WeaponController : Base.WeaponController {

        public EnemyController Enemy { get { return _unit as EnemyController; } }
        [SerializeField] private float _bulletAngle;
        protected override void Awake() {
            base.Awake();
        }
        protected abstract override void Enable();

        protected override void DefaultShot(Vector3 angle) {
            if (!Enemy.TargetUnit)
                return;

            bool isHit;
            RaycastHit hit;

            if (Enemy.Level == (int)EnemyLevel.High) {
                var ran1 = Random.Range(-_bulletAngle * 0.5f, _bulletAngle * 0.5f);
                var ran2 = Random.Range(-_bulletAngle * 0.5f, _bulletAngle * 0.5f);

                Quaternion pelletRotation = Quaternion.Euler(ran1, ran2, 0);
                Vector3 pelletDirection = pelletRotation * angle;

                Debug.DrawRay(_firePoint.position, pelletDirection * 100f, Color.green, 1f);
                isHit = Physics.Raycast(_firePoint.position, pelletDirection, out hit, float.MaxValue, _layerMask);
            }
            else if(Enemy.Level == (int)EnemyLevel.Low) {
                var ran1 = Random.Range(-_bulletAngle * 1.5f, _bulletAngle * 1.5f);
                var ran2 = Random.Range(-_bulletAngle * 1.5f, _bulletAngle * 1.5f);

                Quaternion pelletRotation = Quaternion.Euler(ran1, ran2, 0);
                Vector3 pelletDirection = pelletRotation * angle;

                Debug.DrawRay(_firePoint.position, pelletDirection * 100f, Color.green, 1f);
                isHit = Physics.Raycast(_firePoint.position, pelletDirection, out hit, float.MaxValue, _layerMask);
            }
            else {
                var ran1 = Random.Range(-_bulletAngle, _bulletAngle);
                var ran2 = Random.Range(-_bulletAngle, _bulletAngle);

                Quaternion pelletRotation = Quaternion.Euler(ran1, ran2, 0);
                Vector3 pelletDirection = pelletRotation * angle;

                Debug.DrawRay(_firePoint.position, pelletDirection * 100f, Color.green, 1f);
                isHit = Physics.Raycast(_firePoint.position, pelletDirection, out hit, float.MaxValue, _layerMask);
            }

            DefaultShot(isHit, hit, Enemy);
        }

        public override void Shot() {
            if (!TryShot(Enemy))
                return;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Dead"))
                return;

            StartCoroutine(CoRotate());
            base.Shot();

            var ran = Random.Range(0, 5);
            CreateEffect(_muzzleEffect[ran], _firePos.position, _unit.UnitRotate);

            if (CurrentBullet <= 0) {
                _unit.Reload();
                return;
            }
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
    }

}
