using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Scoreboard : UI_Base
{
    [SerializeField] private GameObject _scoreBoard;
    [SerializeField] private TextMeshProUGUI _playerNumber;
    [SerializeField] private UI_Scoreboard_Child _scoreBoardChild;
    public static UI_Base Instance;
    protected override void Init() {


        base.Init();

        _player.ScoreboardEvent -= ((trigger) => transform.GetChild(0).gameObject.SetActive(trigger));
        _player.ScoreboardEvent += ((trigger) => transform.GetChild(0).gameObject.SetActive(trigger));
        transform.GetChild(0).gameObject.SetActive(false);
    }

    protected override void Awake() {
        base.Awake();
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        GameManager.Instance.EnemyNumberAction += SetEnemyNumber;
    }

    private void SetEnemyNumber(int number) {
        _playerNumber.text = $"PLAYERS ({number + 1})";
    }
}
