using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Define;
using static UnityEngine.UI.CanvasScaler;

public class EnemyController : UnitBase, ITakeDamage {
    private NavMeshAgent _agent;

    public int Level { get; private set; } = (int)EnemyLevel.Middle;
    public string Name { get; private set; }
    [field:SerializeField] public UnitBase TargetUnit { get; private set; }
    private Collider _collider;
    private bool _isTraceItem;
    [field: SerializeField] public bool IsShotState { get; set; }
    [SerializeField] float _viewRange;
    public EnemyStatus MyStatus { get { return _status as EnemyStatus; } set { _status = value; } }

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
        _agent.speed = MyStatus._moveSpeed;
    }

    private void Start() {
        Invoke("StartMove", Managers.GameManager.WaitTime);
    }
    private void StartMove() {
        StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
    }

    private void Update() {
        if (!Managers.GameManager.InGame())
            return;

        if (State == UnitState.Dead)
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

        if (TargetUnit && !SearchUnit()/* && State == UnitState.Shot*/) {
            TargetUnit = null;
            IsShotState = false;
            StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
            return;
        }

        //if (TargetUnit && !TargetUnit.IsDead() && _fov.isTracePlayer()) {
        //    var target = _fov.isTracePlayer();
        //    if (TargetUnit is not PlayerController && target is PlayerController) {
        //        TargetUnit = target;
        //        IsShotState = true;
        //        StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
        //    }
        //}

        if (IsShotState && TargetUnit && !TargetUnit.IsDead() && _state != UnitState.Reload) {
            if (_currentWeapon.TryShot(this)) {
                _agent.SetDestination(transform.position);
                State = UnitState.Shot;
            }
        }
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
        State = UnitState.Move;
        while (true) {
            float dir = (new Vector3(pos.x, 0f, pos.z)
                - new Vector3(transform.position.x, 0f, transform.position.z)).magnitude;

            if (TargetUnit) {
                _agent.SetDestination(transform.position);
                transform.LookAt(TargetUnit.transform.position);
                IsShotState = true;
                StopAllCoroutines();
                break;
            }

            if(IsDead()) {
                _agent.SetDestination(transform.position);
                State = UnitState.Dead;
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
        if(TargetUnit == null) {
            TargetUnit = attackerTrans.GetComponent<UnitBase>();
            transform.LookAt(attackerTrans);
            IsShotState = true;
        }
    }

    protected override void IsDeadEvent(Transform attackerTrans) {
        if (attackerTrans.GetComponent<PlayerController>()) {
            ShareSfxController.instance.SetShareSfx(ShareSfx.KillSound);
        }
        base.IsDeadEvent(attackerTrans);
         transform.LookAt(attackerTrans);
        TargetUnit = null;
        _collider.enabled = false;
    }

    public override void Init() {
        base.Init();
        _collider.enabled = true;
        UnitRotate = Quaternion.identity;
        WeaponInit();
        StopAllCoroutines();
        CollideItem = null;
        State = UnitState.Idle;
        StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
    }
}
