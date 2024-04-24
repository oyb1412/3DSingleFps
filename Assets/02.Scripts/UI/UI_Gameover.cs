using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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



    protected override void Init() {
        base.Init();

        Managers.GameManager.GameoverAction += Gameover;
        _defaultColor = _continueBtn.targetGraphic.color;
        _defaultScale = _continueBtn.transform.localScale;
    }

    public override void OnEnterButton(BaseEventData eventData) {
        base.OnEnterButton(eventData);
        PointerEventData data = eventData as PointerEventData;
        _name = data.pointerCurrentRaycast.gameObject.name;

        SetColorAndScale(_continueBtn, _name, "RestartBG", Color.black, new Vector3(.95f, .95f, .95f));
    }

    public override void OnExitButton(BaseEventData eventData) {
        base.OnExitButton(eventData);

        SetColorAndScale(_continueBtn, _name, "RestartBG", _defaultColor, _defaultScale);

        _name = string.Empty;
    }

    public override void OnPressUpButton() {
        base.OnPressUpButton();

        SetColorAndScale(_continueBtn, _name, "RestartBG", _defaultColor, _defaultScale, RestartGame);

        _name = string.Empty;
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
