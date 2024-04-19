using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private void Start() {
        _confirmBtn.onClick.AddListener(ActiveMenuView);
        _closeBtn.onClick.AddListener(ActiveMenuView);
        _player.SettingEvent += ActiveMenuView;
    }

    private void ActiveMenuView() {
        _crosshairView.SetActive(true);
        _settingView.SetActive(false);
        Managers.GameManager.ChangeState(Define.GameState.StartFight);
        Cursor.lockState = CursorLockMode.Locked;
    }
}
