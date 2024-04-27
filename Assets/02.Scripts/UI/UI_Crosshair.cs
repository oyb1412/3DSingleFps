using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_Crosshair : UI_Base
{
    [SerializeField] private Transform[] _crossLeft;
    [SerializeField] private Transform[] _crossRight;
    [SerializeField] private Transform[] _crossUp;
    [SerializeField] private Transform[] _crossDown;

    [SerializeField] private GameObject[] _crosshairs;

    private float _defalutLeft;
    private float _defalutRight;
    private float _defalutUp;
    private float _defalutDown;

    private float _limitLeft;
    private float _limitRight;
    private float _limitUp;
    private float _limitDown;

    private float _limitValue = 100f;

    private float _crossTime = 0.1f;
    private float _crossValue;

    [SerializeField] private Image _bodyShotImage;
    [SerializeField] private Image _headShotImage;
    [SerializeField] private GameObject _crosshairView;

    private CrosshairType _currentCrosshair = CrosshairType.Line;

    protected override void Init() {
        base.Init();
        _player.CrossValueEvent -= SetCross;
        _player.CrossValueEvent += SetCross;

        _player.ShotEvent -= ShotMethod;
        _player.ShotEvent += ShotMethod;

        _player.AimEvent -= SetCrossHair;
        _player.AimEvent += SetCrossHair;

        _player.ChangeCrosshairEvent -= ChangeCrosshair;
        _player.ChangeCrosshairEvent += ChangeCrosshair;

        _player.BodyshotEvent += (() => StartCoroutine(CoBodyShotImageActive(_bodyShotImage)));
        _player.HeadshotEvent += (() => StartCoroutine(CoHeadShotImageActive(_headShotImage)));

        _defalutLeft = _crossLeft[0].transform.position.x;
        _defalutRight = _crossRight[0].transform.position.x;
        _defalutUp = _crossUp[0].transform.position.y;
        _defalutDown = _crossDown[0].transform.position.y;

        _limitLeft = _defalutLeft - _limitValue;
        _limitRight = _defalutRight + _limitValue;
        _limitUp = _defalutUp + _limitValue;
        _limitDown = _defalutDown - _limitValue;

        _crosshairView.SetActive(true);
    }

    public void ChangeCrosshair(int value) {
        _currentCrosshair = (CrosshairType)value;

        foreach(var t in _crosshairs) {
            t.SetActive(false);
        }

        _crosshairs[value].SetActive(true);
    }

    private void SetCrossHair(bool trigger) {
        _crosshairs[(int)_currentCrosshair].SetActive(!trigger);
    }

    private void SetCross(float value) {
        _crossValue = value;
    }

    private void Update() {
        if (_currentCrosshair == CrosshairType.Point)
            return;

        int index = (int)_currentCrosshair;

        if(_defalutLeft > _crossLeft[index].transform.position.x) {
            _crossLeft[index].transform.position = Vector3.Lerp(_crossLeft[index].transform.position, _crossLeft[index].transform.position + Vector3.right, Time.deltaTime * _crossValue * 0.5f);
        }
        if(_defalutRight < _crossRight[index].transform.position.x) {
            _crossRight[index].transform.position = Vector3.Lerp(_crossRight[index].transform.position, _crossRight[index].transform.position + Vector3.left, Time.deltaTime * _crossValue * 0.5f);
        }
        if(_defalutUp < _crossUp[index].transform.position.y) {
            _crossUp[index].transform.position = Vector3.Lerp(_crossUp[index].transform.position, _crossUp[index].transform.position + Vector3.down, Time.deltaTime * _crossValue * 0.5f);
        }
        if(_defalutDown > _crossDown[index].transform.position.y) {
            _crossDown[index].transform.position = Vector3.Lerp(_crossDown[index].transform.position, _crossDown[index].transform.position + Vector3.up, Time.deltaTime * _crossValue * 0.5f);
        }
    }

    IEnumerator CoHeadShotImageActive(Image image) {
        float alpha = 1f;
        while(alpha > 0f) {
            alpha -= Time.deltaTime;
            image.color = new Color(1f, 0f, 0f, alpha);
            yield return null;
        }
    }

    IEnumerator CoBodyShotImageActive(Image image) {
        float alpha = 1f;
        while (alpha > 0f) {
            alpha -= Time.deltaTime;
            image.color = new Color(1f, 1f, 0f, alpha);
            yield return null;
        }
    }

    public void ShotMethod() {
        if (_currentCrosshair == CrosshairType.Point)
            return;

        StartCoroutine(CoCrossHair());
    }

    private IEnumerator CoCrossHair() {
        float overTime = 0f;

        int index = (int)_currentCrosshair;

        while (overTime < _crossTime) {
            overTime += Time.deltaTime;
            if (_crossLeft[index].transform.position.x > _limitLeft)
                _crossLeft[index].transform.position = Vector3.Lerp(_crossLeft[index].transform.position, _crossLeft[index].transform.position + Vector3.left, Time.deltaTime * _crossValue);
            if (_crossRight[index].transform.position.x < _limitRight)
                _crossRight[index].transform.position = Vector3.Lerp(_crossRight[index].transform.position, _crossRight[index].transform.position + Vector3.right, Time.deltaTime * _crossValue);
            if (_crossUp[index].transform.position.y < _limitUp)
                _crossUp[index].transform.position = Vector3.Lerp(_crossUp[index].transform.position, _crossUp[index].transform.position + Vector3.up, Time.deltaTime * _crossValue);
            if (_crossDown[index].transform.position.y > _limitDown)
                _crossDown[index].transform.position = Vector3.Lerp(_crossDown[index].transform.position, _crossDown[index].transform.position + Vector3.down, Time.deltaTime * _crossValue);
            yield return null;
        }
    }
}
