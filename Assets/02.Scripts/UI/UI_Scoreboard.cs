using TMPro;
using UnityEngine;
using static Define;

public class UI_Scoreboard : UI_Base
{
    [SerializeField] private GameObject _scoreBoard;
    [SerializeField] private TextMeshProUGUI _playerNumber;
    [SerializeField] private UI_Scoreboard_Child _scoreBoardChild;

    protected override void Init() {
        base.Init();

        _player.ShowScoreboardEvent -= ((trigger) => transform.GetChild(0).gameObject.SetActive(trigger));
        _player.ShowScoreboardEvent += ((trigger) => transform.GetChild(0).gameObject.SetActive(trigger));
        Managers.GameManager.EnemyNumberAction += SetEnemyNumber;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void SetEnemyNumber(int number) {
        _playerNumber.text = string.Format(MENT_PLAYERS, (number + 1));
    }
}
