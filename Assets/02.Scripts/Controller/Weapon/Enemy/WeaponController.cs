using System.Collections;
using UnityEngine;
using static Define;

namespace Enemy {
    public abstract class WeaponController : Base.WeaponController {

        public EnemyController Enemy { get { return _unit as EnemyController; } }
        [SerializeField] private float _defaultTargetingChance;
        private float _currentTargetingChance;
        public float RandomBulletPos { get; private set; }
        protected override void Awake() {
            base.Awake();
        }

        protected override void Start() {
            base.Start();

            switch (Enemy.Level) {
                case (int)EnemyLevel.Low:
                    _currentTargetingChance = _defaultTargetingChance * 0.7f;
                    break;
                case (int)EnemyLevel.Middle:
                    _currentTargetingChance = _defaultTargetingChance;
                    break;
                case (int)EnemyLevel.High:
                    _currentTargetingChance = _defaultTargetingChance * 1.3f;
                    break;
            }
        }
        protected abstract override void Enable();

        protected override void DefaultShot(Vector3 angle) {
            if (!Enemy.TargetUnit)
                return;

            bool isHit;
            RaycastHit hit;

            Debug.DrawRay(_firePoint.position, angle * float.MaxValue, Color.green, 1f);
            isHit = Physics.Raycast(_firePoint.position, angle, out hit, float.MaxValue, _layerMask);
            

            float dir = Mathf.Abs((Enemy.transform.position - Enemy.TargetUnit.transform.position).magnitude);

            float ran = Random.Range(0, 100f);

            float chance = _currentTargetingChance;

            if(dir <= 5f) {
                chance = _currentTargetingChance * .9f;
            } else if (dir > 5f &&  dir < 10f) {
                chance = _currentTargetingChance * .7f;
            } else if(dir >= 10f) {
                chance = _currentTargetingChance * .5f;
            }


            if(ran <= chance) {
                Debug.Log($"적의 공격 성공");
            }
            if (ran > chance) {
                Debug.Log($"적의 공격 실패");
                return;
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
            CreateEffect($"muzzelFlash{ran}", _firePos.position, _unit.UnitRotate);
        }

        private IEnumerator CoRotate() {
            float startTime = Time.time;
            while (Time.time < startTime + 1f) {
                if(Enemy.TargetUnit == null) {
                    break;
                }
                Quaternion targetQ = Quaternion.LookRotation(Enemy.TargetUnit.TargetPos.position - Enemy.transform.position);
                Enemy.transform.rotation = Quaternion.Slerp(Enemy.transform.rotation, targetQ, 20f * Time.deltaTime);
                yield return null;
            }
        }

    }
}
