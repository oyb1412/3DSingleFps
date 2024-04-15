using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Base : MonoBehaviour {
    protected PlayerController _player;

    private  void Awake() {
        Init();
    }

    protected virtual void Init() {
        _player = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    
}