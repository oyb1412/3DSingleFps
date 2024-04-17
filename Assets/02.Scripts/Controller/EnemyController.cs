using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static Define;

public class EnemyController : UnitBase, ITakeDamage {
    private NavMeshAgent _agent;

    public UnitBase TargetUnit { get; private set; }
    private EnemyFov _fov;
    private Collider _collider;
    private bool _isTraceItem;
    public EnemyStatus MyStatus { get { return _status as EnemyStatus; } set { _status = value; } }

    protected override void Awake() {
        base.Awake();
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _fov = GetComponent<EnemyFov>();
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

        if (!TargetUnit && _fov.isTracePlayer()) {
            TargetUnit = _fov.isTracePlayer();
        }

        if(TargetUnit && TargetUnit.IsDead()) {
            TargetUnit = null;
            StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
        }

        if (TargetUnit && !_fov.isTracePlayer() && State == UnitState.Shot) {
            TargetUnit = null;
            StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
        }
    }

    public IEnumerator CoMove(Vector3 pos) {
        _agent.SetDestination(pos);
        ChangeState(UnitState.Move, true);
        while (true) {
            float dir = (new Vector3(pos.x, 0f, pos.z)
                - new Vector3(transform.position.x, 0f, transform.position.z)).magnitude;

            if (TargetUnit) {
                Debug.Log("유닛 발견");
                _agent.SetDestination(transform.position);
                transform.LookAt(TargetUnit.transform.position);
                ChangeState(UnitState.Shot, true);
                StopAllCoroutines();
                break;
            }

            if(IsDead()) {
                _agent.SetDestination(transform.position);
                StopAllCoroutines();
                break;
            }

            if(CollideItem != null && !_isTraceItem) {
                if (!_isTraceItem) {
                    Debug.Log("아이템 추적");
                    _isTraceItem = true;
                    StartCoroutine(CoMove(CollideItem.MyTransform.position));
                    break;
                }
            }
            if (dir < 0.2f) {
                if(_isTraceItem && CollideItem != null) {
                    Debug.Log("아이템 습득");
                    CollideItem.Pickup(this);
                    _isTraceItem = false;
                }
                Debug.Log("다음장소 추적");
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
        }
    }

    protected override void IsDeadEvent(Transform attackerTrans) {
        base.IsDeadEvent(attackerTrans);
        transform.LookAt(attackerTrans);
        TargetUnit = null;
        _fov.IsDead = true;
        _collider.enabled = false;
    }

    public override void Init() {
        base.Init();
        _collider.enabled = true;
        UnitRotate = Quaternion.identity;
        WeaponInit();
        ChangeState(UnitState.Shot, false);
        ChangeState(UnitState.Idle);
        StopAllCoroutines();
        _fov.IsDead = false;
        CollideItem = null;
        transform.position = Managers.RespawnManager.GetRespawnPosition();
        StartCoroutine(CoMove(Managers.RespawnManager.GetRandomPosition()));
    }
}
