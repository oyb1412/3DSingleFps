using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.Events;

public class UI_Base : MonoBehaviour {
    protected PlayerController _player;
    protected UnityEngine.Color _defaultColor;
    protected Vector3 _defaultScale;
    protected string _name;


    protected virtual void Awake() {
        
    }

    private void Start() {
        Init();
    }

    public void SetPlayer(PlayerController playerController) {
        _player = playerController;
    }

    protected virtual void Init() { }

    protected void ExitGame() {
        Time.timeScale = 1f;
        Managers.Scene.LoadScene(Define.SceneType.Exit);
    }

    protected void RestartGame() {
        Time.timeScale = 1f;
        Managers.Scene.LoadScene(Define.SceneType.Startup);
    }


    protected void SetColorAndScale(UnityEngine.UI.Button btn, string name, string objName, Color color, Vector3 scale) {
        if (btn.interactable == false)
            return;

        if (name == objName) {
            btn.targetGraphic.color = color;
            btn.transform.localScale = scale;
        }
    }

    protected void SetColorAndScale(UnityEngine.UI.Button btn, string name, string objName, Color color, Vector3 scale, UnityAction call) {
        if (btn.interactable == false)
            return;

        if (name == objName) {
            btn.targetGraphic.color = color;
            btn.transform.localScale = scale;
            call.Invoke();
        }
    }

    public virtual void OnEnterButton(BaseEventData eventData) {

    }

    public virtual void OnExitButton(BaseEventData eventData) {

    }

    public virtual void OnPressUpButton() {
        ShareSfxController.instance.SetShareSfx(Define.ShareSfx.Button);
    }
}