using UnityEngine;

namespace Player {
    public abstract class WeaponController : Base.WeaponController {
        public float HorizontalBoundValue { get; protected set; }
        public float VerticalBoundValue { get; protected set; }
        public float CrossValue { get; protected set; }

        [SerializeField] public Vector3 _cameraPos;
        [SerializeField] public float _cameraView;
        [SerializeField] private RuntimeAnimatorController _NormalAC;
        [SerializeField] private RuntimeAnimatorController _AimAC;
        protected Transform _aimFirePos;


        public PlayerController Player { get { return _unit as PlayerController; } }

        public Animator CurrentAnime { get {
            if(Player.IsAiming) {
                    if (_animator.runtimeAnimatorController != _AimAC)
                        _animator.runtimeAnimatorController = _AimAC;
                }
            else {
                    if(_animator.runtimeAnimatorController != _NormalAC) {
                        _animator.runtimeAnimatorController = _NormalAC;
                        Player.ChangeState(Define.UnitState.Idle);
                        _animator.SetTrigger("Reset");
                    }
                    
                }
            return _animator;
            } 
        }

        public void ChangeAimAC(bool trigger) {
            if (trigger)
                _animator.runtimeAnimatorController = _AimAC;
            else
                _animator.runtimeAnimatorController = _NormalAC;
        }

        protected override void Awake() {
            base.Awake();
            _aimFirePos = Util.FindChild(gameObject, "AimFirePos", false).transform;
        }

        public override void Reload() {
            base.Reload();
            Player.BulletEvent.Invoke(CurrentBullet, MaxBullet, RemainBullet);
        }

        protected abstract override void Enable();

        protected override void DefaultShot(Vector3 angle) {
            Vector3 pos = Player.IsAiming ? _firePoint.position + (_firePoint.forward * .5f) : _firePoint.position;
            Debug.DrawRay(pos, angle * 100f, Color.cyan, 1f);
            bool isHit = Physics.Raycast(pos, angle, out var hit, float.MaxValue, _layerMask);
            DefaultShot(isHit, hit, Player);
        }
        public override void Shot() {
            base.Shot();

            float verticla = Player.IsAiming ? VerticalBoundValue * .5f : VerticalBoundValue;
            float horizontal = Player.IsAiming ? HorizontalBoundValue * .5f : HorizontalBoundValue;

            Player.StartCoroutine(Player.COBound(verticla, horizontal));
            Player.BulletEvent.Invoke(CurrentBullet, MaxBullet, RemainBullet);

            var ran = Random.Range(0, 5);
            Transform trans = Player.IsAiming ? _aimFirePos : _firePos;
            CreateEffect(_muzzleEffect[ran], trans.position, _unit.UnitRotate);

            if (CurrentBullet <= 0) {
                _unit.Reload();
                return;
            }
        }

    }
}

