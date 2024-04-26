using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using TMPro;

public class UI_Base : MonoBehaviour {
    protected PlayerController _player;
    protected Color _defaultColor;
    protected Vector3 _defaultScale;
    protected string _name;
    protected Coroutine _currentCoroutine;

    protected virtual void Awake() {
        
    }

    protected void StopCoroutine() {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);
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

    protected IEnumerator Co_ImageGradualInvisible(Image image, float speed = 1f, float r = 1f, float g = 1f, float b = 1f, float startAlpha = 1f) {
        float alpha = startAlpha;
        image.gameObject.SetActive(true);
        while (alpha > 0) {
            alpha -= Time.deltaTime * speed;
            image.color = new Color(r, g, b, alpha);
            yield return null;
        }
        image.gameObject.SetActive(false);
    }

    protected IEnumerator Co_TextGradualInvisible(TextMeshProUGUI text, string logo, float speed = 1f, float r = 1f, float g = 1f, float b = 1f) {
        float alpha = 1f;
        text.gameObject.SetActive(true);
        text.text = logo;
        while (alpha > 0) {
            alpha -= Time.deltaTime * speed;
            text.color = new Color(r, g, b, alpha);
            yield return null;
        }
        text.gameObject.SetActive(false);
    }


    protected void SetColorAndScale(Button btn, string name, string objName, Color color, Vector3 scale) {
        if (btn.interactable == false)
            return;

        if (name == objName) {
            btn.targetGraphic.color = color;
            btn.transform.localScale = scale;
        }
    }

    protected void SetColorAndScale(Button btn, string name, string objName, Color color, Vector3 scale, UnityAction call) {
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