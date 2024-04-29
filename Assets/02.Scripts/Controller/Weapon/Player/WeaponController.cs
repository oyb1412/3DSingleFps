using UnityEngine;
using static Define;

namespace Player {
    public abstract class WeaponController : Base.WeaponController {
        [SerializeField] private RuntimeAnimatorController _NormalAC;
        [SerializeField] private RuntimeAnimatorController _AimAC;
        protected Transform _aimFirePos;
        public float HorizontalBoundValue { get; protected set; }
        public float VerticalBoundValue { get; protected set; }
        public float CrossValue { get; protected set; }
        public Vector3 CameraPos { get; private set; } = AIM_CAMERA_POSITION;
        public float CameraView { get; protected set; }
        public PlayerController Player { get { return _unit as PlayerController; } }
        public Animator CurrentAnime { 
            get {
            if(Player.IsAiming) {
                    if (Animator.runtimeAnimatorController != _AimAC)
                        Animator.runtimeAnimatorController = _AimAC;
                }
            else {
                    if(Animator.runtimeAnimatorController != _NormalAC) {
                        Animator.runtimeAnimatorController = _NormalAC;
                        Player.ChangeState(UnitState.Idle);
                        Animator.SetTrigger(ANIMATOR_PARAMETER_RESET);
                    }
                }
            return Animator;
            } 
        }

        public void ChangeAimAC(bool trigger) {
            if (trigger)
                Animator.runtimeAnimatorController = _AimAC;
            else
                Animator.runtimeAnimatorController = _NormalAC;
        }

        protected override void Start() {
            base.Start();

            _aimFirePos = Util.FindChild(gameObject, NAME_AIMFIREPOS, false).transform;
        }
        protected override void Awake() {
            base.Awake();
        }

        public override void Reload() {
            base.Reload();
            Player.SetReload(CurrentBullet, MaxBullet, RemainBullet);
        }

        protected abstract override void Enable();

        protected override void DefaultShot(Vector3 angle) {
            Vector3 pos = Player.IsAiming ? _firePoint.position + (_firePoint.forward * .5f) : _firePoint.position;
            bool isHit = Physics.Raycast(pos, angle, out var hit, float.MaxValue, _layerMask);
            DefaultShot(isHit, hit, Player);
        }
        public override void Shot() {
            base.Shot();
            float verticla = Player.IsAiming ? VerticalBoundValue * .5f : VerticalBoundValue;
            float horizontal = Player.IsAiming ? HorizontalBoundValue * .5f : HorizontalBoundValue;

            Player.SetShot(verticla, horizontal, CurrentBullet, MaxBullet, RemainBullet);

            var ran = Random.Range(0, _muzzleEffect.Length);
            Transform trans = Player.IsAiming ? _aimFirePos : _firePos;
            GameObject go = CreateEffect(_muzzleEffect[ran], trans.position, _unit.transform.eulerAngles);
            Managers.Instance.DestoryCoroutine(go, EFFECT_MUZZLE_DESTROY_TIME);

        }

    }
}

