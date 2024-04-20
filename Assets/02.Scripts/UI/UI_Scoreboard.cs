using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_Scoreboard : UI_Base
{
    [SerializeField] private GameObject _scoreBoard;
    [SerializeField] private TextMeshProUGUI _playerNumber;
    [SerializeField] private UI_Scoreboard_Child _scoreBoardChild;

    private void Start() {
        _player.ScoreboardEvent -= ((trigger) => transform.GetChild(0).gameObject.SetActive(trigger) );
        _player.ScoreboardEvent += ((trigger) => transform.GetChild(0).gameObject.SetActive(trigger) );
        _playerNumber.text = $"PLAYERS ({Managers.GameManager.EnemyNumber + 1})";
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
