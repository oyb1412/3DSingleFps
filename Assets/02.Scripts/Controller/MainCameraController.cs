using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraController : MonoBehaviour
{
    private Vector3 _defaultPos;
    private PlayerController _player;
    private Camera _camera;
    private float _defaultView;

    void Start() {
        _camera = GetComponent<Camera>();
        _defaultView = _camera.fieldOfView;
        _player = GetComponentInParent<PlayerController>();
        _player.AimEvent += AimEvent;
        _defaultPos = transform.localPosition;
    }

    private void AimEvent(bool trigger) {
        if(trigger) {
            transform.localPosition = _player.CurrentWeapon._cameraPos;
            _camera.fieldOfView = _player.CurrentWeapon._cameraView;
        }
        else {
            transform.localPosition = _defaultPos;
            _camera.fieldOfView = _defaultView;
        }
    }
    
}
