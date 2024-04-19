using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Crosshair : UI_Base
{
    private Transform _crossLeft;
    private Transform _crossRight;
    private Transform _crossUp;
    private Transform _crossDown;

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

    protected override void Init() {
        base.Init();
        _player.CrossValueEvent -= SetCross;
        _player.CrossValueEvent += SetCross;
        _player.ShotEvent -= ShotMethod;
        _player.ShotEvent += ShotMethod;

        _player.BodyshotEvent += (() => StartCoroutine(CoBodyShotImageActive(_bodyShotImage)));
        _player.HeadshotEvent += (() => StartCoroutine(CoHeadShotImageActive(_headShotImage)));
    }

    private void Start() {

        _crossLeft = Util.FindChild(gameObject, "Cross_Left", false).transform;
        _crossRight = Util.FindChild(gameObject, "Cross_Right", false).transform;
        _crossUp = Util.FindChild(gameObject, "Cross_Up", false).transform;
        _crossDown = Util.FindChild(gameObject, "Cross_Down", false).transform;

        _defalutLeft = _crossLeft.transform.position.x;
        _defalutRight = _crossRight.transform.position.x;
        _defalutUp = _crossUp.transform.position.y;
        _defalutDown = _crossDown.transform.position.y;

        _limitLeft = _defalutLeft - _limitValue;
        _limitRight = _defalutRight + _limitValue;
        _limitUp = _defalutUp + _limitValue;
        _limitDown = _defalutDown - _limitValue;
    }

    private void SetCross(float value) {
        _crossValue = value;
    }

    private void Update() {
        if(_defalutLeft > _crossLeft.transform.position.x) {
            _crossLeft.transform.position = Vector3.Lerp(_crossLeft.transform.position, _crossLeft.transform.position + Vector3.right, Time.deltaTime * _crossValue * 0.5f);
        }
        if(_defalutRight < _crossRight.transform.position.x) {
            _crossRight.transform.position = Vector3.Lerp(_crossRight.transform.position, _crossRight.transform.position + Vector3.left, Time.deltaTime * _crossValue * 0.5f);
        }
        if(_defalutUp < _crossUp.transform.position.y) {
            _crossUp.transform.position = Vector3.Lerp(_crossUp.transform.position, _crossUp.transform.position + Vector3.down, Time.deltaTime * _crossValue * 0.5f);
        }
        if(_defalutDown > _crossDown.transform.position.y) {
            _crossDown.transform.position = Vector3.Lerp(_crossDown.transform.position, _crossDown.transform.position + Vector3.up, Time.deltaTime * _crossValue * 0.5f);
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
        StartCoroutine(CoCrossHair());
    }

    private IEnumerator CoCrossHair() {
        float overTime = 0f;
        while (overTime < _crossTime) {
            overTime += Time.deltaTime;
            if (_crossLeft.transform.position.x > _limitLeft)
                _crossLeft.transform.position = Vector3.Lerp(_crossLeft.transform.position, _crossLeft.transform.position + Vector3.left, Time.deltaTime * _crossValue);
            if (_crossRight.transform.position.x < _limitRight)
                _crossRight.transform.position = Vector3.Lerp(_crossRight.transform.position, _crossRight.transform.position + Vector3.right, Time.deltaTime * _crossValue);
            if (_crossUp.transform.position.y < _limitUp)
                _crossUp.transform.position = Vector3.Lerp(_crossUp.transform.position, _crossUp.transform.position + Vector3.up, Time.deltaTime * _crossValue);
            if (_crossDown.transform.position.y > _limitDown)
                _crossDown.transform.position = Vector3.Lerp(_crossDown.transform.position, _crossDown.transform.position + Vector3.down, Time.deltaTime * _crossValue);
            yield return null;
        }
    }
}
