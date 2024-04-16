using System;
using System.Collections;
using UnityEngine;
using static Define;

public class GameManager
{
    public Action WaitStateEvent;
    public Action FightStateEvent;
    public float GameTime { get; private set; }
    public GameState State { get; private set; } = GameState.None;

    public float WaitTime { get; private set; }

    private float _doNextStateTime = 0f;
    public void Init()
    {
        
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
