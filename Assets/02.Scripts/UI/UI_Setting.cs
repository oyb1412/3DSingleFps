using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Define;

public class UI_Setting : UI_Base
{
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Slider _sensitivitySlider;

    [SerializeField] private TextMeshProUGUI _sensitivityText;

    [SerializeField] private Button _confirmBtn;
    [SerializeField] private Button _closeBtn;

    [SerializeField] private GameObject _menuView;
    [SerializeField] private GameObject _settingView;
    [SerializeField] private GameObject _crosshairView;

    [SerializeField] private TMP_Dropdown _outlineDP;
    [SerializeField] private TMP_Dropdown _crosshairDP;

    [SerializeField] private Image _crosshairIconImage;

    [SerializeField] private Sprite[] _crosshairIconSprites;

    private enum BtnType {
        ConfirmBG,
        CloseBG,
    }


    protected override void Init() {
        base.Init();

        _sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        _volumeSlider.onValueChanged.AddListener(SetVolume);
        _outlineDP.onValueChanged.AddListener(SetOutlineColor);
        _crosshairDP.onValueChanged.AddListener(SetCrosshair);

        _player.SettingEvent += ActiveMenuView;

        _defaultColor = _confirmBtn.targetGraphic.color;
        _defaultScale = _confirmBtn.transform.localScale;
    }

    private void SetOutlineColor(int value) {
        var units = GameManager.Instance.UnitsList;
        Color color = Color.red;

        switch (value) 
        {
            case (int)OutlineColorType.Red:
                color = Color.red;
                break;
            case (int)OutlineColorType.Yellow:
                color = Color.yellow;
                break;
            case (int)OutlineColorType.Green:
                color = Color.green;
                break;
        }

        foreach(var u in units) {
            u.SetOutlineColor(color);
        }
    }

    private void SetCrosshair(int value) {
        _crosshairIconImage.sprite = _crosshairIconSprites[value];
        _player.ChangeCrosshairEvent?.Invoke(value);
    }

    private void SetVolume(float value) {
        GameManager.Instance.Volume = value;
        GameManager.Instance.VolumeEvent.Invoke(value);
    }

    private void SetSensitivity(float value) {
        _player.Sensitiv = value;
        _sensitivityText.text = ((int)value).ToString();
    }

    public override void OnEnterButton(BaseEventData eventData) {
        base.OnEnterButton(eventData);

        PointerEventData data = eventData as PointerEventData;
        _name = data.pointerCurrentRaycast.gameObject.name;

        Color color = new Color(0f, 0f, 0f, .5f);
        Vector3 scale = new Vector3(.95f, .95f, .95f);

        SetColorAndScale(_confirmBtn, _name, BtnType.ConfirmBG.ToString(), color, scale);
        SetColorAndScale(_closeBtn, _name, BtnType.CloseBG.ToString(), color, scale);
    }

    public override void OnExitButton(BaseEventData eventData) {
        base.OnExitButton(eventData);

        SetColorAndScale(_confirmBtn, _name, BtnType.ConfirmBG.ToString(), _defaultColor, _defaultScale);
        SetColorAndScale(_closeBtn, _name, BtnType.CloseBG.ToString(), _defaultColor, _defaultScale);

        _name = string.Empty;
    }

    public override void OnPressUpButton() {
        base.OnPressUpButton();

        SetColorAndScale(_confirmBtn, _name, BtnType.ConfirmBG.ToString(), _defaultColor, _defaultScale, ActiveMenuView);
        SetColorAndScale(_closeBtn, _name, BtnType.CloseBG.ToString(), _defaultColor, _defaultScale, ActiveMenuView);

        _name = string.Empty;
    }

    private void ActiveMenuView() {
        _crosshairView.SetActive(true);
        _settingView.SetActive(false);
        GameManager.Instance.ChangeState(Define.GameState.StartFight);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
