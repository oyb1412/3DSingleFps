using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_SystemSetting : UI_Base
{
    [SerializeField] private GameObject _startView;
    [SerializeField] private GameObject _settingView;

    [SerializeField] private Button _backBtn;
    [SerializeField] private Button _startBtn;

    [SerializeField] private TMP_Dropdown _enemyNumberDP;
    [SerializeField] private TMP_Dropdown _enemyLevelDP;
    [SerializeField] private TMP_Dropdown _timeLimitDP;
    [SerializeField] private TMP_Dropdown _respawnTimeDP;
    [SerializeField] private TMP_Dropdown _killLimitDP;
    private enum BtnType {
        BackBG,
        StartBG,
    }
    protected override void Init() {
    }

    private void Start() {
        _defaultColor = _startBtn.targetGraphic.color;
        _defaultScale = _startBtn.transform.localScale;
    }

    public override void OnEnterButton(BaseEventData eventData) {
        base.OnEnterButton(eventData);

        PointerEventData data = eventData as PointerEventData;
        _name = data.pointerCurrentRaycast.gameObject.name;
        Debug.Log($"클릭한 객체의 이름은 {_name}");

        Color color = new Color(.6f, .8f, 1f, 1f);
        Vector3 scale = new Vector3(1.05f, 1.05f, 1.05f);

        SetColorAndScale(_backBtn, _name, BtnType.BackBG.ToString(), color, scale);
        SetColorAndScale(_startBtn, _name, BtnType.StartBG.ToString(), color, scale);
    }

    public override void OnExitButton(BaseEventData eventData) {
        base.OnExitButton(eventData);

        SetColorAndScale(_backBtn, _name, BtnType.BackBG.ToString(), _defaultColor, _defaultScale);
        SetColorAndScale(_startBtn, _name, BtnType.StartBG.ToString(), _defaultColor, _defaultScale);
        _name = string.Empty;
    }

    public override void OnPressUpButton() {
        base.OnPressUpButton();

        SetColorAndScale(_backBtn, _name, BtnType.BackBG.ToString(), _defaultColor, _defaultScale, Back);
        SetColorAndScale(_startBtn, _name, BtnType.StartBG.ToString(), _defaultColor, _defaultScale, GameStart);

        _name = string.Empty;
    }

    private void GameStart() {
        PlayerPrefs.SetInt("EnemyNumber", _enemyNumberDP.value + 1);
        PlayerPrefs.SetInt("EnemyLevel", _enemyLevelDP.value);
        PlayerPrefs.SetInt("TimeLimit", _timeLimitDP.value + 1);
        PlayerPrefs.SetInt("RespawnTime", _respawnTimeDP.value + 3);
        PlayerPrefs.SetInt("KillLimit", _killLimitDP.value * 50);

        Managers.Scene.LoadScene(Define.SceneType.InGame);
    }

    private void Back() {
        _startView.SetActive(true);
        _settingView.SetActive(false);
    }
}
