using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static Define;
public class EnemyController : UnitBase, ITakeDamage {
    private EnemyStateMachine _stateMachine;
    private NavMeshAgent _agent;
    private Collider _collider;
    private bool _isTraceItem;
    private bool _isShotState;
    private float _viewRange = ENEMY_DEFAULT_VIEWRANGE;
    public int Level { get; private set; } = (int)EnemyLevel.Middle;
    public UnitBase TargetUnit { get; set; }

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
        _stateMachine = new EnemyStateMachine(this);
    }

    public void EnemyStart() {
        Invoke("StartMove", Managers.GameManager.WaitTime);
    }

    private void StartMove() {
        _agent.enabled = true;
        _stateMachine.ChangeState(EnemyState.Patrol);
    }

    public void SearchUnit(bool trigger) {
        _isShotState = trigger;
    }

    public void StartPatrol() {
        TargetUnit = null;
        _isShotState = false;
        StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
    }

    public void Patrol() {
        if (!TargetUnit && SearchUnit()) {
            TargetUnit = SearchUnit();
            _stateMachine.ChangeState(EnemyState.Search);
        }

        if (TargetUnit && !TargetUnit.IsDead() && SearchUnit()) {
            _stateMachine.ChangeState(EnemyState.Search);
        }
    }

    public void Attack() {
        if (_isShotState) {
            if (_state != UnitState.Reload) {
                if (_currentWeapon.TryShot(this)) {
                    _agent.SetDestination(transform.position);
                    ChangeState(UnitState.Shot);
                }
            }
            if (!TargetUnit || TargetUnit.IsDead()) {
                _stateMachine.ChangeState(EnemyState.Patrol);
                return;
            }
            if (TargetUnit && TargetUnit.IsDead()) {
                _stateMachine.ChangeState(EnemyState.Patrol);
                return;
            }
            if (TargetUnit && !SearchUnit()) {
                _stateMachine.ChangeState(EnemyState.Patrol);
                return;
            }
        }
    }

    private void Update() {
        _stateMachine.Update();
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
        _state = UnitState.Move;
        _isShotState = false;
        BaseWeapon.Animator.ResetTrigger(ANIMATOR_PARAMETER_SHOT);

        ChangeState(UnitState.Move);

        while (true) {
            float dir = (new Vector3(pos.x, 0f, pos.z)
                - new Vector3(transform.position.x, 0f, transform.position.z)).magnitude;

            if (TargetUnit) {
                _agent.SetDestination(transform.position);
                transform.LookAt(TargetUnit.transform);
                _isShotState = true;
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
            _isShotState = true;
        }
    }

    protected override void IsDeadEvent(Transform attackerTrans, bool headShot) {
        if (attackerTrans.TryGetComponent<PlayerController>(out var player)) {
            PersonalSfxController.instance.SetPersonalSfx(PersonalSfx.KillSound);
            DirType dir = Util.DirectionCalculation(attackerTrans, transform);
            player.ShowKillAndDeadText(dir, name, true, headShot);
        }
        transform.LookAt(attackerTrans);
        TargetUnit = null;
        _collider.enabled = false;
        base.IsDeadEvent(attackerTrans, headShot);
    }

    public override void Respawn() {
        base.Respawn();
        _bodyCollider.enabled = true;
        _headCollider.enabled = true;
        _collider.enabled = true;

        StopAllCoroutines();
        ChangeState(UnitState.Idle);
        StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
    }

    private void SetAnimationBool(string parameter, bool trigger) {
        Model.Animator.SetBool(parameter, trigger);
        BaseWeapon.Animator.SetBool(parameter, trigger);
    }

    private void SetAnimationTrigger(string parameter) {
        Model.Animator.SetTrigger(parameter);
        BaseWeapon.Animator.SetTrigger(parameter);
    }

    public void SetCollideItem(IItem item) {
        CollideItem = item;
    }
    public override void ChangeState(UnitState state) {
        switch (state) {
            case UnitState.Idle:
                if (_state == UnitState.Dead || _state == UnitState.Shot)
                    return;

                SetAnimationBool(UnitState.Move.ToString(), false);
                break;
            case UnitState.Move:
                if (_state == UnitState.Dead || _state == UnitState.Reload || _state == UnitState.Shot)
                    return;

                SetAnimationBool(UnitState.Move.ToString(), true);
                break;
            case UnitState.Shot:
                if (_state == UnitState.Reload || _state == UnitState.Dead)
                    return;

                SetAnimationBool(UnitState.Move.ToString(), false);

                Model.Animator.SetTrigger($"{state.ToString()}{BaseWeapon.Type.ToString()}");
                BaseWeapon.Animator.SetTrigger(state.ToString());
                break;
            case UnitState.Reload:
                if (_state == UnitState.Reload || _state == UnitState.Dead)
                    return;

                SetAnimationBool(UnitState.Move.ToString(), false);

                SetAnimationTrigger(state.ToString());
                break;
            case UnitState.Dead:
                if (_state == UnitState.Dead)
                    return;

                SetAnimationBool(UnitState.Move.ToString(), false);

                SetAnimationTrigger(state.ToString());
                break;
            case UnitState.Get:
                if (_state == UnitState.Get)
                    return;

                SetAnimationBool(UnitState.Move.ToString(), false);

                SetAnimationTrigger(state.ToString());
                break;
        }
        _state = state;
    }

    public bool IsDefaultWeapon() {
        return BaseWeapon.Type == WeaponType.Pistol;
    }
}
