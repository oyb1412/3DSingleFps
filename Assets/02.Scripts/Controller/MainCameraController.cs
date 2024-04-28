using UnityEngine;
using DG.Tweening;
using static Define;

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
        if (trigger) {
            transform.DOLocalMove(_player.CurrentWeapon.CameraPos, MAINCAMERA_FADETIME);
            DOTween.To(() => _camera.fieldOfView, x => _camera.fieldOfView = x, _player.CurrentWeapon.CameraView, MAINCAMERA_FADETIME);
        }
        else {
            transform.DOLocalMove(_defaultPos, MAINCAMERA_FADETIME);
            DOTween.To(() => _camera.fieldOfView, x => _camera.fieldOfView = x, _defaultView, MAINCAMERA_FADETIME);
        }
    }

}
