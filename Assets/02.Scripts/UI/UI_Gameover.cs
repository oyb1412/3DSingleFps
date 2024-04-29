using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

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
        SetColorAndScale(_continueBtn, _name, NAME_RESTARTBG, Color.black, UI_ENTER_SIZE);
    }

    public override void OnExitButton(BaseEventData eventData) {
        base.OnExitButton(eventData);

        SetColorAndScale(_continueBtn, _name, NAME_RESTARTBG, _defaultColor, _defaultScale);

        _name = string.Empty;
    }

    public override void OnPressUpButton() {
        base.OnPressUpButton();

        SetColorAndScale(_continueBtn, _name, NAME_RESTARTBG, _defaultColor, _defaultScale, RestartGame);

        _name = string.Empty;
    }

    private void Gameover() {
        _menuView.SetActive(false);
        _settingView.SetActive(false);
        _crosshairView.SetActive(false);
        _gameoverView.SetActive(true);
        _playerNumberText.text = string.Format(MENT_PLAYERS, Managers.GameManager.UnitsList.Count);
        Managers.GameManager.UnitSortToRank();
        _winnerText.text = string.Format(WINNERLOGO, Managers.GameManager.UnitsList[0].name.ToString());

        foreach (var t in Managers.GameManager.BoardChild) {
            t.transform.parent = _scoreBoard;
        }

        Managers.GameManager.BoardSortToRank();
    }
}
