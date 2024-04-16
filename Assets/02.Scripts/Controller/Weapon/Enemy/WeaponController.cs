using System.Collections;
using UnityEngine;
using static Define;
using static UnityEngine.GraphicsBuffer;

namespace Enemy {
    public class WeaponController : MonoBehaviour, IWeapon {
        public GameObject myObject { get { return gameObject; } }

        public int CurrentBullet { get; set; } = 30;
        public int RemainBullet { get; set; } = 30;
        public int MaxBullet { get; set; } = 180;
        public string Name { get; set; }
        public Sprite WeaponIcon { get; set; }

        protected Transform _firePos;
        protected int _layerMask;
        protected Transform _firePoint;
        protected EnemyController _enemy;
        protected ParticleSystem _ejectEffect;

        private Animator _animator;

        public float BoundValue { get; set; }
        public float CrossValue { get; set; }
        public int Damage { get; set; }
        public GameObject CreateObject { get ; set ; }

        protected virtual void Awake() {
            _animator = GetComponent<Animator>();
        }

        protected void Start() {
            _layerMask = (1 << (int)LayerType.Unit) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Ground) | (1 << (int)LayerType.Wall);
            _firePos = Util.FindChild(gameObject, "FirePos", true).transform;
            _ejectEffect = Util.FindChild(_firePos.gameObject, "Eject", false).GetComponent<ParticleSystem>();
        }
        public void Activation(Transform firePoint = null, UnitBase enemy = null) {
            if (firePoint != null && _firePoint == null) {
                _firePoint = firePoint;
            }
            if (enemy != null && _enemy == null) {
                _enemy = enemy as EnemyController;
            }
        }

        public void Reload() {
            if (MaxBullet >= RemainBullet) {
                CurrentBullet = RemainBullet;
                MaxBullet -= CurrentBullet;
            } else if (MaxBullet < RemainBullet) {
                CurrentBullet = RemainBullet - MaxBullet;
                MaxBullet = 0;
            }
        }

        protected void DefaultShot(Vector3 angle) {
            if (!_enemy.TargetUnit)
                return;

            Debug.DrawRay(_firePoint.position, angle * 100f, Color.green, 1f);
            bool isHit = Physics.Raycast(_firePoint.position, angle, out var hit, float.MaxValue, _layerMask);

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

            if (layer == (int)LayerType.Unit) {
                hit.collider.GetComponent<ITakeDamage>().TakeDamage(Damage, _enemy.transform, hit.transform);
            }
        }
        public virtual void Shot() {
            if (!_enemy.TargetUnit)
                return; 

            CurrentBullet--;
            StartCoroutine(CoRotate());
            _ejectEffect.Play();
            var ran = Random.Range(0, 5);
            GameObject muzzle = Managers.Resources.Instantiate($"Effect/muzzelFlash{ran}", null);
            muzzle.transform.position = _firePos.position;
            muzzle.transform.eulerAngles = _enemy.transform.forward;
        }

        private IEnumerator CoRotate() {
            float startTime = Time.time;

            while (Time.time < startTime + 1f) {
                if(_enemy.TargetUnit == null) {
                    break;
                }
                Quaternion targetQ = Quaternion.LookRotation(_enemy.TargetUnit.transform.position - _enemy.transform.position);
                _enemy.transform.rotation = Quaternion.Slerp(_enemy.transform.rotation, targetQ, 20f * Time.deltaTime);
                yield return null;
            }
        }

        public bool TryReload(UnitBase player) {
            if (MaxBullet <= 0)
                return false;

            if (CurrentBullet == RemainBullet)
                return false;

            return true;
        }

        public bool TryShot(UnitBase player) {
            if (player.State == Define.UnitState.Reload ||
                player.State == Define.UnitState.Get)
                return false;

            if (CurrentBullet <= 0) {
                player.Reload();
                return false;
            }

            if (!_enemy.TargetUnit)
                return false;

            return true;
        }

        public void SetAnimation(UnitState anime, bool trigger) {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(anime.ToString()))
                return;
            _animator.SetBool("Move", false);
            _animator.SetBool("Shot", false);


            _animator.SetBool(anime.ToString(), trigger);
            _enemy.Model.ChangeAnimation(anime, trigger);
        }

        public void SetAnimation(UnitState anime) {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName(anime.ToString()))
                return;

            _animator.SetTrigger(anime.ToString());
            _enemy.Model.ChangeAnimation(anime);
        }

        public void EndAnimation(string name) {
            _animator.ResetTrigger(name);
            _enemy.Model.ResetTrigger(name);
            _enemy.ChangeState(UnitState.Idle);
        }

        public void EneAnimation(string name, bool trigger) {
            _enemy.ChangeState(UnitState.Shot, trigger);
        }
    }

}
