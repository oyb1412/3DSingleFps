using System.Collections;
using UnityEngine;
using static Define;

namespace Enemy {
    public abstract class WeaponController : Base.WeaponController {

        public EnemyController Enemy { get { return _unit as EnemyController; } }
        private float _currentTargetingChance;

        protected override void Awake() {
            base.Awake();
        }

        protected override void Start() {
            base.Start();

            switch (Enemy.Level) {
                case (int)EnemyLevel.Low:
                    _currentTargetingChance = ENEMY_LOW_ATTACK_CHANCE;
                    break;
                case (int)EnemyLevel.Middle:
                    _currentTargetingChance = ENEMY_DEFAULT_ATTACK_CHANCE;
                    break;
                case (int)EnemyLevel.High:
                    _currentTargetingChance = ENEMY_HIGH_ATTACK_CHANCE;
                    break;
            }
        }
        protected abstract override void Enable();

        protected override void DefaultShot(Vector3 angle) {
            if (!Enemy.TargetUnit)
                return;

            bool isHit;
            RaycastHit hit;

            isHit = Physics.Raycast(_firePoint.position, angle, out hit, float.MaxValue, _layerMask);

            //BulletController bullet = Managers.Resources.Instantiate(_bullet, null).GetComponent<BulletController>(); 
            //bullet.Init(transform.position, angle, Enemy.gameObject);

            float dir = Mathf.Abs((Enemy.transform.position - Enemy.TargetUnit.transform.position).magnitude);

            float ran = Random.Range(0, 100f);

            float chance = _currentTargetingChance;

            if(dir <= ENEMY_ATTACK_CHANCE_MINUS_RANGE) {
                chance = _currentTargetingChance - ENEMY_ATTACK_CHANCE_MINUS;
            } else if (dir > ENEMY_ATTACK_CHANCE_MINUS_RANGE &&  dir < ENEMY_ATTACK_CHANCE_MINUS_RANGE * 2) {
                chance = _currentTargetingChance - ENEMY_ATTACK_CHANCE_MINUS * 2f;
            } else if(dir >= ENEMY_ATTACK_CHANCE_MINUS_RANGE * 2) {
                chance = _currentTargetingChance - ENEMY_ATTACK_CHANCE_MINUS * 3f;
            }
           
            if (ran > chance)
                return;

            DefaultShot(isHit, hit, Enemy);
        }

        public override void Shot() {
            if (!TryShot(Enemy))
                return;

            if (Animator.GetCurrentAnimatorStateInfo(0).IsName(ANIMATOR_PARAMETER_DEAD))
                return;

            StartCoroutine(CoRotate());
            base.Shot();

            var ran = Random.Range(0, _muzzleEffect.Length);
            CreateEffect(_muzzleEffect[ran], _firePos.position, _unit.transform.rotation);
        }

        private IEnumerator CoRotate() {
            float startTime = Time.time;
            while (Time.time < startTime + 1f) {
                if(Enemy.TargetUnit == null) {
                    break;
                }
                Quaternion targetQ = Quaternion.LookRotation(Enemy.TargetUnit.transform.position - Enemy.transform.position);
                Enemy.transform.rotation = Quaternion.Slerp(Enemy.transform.rotation, targetQ, ENEMY_ROTATE_SPEED * Time.deltaTime);
                yield return null;
            }
        }

    }
}
