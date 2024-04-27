using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Start : UI_Base
{
    [SerializeField] private Button _singleBtn;
    [SerializeField] private Button _multiBtn;
    [SerializeField] private Button _quitBtn;

    [SerializeField] private GameObject _startView;
    [SerializeField] private GameObject _settingView;

    protected override void Init() {
        base.Init();
        _defaultColor = _singleBtn.targetGraphic.color;
        _defaultScale = _singleBtn.transform.localScale;
    }

    private enum BtnType {
        SingleBG,
        MultiBG,
        QuitBG
    }


    public override void OnEnterButton(BaseEventData eventData) {
        base.OnEnterButton(eventData);

        PointerEventData data = eventData as PointerEventData;
        _name = data.pointerCurrentRaycast.gameObject.name;

        Color color = new Color(.6f, .8f, 1f, 1f);
        Vector3 scale = new Vector3(.95f, .95f, .95f);

        SetColorAndScale(_singleBtn, _name, BtnType.SingleBG.ToString(), color, scale);
        SetColorAndScale(_multiBtn, _name, BtnType.MultiBG.ToString(), color, scale);
        SetColorAndScale(_quitBtn, _name, BtnType.QuitBG.ToString(), color, scale);
    }

    public override void OnExitButton(BaseEventData eventData) {
        base.OnExitButton(eventData);

        SetColorAndScale(_singleBtn, _name, BtnType.SingleBG.ToString(), _defaultColor, _defaultScale);
        SetColorAndScale(_multiBtn, _name, BtnType.MultiBG.ToString(), _defaultColor, _defaultScale);
        SetColorAndScale(_quitBtn, _name, BtnType.QuitBG.ToString(), _defaultColor, _defaultScale);

        _name = string.Empty;
    }

    public override void OnPressUpButton() {
        base.OnPressUpButton();

        SetColorAndScale(_singleBtn, _name, BtnType.SingleBG.ToString(), _defaultColor, _defaultScale, GoSettingUI);
        SetColorAndScale(_multiBtn, _name, BtnType.MultiBG.ToString(), _defaultColor, _defaultScale);
        SetColorAndScale(_quitBtn, _name, BtnType.QuitBG.ToString(), _defaultColor, _defaultScale, ExitGame);

        _name = string.Empty;
    }

    private void GoSettingUI() {
        _startView.SetActive(false);
        _settingView.SetActive(true);
    }
}
