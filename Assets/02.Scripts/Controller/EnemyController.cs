using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Define;

public class EnemyController : UnitBase, ITakeDamage {
    private NavMeshAgent _agent;
    public int Level { get; private set; } = (int)EnemyLevel.Middle;
    [field:SerializeField]public UnitBase TargetUnit { get; set; }
    private Collider _collider;
    [SerializeField]private bool _isTraceItem;
    [field: SerializeField] public bool IsShotState { get; set; }
    [SerializeField] float _viewRange;

    public void Create(Vector3 pos, string name, int level) {
        transform.position = pos;
        gameObject.name = name;
        Level = level;
    }
    protected override void Awake() {
        base.Awake();
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _moveSpeed;
        _agent.enabled = false;
    }

    private void Start() {
        Invoke("StartMove", Managers.GameManager.WaitTime);
    }

    private void StartMove() {
        _agent.enabled = true;
        StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
    }

    private void Update() {
        if (!Managers.GameManager.InGame())
            return;

        if (State == UnitState.Dead)
            return;

        //적 미발견 상태에서 적 발견 및 공격 가능
        if (!TargetUnit && SearchUnit()) {
            TargetUnit = SearchUnit();
            IsShotState = true;
        }

        if (TargetUnit && !TargetUnit.IsDead() && SearchUnit()) {
            IsShotState = true;
        }

        //적 발견 상태에서 적 사망
        if (TargetUnit && TargetUnit.IsDead()) {
            TargetUnit = null;
            IsShotState = false;
            StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
            return;
        }

        //적 발견 상태에서 적 시야 밖으로 나감
        if (TargetUnit && !SearchUnit()) {
            TargetUnit = null;
            IsShotState = false;
            StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
            return;
        }

        //공격 상태
        if (IsShotState) {
            if(!TargetUnit || TargetUnit.IsDead()) {
                IsShotState = false;
                StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
                return;
            }
            if(_state != UnitState.Reload) {
                if (_currentWeapon.TryShot(this)) {
                    _agent.SetDestination(transform.position);
                    ChangeState(UnitState.Shot);
                }
            }
        }
    }

    private UnitBase SearchUnit() {
        var units = Managers.GameManager.UnitsList;
        foreach(var unit in units) {
                if (unit.IsDead()) continue;
            Vector3 dir = (unit.transform.position - transform.position).normalized;
            float product = Vector3.Dot(transform.forward, dir);
            float angle = Mathf.Cos(_viewRange * 0.5f * Mathf.Deg2Rad);
            if(product >= angle) {
                int mask = (1 << (int)LayerType.Unit) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Wall);
                Debug.DrawRay(FirePoint.position, dir * float.MaxValue, Color.red);
                bool hit = Physics.Raycast(FirePoint.position, dir, out var target, float.MaxValue, mask);

                if (!hit)
                    continue;

                if (target.collider.gameObject.layer == (int)LayerType.Obstacle ||
                    target.collider.gameObject.layer == (int)LayerType.Wall)
                    continue;

                if (target.collider.gameObject.layer == (int)LayerType.Unit) {
                    return unit;
                }
            }
        }
        return null;
    }

    public IEnumerator CoMove(Vector3 pos) {
        _agent.SetDestination(pos);
        BaseWeapon.Animator.ResetTrigger(ANIMATOR_PARAMETER_SHOT);
        _state = UnitState.Move;
        IsShotState = false;
        ChangeState(UnitState.Move);
        while (true) {
            float dir = (new Vector3(pos.x, 0f, pos.z)
                - new Vector3(transform.position.x, 0f, transform.position.z)).magnitude;

            if (TargetUnit) {
                _agent.SetDestination(transform.position);
                transform.LookAt(TargetUnit.transform);
                IsShotState = true;
                StopAllCoroutines();
                break;
            }

            if(IsDead()) {
                _agent.SetDestination(transform.position);
                ChangeState(UnitState.Dead);
                StopAllCoroutines();
                break;
            }

            if(CollideItem != null && !_isTraceItem) {
                if (!_isTraceItem) {
                    _isTraceItem = true;
                    StartCoroutine(CoMove(CollideItem.MyTransform.position));
                    break;
                }
            }
            if (dir < ENEMY_MOVE_ALLOW_RANGE) {
                if(_isTraceItem && CollideItem != null) {
                    CollideItem.Pickup(this);
                    _isTraceItem = false;
                }
                StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
                break;
            }
            yield return null;
        }
    }

    protected override void IsHitEvent(int damage, Transform attackerTrans, Transform myTrans) {
        if (TargetUnit == null) {
            TargetUnit = attackerTrans.GetComponentInParent<UnitBase>();
            transform.LookAt(TargetUnit.transform);
            IsShotState = true;
        }
    }

    protected override void IsDeadEvent(Transform attackerTrans, bool headShot) {
        base.IsDeadEvent(attackerTrans, headShot);

        if (attackerTrans.TryGetComponent<PlayerController>(out var player)) {
            PersonalSfxController.instance.SetShareSfx(ShareSfx.KillSound);
            DirType dir = Util.DirectionCalculation(attackerTrans, transform);
            player.KillAndDeadEvent.Invoke(dir, name, true, headShot);
        }
         transform.LookAt(attackerTrans);
        TargetUnit = null;
        _collider.enabled = false;
    }

    public override void Init() {
        base.Init();
        _bodyCollider.enabled = true;
        _headCollider.enabled = true;
        _collider.enabled = true;

        StopAllCoroutines();
        ChangeState(UnitState.Idle);
        StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
    }

    public override void ChangeState(UnitState state) {
        switch (state) {
            case UnitState.Idle:
                if (_state == UnitState.Dead || _state == UnitState.Shot)
                    return;

                Model.Animator.SetBool(UnitState.Move.ToString(), false);
                BaseWeapon.Animator.SetBool(UnitState.Move.ToString(), false);
                break;
            case UnitState.Move:
                if (_state == UnitState.Dead || _state == UnitState.Reload || _state == UnitState.Shot)
                    return;

                Model.Animator.SetBool(UnitState.Move.ToString(), true);
                BaseWeapon.Animator.SetBool(UnitState.Move.ToString(), true);
                break;
            case UnitState.Shot:
                if (_state == UnitState.Reload || _state == UnitState.Dead)
                    return;

                Model.Animator.SetBool(UnitState.Move.ToString(), false);
                BaseWeapon.Animator.SetBool(UnitState.Move.ToString(), false);
                Model.Animator.SetTrigger($"{state.ToString()}{BaseWeapon.Type.ToString()}");
                BaseWeapon.Animator.SetTrigger(state.ToString());
                break;
            case UnitState.Reload:
                if (_state == UnitState.Reload || _state == UnitState.Dead)
                    return;

                Model.Animator.SetBool(UnitState.Move.ToString(), false);
                BaseWeapon.Animator.SetBool(UnitState.Move.ToString(), false);
                Model.Animator.SetTrigger(state.ToString());
                BaseWeapon.Animator.SetTrigger(state.ToString());
                break;
            case UnitState.Dead:
                if (_state == UnitState.Dead)
                    return;

                Model.Animator.SetBool(UnitState.Move.ToString(), false);
                BaseWeapon.Animator.SetBool(UnitState.Move.ToString(), false);
                Model.Animator.SetTrigger(state.ToString());
                BaseWeapon.Animator.SetTrigger(state.ToString());
                break;
            case UnitState.Get:
                if (_state == UnitState.Get)
                    return;

                Model.Animator.SetBool(UnitState.Move.ToString(), false);
                BaseWeapon.Animator.SetBool(UnitState.Move.ToString(), false);
                Model.Animator.SetTrigger(state.ToString());
                BaseWeapon.Animator.SetTrigger(state.ToString());
                break;
        }
        _state = state;
    }

    public bool IsDefaultWeapon() {
        return BaseWeapon.Type == WeaponType.Pistol;
    }
}
