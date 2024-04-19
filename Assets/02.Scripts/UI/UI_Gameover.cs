using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Gameover : UI_Base
{
    [SerializeField] private GameObject _menuView;
    [SerializeField] private GameObject _settingView;
    [SerializeField] private GameObject _crosshairView;
    [SerializeField] private GameObject _gameoverView;

    [SerializeField] private TextMeshProUGUI _winnerText;
    [SerializeField] private TextMeshProUGUI _playerNumberText;

    [SerializeField] private Button _continueBtn;

    [SerializeField] private Transform _scoreBoard;

    private void Start() {
        Managers.GameManager.GameoverAction += Gameover;
        _continueBtn.onClick.AddListener(ExitGame);

        
    }

    private void Gameover() {
        _menuView.SetActive(false);
        _settingView.SetActive(false);
        _crosshairView.SetActive(false);
        _gameoverView.SetActive(true);

        Managers.GameManager.UnitSortToRank();
        _winnerText.text = $"Winner is {Managers.GameManager.UnitsList[0].name.ToString()}";

        foreach (var t in Managers.GameManager.BoardChild) {
            t.transform.parent = _scoreBoard;
        }

        Managers.GameManager.BoardSortToRank();
    }
}
