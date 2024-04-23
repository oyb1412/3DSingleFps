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
            
            float ran = Random.Range(0, 100f);

            float dir = Mathf.Abs((Enemy.transform.position - Enemy.TargetUnit.transform.position).magnitude);

            //todo
            //거리 30이상 
            //float chance = Mathf.Clamp()

            if (ran < _currentTargetingChance)
                return;

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
