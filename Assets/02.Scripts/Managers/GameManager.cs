using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class GameManager
{
    private readonly string[] ENEMY_NAME = new string[] { "James", "Aaron", "Peyton", "London", "Daniel", "Aiden", "Jackson","Lucas", "Samuel","Luke"};
    public Action WaitStateEvent;
    public Action FightStateEvent;
    public Action GameoverAction;
    public Action<int> EnemyNumberAction;
    //public float GameTime { get; private set; } = 60f;
    public float GameTime { get; private set; } = 300f;
    public float RespawnTime { get; private set; } = 3f;
    public int EnemyNumber { get; private set; }
    public int EnemyLevel { get; private set; } = 1;
    public int KillLimit { get; private set; } = 0;
    public GameState State { get; private set; } = GameState.None;

    public Action<float> VolumeEvent;
    public float Volume { get; set; } = 50f;
    public float WaitTime { get; private set; }

    private float _doNextStateTime = 3f;
    private Transform _scoreBoardTransform;
    public Transform KillFeedParent { get; private set; }

    private List<UnitBase> _units = new List<UnitBase>();
    public List<UnitBase> UnitsList => _units;

    private List<UI_Scoreboard_Child> _boardChild = new List<UI_Scoreboard_Child>();

    public List<UI_Scoreboard_Child> BoardChild => _boardChild;

    public void Init()
    {
        GameTime = 60f;
        RespawnTime = 3f;
        EnemyNumber = 0;
        EnemyLevel = 1;
        Volume = 50f;
        _doNextStateTime = 3f;

        int test = PlayerPrefs.GetInt("EnemyNumber");

        if(test != 0) {
            EnemyNumber = PlayerPrefs.GetInt("EnemyNumber");
            EnemyLevel = PlayerPrefs.GetInt("EnemyLevel");
            GameTime *= PlayerPrefs.GetInt("TimeLimit");
            RespawnTime = PlayerPrefs.GetInt("RespawnTime");
            KillLimit = PlayerPrefs.GetInt("KillLimit");
        }


        EnemyNumberAction?.Invoke(EnemyNumber);

        PlayerPrefs.DeleteAll();

        KillFeedParent = GameObject.Find("KIllFeeds").transform;
        if (_scoreBoardTransform == null) {
            var uiScoreBoard = GameObject.Find("UI_Scoreboard");
            _scoreBoardTransform = Util.FindChild(uiScoreBoard, "Scoreboard", true).transform;
        }
        _scoreBoardTransform.parent.gameObject.SetActive(true);
        


        for (int i = 0; i< EnemyNumber; i++) {
            EnemyController go = Managers.Resources.Instantiate("Unit/Enemy", null).GetComponent<EnemyController>();
            Vector3 ranPos = Managers.RespawnManager.GetRespawnPosition();
            go.Create(ranPos, ENEMY_NAME[i], EnemyLevel);
            UI_Scoreboard_Child child2 = Managers.Resources.Instantiate("UI/ScoreboardChild", _scoreBoardTransform).GetComponent<UI_Scoreboard_Child>();
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

    public void SetPlayer(PlayerController networkPlayer = null) {
        if (_scoreBoardTransform == null) {
            var uiScoreBoard = GameObject.Find("UI_Scoreboard");
            _scoreBoardTransform = Util.FindChild(uiScoreBoard, "Scoreboard", true).transform;
        }

        if (networkPlayer != null) {
            UI_Scoreboard_Child child = PhotonNetwork.Instantiate("Prefabs/UI/ScoreboardChild", Vector2.zero, Quaternion.identity).GetComponent<UI_Scoreboard_Child>();
            child.transform.parent = _scoreBoardTransform;
            child.Init(networkPlayer.name, networkPlayer, Color.green);
            _units.Add(networkPlayer);
            _boardChild.Add(child);
        } else {
            PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
            UI_Scoreboard_Child child = Managers.Resources.Instantiate("UI/ScoreboardChild", _scoreBoardTransform).GetComponent<UI_Scoreboard_Child>();
            child.Init(player.name, player, Color.green);
            _units.Add(player);
            _boardChild.Add(child);
        }
    }

    [PunRPC]
    public void SavePlayersInfo() {

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
        ShareSfxController.instance.ChangeVolume(realVolume);
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
                ShareSfxController.instance.SetShareSfx(ShareSfx.Gameover);
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
