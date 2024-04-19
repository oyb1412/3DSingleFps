using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UI_Base : MonoBehaviour {
    protected PlayerController _player;
    protected UnityEngine.Color _defaultColor;
    protected Vector3 _defaultScale;
    protected string _name;
    private  void Awake() {
        Init();
    }

    protected virtual void Init() {
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    protected void ExitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public virtual void PressDownButton(BaseEventData eventData) {

    }

    public virtual void PressUpButton() {

    }
}