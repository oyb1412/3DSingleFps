using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Define;

public class EnemyController : UnitBase, ITakeDamage {
    private NavMeshAgent _agent;

    [SerializeField] private float _searchRange;
    public int Level { get; private set; } = (int)EnemyLevel.Middle;
    public string Name { get; private set; }
    [field:SerializeField] public UnitBase TargetUnit { get; private set; }
    private Collider _collider;
    private bool _isTraceItem;
    [field: SerializeField] public bool IsShotState { get; set; }
    [SerializeField] float _viewRange;

    public Enemy.WeaponController EnemyWeapon { get { return _currentWeapon as Enemy.WeaponController; } }
    public void Create(Vector3 pos, string name, int level) {
        transform.position = pos;
        gameObject.name = name;
        Name = name;
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

        if (!_agent.enabled)
            return;

        UnitRotate = transform.rotation;

        if (!TargetUnit && SearchUnit()) {
            TargetUnit = SearchUnit();
            IsShotState = true;
        }

        if (TargetUnit && !TargetUnit.IsDead() && SearchUnit()) {
            IsShotState = true;
        }

        if (TargetUnit && TargetUnit.IsDead()) {
            TargetUnit = null;
            IsShotState = false;
            StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
            return;
        }
        //move && shotstate
        if (TargetUnit && !SearchUnit()/* && State == UnitState.Shot*/) {
            TargetUnit = null;
            IsShotState = false;
            StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
            return;
        }

        if(IsShotState) {
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

        //if (IsShotState && TargetUnit && !TargetUnit.IsDead() && _state != UnitState.Reload) {
        //    if (_currentWeapon.TryShot(this)) {
        //        _agent.SetDestination(transform.position);
        //        ChangeState(UnitState.Shot);
        //    }
        //}
    }

    private UnitBase SearchUnit() {
        var units = Managers.GameManager.UnitsList;
        foreach(var unit in units) {
            Vector3 dir = (unit.transform.position - transform.position).normalized;
            float product = Vector3.Dot(transform.forward, dir);
            float angle = Mathf.Cos(_viewRange * 0.5f * Mathf.Deg2Rad);
            if(product >= angle) {
                int mask = (1 << (int)LayerType.Unit) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Wall);
                Debug.DrawRay(FirePoint.position, dir * 100f, Color.red);
                bool hit = Physics.Raycast(FirePoint.position, dir, out var target, float.MaxValue, mask);

                if (!hit)
                    continue;

                if (target.collider.gameObject.layer == (int)LayerType.Obstacle ||
                    target.collider.gameObject.layer == (int)LayerType.Wall)
                    continue;

                if (target.collider.gameObject.layer == (int)LayerType.Unit) {
                    TargetUnit = unit;
                    return TargetUnit;
                }
            }
        }
        return null;
    }

    public IEnumerator CoMove(Vector3 pos) {
        _agent.SetDestination(pos);
        BaseWeapon.Animator.ResetTrigger("Shot");
        _state = UnitState.Move;
        IsShotState = false;
        ChangeState(UnitState.Move);
        while (true) {
            float dir = (new Vector3(pos.x, 0f, pos.z)
                - new Vector3(transform.position.x, 0f, transform.position.z)).magnitude;

            if (TargetUnit) {
                _agent.SetDestination(transform.position);
                transform.LookAt(TargetUnit.TargetPos);
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
            if (dir < 0.2f) {
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
        base.IsHitEvent(damage, attackerTrans, myTrans);

        if (TargetUnit == null) {
            TargetUnit = attackerTrans.GetComponentInParent<UnitBase>();
            transform.LookAt(TargetUnit.TargetPos);
            IsShotState = true;
        }
    }

    protected override void IsDeadEvent(Transform attackerTrans, bool headShot) {
        if (attackerTrans.parent.TryGetComponent<PlayerController>(out var player)) {
            ShareSfxController.instance.SetShareSfx(ShareSfx.KillSound);
            DirType dir = Util.DirectionCalculation(attackerTrans, transform);
            player.KillAndDeadEvent.Invoke(dir, name, true, headShot);
        }
        base.IsDeadEvent(attackerTrans, headShot);
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

                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);
                break;
            case UnitState.Move:
                if (_state == UnitState.Dead || _state == UnitState.Reload || _state == UnitState.Shot)
                    return;

                Model.Animator.SetBool("Move", true);
                BaseWeapon.Animator.SetBool("Move", true);
                break;
            case UnitState.Shot:
                if (_state == UnitState.Reload || _state == UnitState.Dead)
                    return;

                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);
                Model.Animator.SetTrigger($"Shot{BaseWeapon.Type.ToString()}");
                BaseWeapon.Animator.SetTrigger("Shot");
                break;
            case UnitState.Reload:
                if (_state == UnitState.Reload || _state == UnitState.Dead)
                    return;

                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);
                Model.Animator.SetTrigger("Reload");
                BaseWeapon.Animator.SetTrigger("Reload");
                break;
            case UnitState.Dead:
                if (_state == UnitState.Dead)
                    return;

                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);
                Model.Animator.SetTrigger("Dead");
                BaseWeapon.Animator.SetTrigger("Dead");
                break;
            case UnitState.Get:
                if (_state == UnitState.Get)
                    return;

                Model.Animator.SetBool("Move", false);
                BaseWeapon.Animator.SetBool("Move", false);
                Model.Animator.SetTrigger("Get");
                BaseWeapon.Animator.SetTrigger("Get");
                break;
        }
        _state = state;
    }

    
}
