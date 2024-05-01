using UnityEngine;
using static Define;

public class EnemyStateMachine {
    public EnemyStateMachine(EnemyController enemy) {
        _enemy = enemy;
        ChangeState(EnemyState.Idle);
    }
    

    private EnemyState _currentEnemyState = EnemyState.None;
    private EnemyController _enemy;

    public void ChangeState(EnemyState state) {
        if (_currentEnemyState == state)
            return;

        switch(state) {
            case EnemyState.Idle:
                _enemy.EnemyStart();
                break;
            case EnemyState.Patrol:
                _enemy.StartPatrol();
                break; 
            case EnemyState.Search:
                _enemy.SearchUnit(true);
                break;
        }
        _currentEnemyState = state;
    }

    public void Update() {
        if (!Managers.GameManager.InGame())
            return;

        if (_enemy.State == UnitState.Dead)
            return;

        switch (_currentEnemyState) {
            case EnemyState.Patrol:
                _enemy.Patrol();
                break;
            case EnemyState.Search:
                _enemy.Attack();
                break;
        }
    }
}