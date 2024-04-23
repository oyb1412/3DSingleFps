using UnityEngine;
using DG.Tweening;

public class MainCameraController : MonoBehaviour
{
    private const float FADE_TIME = 0.3f;
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
            transform.DOLocalMove(_player.CurrentWeapon._cameraPos, FADE_TIME);
            DOTween.To(() => _camera.fieldOfView, x => _camera.fieldOfView = x, _player.CurrentWeapon._cameraView, FADE_TIME);
        }
        else {
            transform.DOLocalMove(_defaultPos, FADE_TIME);
            DOTween.To(() => _camera.fieldOfView, x => _camera.fieldOfView = x, _defaultView, FADE_TIME);
        }
    }
    
}
