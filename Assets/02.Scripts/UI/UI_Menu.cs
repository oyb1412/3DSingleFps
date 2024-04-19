using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class UI_Menu : UI_Base
{
    [SerializeField] private GameObject _menuView;
    [SerializeField] private GameObject _settingView;
    [SerializeField] private GameObject _crosshairView;

    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _settingBtn;
    [SerializeField] private Button _exitBtn;

    public enum BtnType {
        ResumeBG,
        SettingBG,
        ExitBG,
    }
    private void Start() {
        _player.MenuEvent += ActiveMenuView;
        _defaultColor = _resumeBtn.transform.GetChild(1).GetComponent<Image>().color;
        _defaultScale = _resumeBtn.transform.localScale;
    }

    private void OnSettingUI() {
        _menuView.SetActive(false);
        _settingView.SetActive(true);
        Managers.GameManager.ChangeState(Define.GameState.Setting);
    }

    public override void PressDownButton(BaseEventData eventData) {
        base.PressDownButton(eventData);
        PointerEventData data = eventData as PointerEventData;
        _name = data.pointerCurrentRaycast.gameObject.name;
        Debug.Log($"클릭한 객체의 이름은 {_name}");
        if(_name == BtnType.ResumeBG.ToString()) {
            _resumeBtn.transform.GetChild(1).GetComponent<Image>().color = new Color(0f, 0f, 0f, .5f);
            _resumeBtn.transform.localScale = new Vector3(.95f, .95f, .95f);
        }
        else if(_name == BtnType.SettingBG.ToString()) {
            _settingBtn.transform.GetChild(1).GetComponent<Image>().color = new Color(0f, 0f, 0f, .5f);
            _settingBtn.transform.localScale = new Vector3(.95f, .95f, .95f);
        }
        else if(_name == BtnType.ExitBG.ToString()) {
            _exitBtn.transform.GetChild(1).GetComponent<Image>().color = new Color(0f, 0f, 0f, .5f);
            _exitBtn.transform.localScale = new Vector3(.95f, .95f, .95f);
        }
    }

    public override void PressUpButton() {
        base.PressUpButton();
        if (_name == BtnType.ResumeBG.ToString()) {
            _resumeBtn.transform.GetChild(1).GetComponent<Image>().color = _defaultColor;
            _resumeBtn.transform.localScale = _defaultScale;
            ActiveMenuView();
        } else if (_name == BtnType.SettingBG.ToString()) {
            _settingBtn.transform.GetChild(1).GetComponent<Image>().color = _defaultColor;
            _settingBtn.transform.localScale = _defaultScale;
            OnSettingUI();
        } else if (_name == BtnType.ExitBG.ToString()) {
            _exitBtn.transform.GetChild(1).GetComponent<Image>().color = _defaultColor;
            _exitBtn.transform.localScale = _defaultScale;
            ExitGame();
        }

        _name = string.Empty;
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
