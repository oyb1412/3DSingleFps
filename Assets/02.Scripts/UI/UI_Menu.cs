using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static Define;

public class UI_Menu : UI_Base
{
    [SerializeField] private GameObject _menuView;
    [SerializeField] private GameObject _settingView;
    [SerializeField] private GameObject _crosshairView;

    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _settingBtn;
    [SerializeField] private Button _exitBtn;

    private enum BtnType {
        ResumeBG,
        SettingBG,
        ExitBG,
    }

    protected override void Init() {
        base.Init();

        _player.MenuEvent += ActiveMenuView;
        _defaultColor = _resumeBtn.targetGraphic.color;
        _defaultScale = _resumeBtn.transform.localScale;
    }

    public override void OnEnterButton(BaseEventData eventData) {
        base.OnEnterButton(eventData);
        PointerEventData data = eventData as PointerEventData;
        _name = data.pointerCurrentRaycast.gameObject.name;

        Color color = UI_ENTER_COLOR;
        Vector3 scale = UI_ENTER_SIZE;

        SetColorAndScale(_resumeBtn, _name, BtnType.ResumeBG.ToString(), color, scale);
        SetColorAndScale(_settingBtn, _name, BtnType.SettingBG.ToString(), color, scale);
        SetColorAndScale(_exitBtn, _name, BtnType.ExitBG.ToString(), color, scale);
    }

    public override void OnExitButton(BaseEventData eventData) {
        base.OnExitButton(eventData);

        SetColorAndScale(_resumeBtn, _name, BtnType.ResumeBG.ToString(), _defaultColor, _defaultScale);
        SetColorAndScale(_settingBtn, _name, BtnType.SettingBG.ToString(), _defaultColor, _defaultScale);
        SetColorAndScale(_exitBtn, _name, BtnType.ExitBG.ToString(), _defaultColor, _defaultScale);

        _name = string.Empty;
    }

    public override void OnPressUpButton() {
        base.OnPressUpButton();

        SetColorAndScale(_resumeBtn, _name, BtnType.ResumeBG.ToString(), _defaultColor, _defaultScale, ActiveMenuView);
        SetColorAndScale(_settingBtn, _name, BtnType.SettingBG.ToString(), _defaultColor, _defaultScale, OnSettingUI);
        SetColorAndScale(_exitBtn, _name, BtnType.ExitBG.ToString(), _defaultColor, _defaultScale, ExitGame);

        _name = string.Empty;
    }

    private void OnSettingUI() {
        _menuView.SetActive(false);
        _settingView.SetActive(true);
        Managers.GameManager.ChangeState(Define.GameState.Setting);
    }

    private void ActiveMenuView() {
        if(_menuView.activeInHierarchy) {
            _menuView.SetActive(false);
            _settingView.SetActive(false);
            _crosshairView.SetActive(true);
            Managers.GameManager.ChangeState(Define.GameState.StartFight);
            Cursor.lockState = CursorLockMode.Locked;

        } else {
            _menuView.SetActive(true);
            _crosshairView.SetActive(false);
            Managers.GameManager.ChangeState(Define.GameState.Menu);
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
