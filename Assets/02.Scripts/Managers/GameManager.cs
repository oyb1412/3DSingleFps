using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager
{
    public Action WaitStateEvent;
    public Action FightStateEvent;
    public Action GameoverAction;
    public Action<int> EnemyNumberAction;
    public float GameTime { get; private set; }
    public float RespawnTime { get; private set; }
    public int EnemyNumber { get; private set; }
    public int EnemyLevel { get; private set; }
    public int KillLimit { get; private set; }
    public GameState State { get; private set; } = GameState.None;

    public Action<float> VolumeEvent;
    public float WaitTime { get; private set; }

    private float _doNextStateTime;

    private Transform _scoreBoardTransform;
    public Transform KillFeedParent { get; private set; }

    private List<UnitBase> _units = new List<UnitBase>();
    public List<UnitBase> UnitsList => _units;

    private List<UI_Scoreboard_Child> _boardChild = new List<UI_Scoreboard_Child>();

    public List<UI_Scoreboard_Child> BoardChild => _boardChild;

    public void Init()
    {
        GameTime = DEFAULT_GAMETIME;
        RespawnTime = DEFAULT_RESPAWNTIME;
        EnemyNumber = 0;
        EnemyLevel = (int)Define.EnemyLevel.Middle;
        _doNextStateTime = DEFAULT_GAMESTARTTIME;

        int test = PlayerPrefs.GetInt(PLAYERPREFS_ENEMYNUMBER);

        if(test != 0) {
            EnemyNumber = PlayerPrefs.GetInt(PLAYERPREFS_ENEMYNUMBER);
            EnemyLevel = PlayerPrefs.GetInt(PLAYERPREFS_ENEMYLEVEL);
            GameTime *= PlayerPrefs.GetInt(PLAYERPREFS_TIMELIMIT);
            RespawnTime = PlayerPrefs.GetInt(PLAYERPREFS_RESPAWNTIME);
            KillLimit = PlayerPrefs.GetInt(PLAYERPREFS_KILLLIMIT);
        }

        EnemyNumberAction?.Invoke(EnemyNumber);

        PlayerPrefs.DeleteAll();

        KillFeedParent = GameObject.Find(NAME_KILLFEEDS).transform;
        if (_scoreBoardTransform == null) {
            var uiScoreBoard = GameObject.Find(NAME_UI_SCOREBOARD);
            _scoreBoardTransform = Util.FindChild(uiScoreBoard, NAME_SCOREBOARD, true).transform;
        }
        _scoreBoardTransform.parent.gameObject.SetActive(true);
        


        for (int i = 0; i< EnemyNumber; i++) {
            EnemyController go = Managers.Resources.Instantiate(ENEMY_PATH, null).GetComponent<EnemyController>();
            Vector3 ranPos = Managers.RespawnManager.GetRespawnPosition();
            go.Create(ranPos, ENEMY_NAME[i], EnemyLevel);
            UI_Scoreboard_Child child2 = Managers.Resources.Instantiate(NAME_UI_SCOREBOARDCHILD, _scoreBoardTransform).GetComponent<UI_Scoreboard_Child>();
            child2.Init(go.name, go, Color.gray);
            _boardChild.Add(child2);
            _units.Add(go);
        }

        _scoreBoardTransform.parent.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        ChangeState(GameState.WaitFight);

        VolumeEvent -= SetVolume;
        VolumeEvent += SetVolume;
    }

    public void SetPlayer() {
        if (_scoreBoardTransform == null) {
            var uiScoreBoard = GameObject.Find(NAME_UI_SCOREBOARD);
            _scoreBoardTransform = Util.FindChild(uiScoreBoard, NAME_SCOREBOARD, true).transform;
        }

        PlayerController player = GameObject.Find(NAME_PLAYER).GetComponent<PlayerController>();
         UI_Scoreboard_Child child = Managers.Resources.Instantiate(NAME_UI_SCOREBOARDCHILD, _scoreBoardTransform).GetComponent<UI_Scoreboard_Child>();
         child.Init(player.name, player, Color.green);
         _units.Add(player);
         _boardChild.Add(child);
    }


    public void Clear() {
        WaitStateEvent = null;
        FightStateEvent = null;
        GameoverAction = null;
        VolumeEvent = null;
        _units?.Clear();
        _boardChild?.Clear();
        ChangeState(GameState.None);
    }

    private void SetVolume(float volume) {
        float realVolume = volume * 0.01f;
        foreach(var t in _units) {
            t.Ufx.ChangeVolume(realVolume);
        }
        BgmController.instance.ChangeVolume(realVolume);
        PersonalSfxController.instance.ChangeVolume(realVolume);
    }

    public void BoardSortToRank() {
        _boardChild.Sort((x,y) => y.UnitBase.MyKill.CompareTo(x.UnitBase.MyKill));

        for(int i = 0; i < _boardChild.Count; i++) {
            _boardChild[i].transform.SetSiblingIndex(i + 1);
            _boardChild[i].SetRank(i + 1);
        }
    }

    public void UnitSortToRank() {
        _units.Sort((x, y) => y.MyKill.CompareTo(x.MyKill));
    }

    public void Update()
    {
        switch(State) {
            case GameState.WaitFight:
                WaitTime -= Time.deltaTime;
                if(WaitTime <= 0f) {
                    ChangeState(GameState.StartFight);
                }
                break;
            case GameState.StartFight:
                GameTime -= Time.deltaTime;
                if(GameTime <= 0f) {
                    ChangeState(GameState.Gameover);
                }
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
                Time.timeScale = 1f;
                break;
            case GameState.Gameover:
                GameoverAction?.Invoke();
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.Confined;
                BgmController.instance.SetBgm(Bgm.Ingame, false);
                PersonalSfxController.instance.SetShareSfx(ShareSfx.Gameover);
                break;
            case GameState.Menu:
            case GameState.Setting:
                Time.timeScale = 0f;
                break;
        }
        State = state;
    }

    public bool InGame() {
        return State == GameState.StartFight;
    }


}
