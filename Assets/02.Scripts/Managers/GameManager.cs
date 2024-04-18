using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager
{
    public const int ENEMY_NUMBER = 5;
    private readonly string[] ENEMY_NAME = new string[] { "James", "Aaron", "Peyton", "London", "Daniel", "Aiden", "Jackson","Lucas", "Samuel","Luke"};
    public Action WaitStateEvent;
    public Action FightStateEvent;
    public float GameTime { get; private set; }
    public GameState State { get; private set; } = GameState.None;

    public float WaitTime { get; private set; }

    private float _doNextStateTime = 5f;
    private Transform _scoreBoardTransform;

    private List<UI_Scoreboard_Child> _boardChild = new List<UI_Scoreboard_Child>();

    public void Init()
    {
        _scoreBoardTransform = GameObject.Find("Scoreboard").transform;
        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        UI_Scoreboard_Child child = Managers.Resources.Instantiate("UI/ScoreboardChild", _scoreBoardTransform).GetComponent<UI_Scoreboard_Child>();
        child.Init(player.name, player, Color.green);
        _boardChild.Add(child);
        for (int i = 0; i< ENEMY_NUMBER; i++) {
            EnemyController go = Managers.Resources.Instantiate("Unit/Enemy", null).GetComponent<EnemyController>();
            go.transform.position = Managers.RespawnManager.GetValidSpawnPosition();
            go.name = ENEMY_NAME[i];
            go.Name = ENEMY_NAME[i];
            UI_Scoreboard_Child child2 = Managers.Resources.Instantiate("UI/ScoreboardChild", _scoreBoardTransform).GetComponent<UI_Scoreboard_Child>();
            child2.Init(go.name, go, Color.gray);
            _boardChild.Add(child2);
        }
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    public void UnitsSortToRank() {
        _boardChild.Sort((x,y) => y.UnitBase.MyKill.CompareTo(x.UnitBase.MyKill));

        for(int i = 0; i < _boardChild.Count; i++) {
            _boardChild[i].transform.SetSiblingIndex(i + 1);
        }
    }

    public void Update()
    {
        switch(State) {
            case GameState.WaitFight:
                WaitTime -= Time.deltaTime;
                if(WaitTime <= 0f) {
                    State = GameState.StartFight;
                }
                break;
            case GameState.StartFight:
                GameTime = (Time.time - _doNextStateTime);
                break;
        }
    }

    public void ChangeState(GameState state) {
        if (State == state)
            return;

        switch(state) {
            case GameState.WaitFight:
                WaitTime = _doNextStateTime;
                WaitStateEvent?.Invoke();
                break;
            case GameState.StartFight:
                FightStateEvent?.Invoke();
                break;
        }

        State = state;
    }

    public bool InGame() {
        return State == GameState.StartFight;
    }


}
