using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_GameSystem : UI_Base
{
    private const string DOUBLE_KILL_LOGO = "DOUBLE KILL!";
    private const string TRIPLE_KILL_LOGO = "TRIPLE KILL!";

    [SerializeField] private TextMeshProUGUI _killNumberText;
    [SerializeField] private TextMeshProUGUI _deadNumberText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _gameStateText;
    [SerializeField] private TextMeshProUGUI _waitTimeText;
    [SerializeField] private TextMeshProUGUI _continueKillText;

    private bool[] _count = new bool[3];

    protected override void Awake() {
        GameManager.Instance.WaitStateEvent -= SetWaitUI;
        GameManager.Instance.WaitStateEvent += SetWaitUI;
    }

    protected override void Init() {
        base.Init();


        _player.DeadEvent -= (() => _deadNumberText.text = _player.MyDead.ToString());
        _player.KillEvent -= (() => _killNumberText.text = _player.MyKill.ToString());
        _player.MutilKillEvent -= MultiKillEvent;

        _player.DeadEvent += (() => _deadNumberText.text = _player.MyDead.ToString());
        _player.KillEvent += (() => _killNumberText.text = _player.MyKill.ToString());
        _player.MutilKillEvent += MultiKillEvent;

        _deadNumberText.text = "0";
        _killNumberText.text = "0";
    }

    private void Update() {
        int min = (int)GameManager.Instance.GameTime / 60;
        int sec = (int)GameManager.Instance.GameTime % 60;

        _timeText.text = $"{min:D2}:{sec:D2}";
    }

    private void MultiKillEvent(int kill) {
        if(kill == PlayerController.DOUBLE_KILL) {
            _currentCoroutine = StartCoroutine(Co_TextGradualInvisible(_continueKillText, DOUBLE_KILL_LOGO,1f, 1f, 0.7f, 0.1f));
        }
        else {
            if (_currentCoroutine != null)
                StopCoroutine(_currentCoroutine);

            StartCoroutine(Co_TextGradualInvisible(_continueKillText, TRIPLE_KILL_LOGO,1f, 1f, 0f, 0f));
        }
    }

    private void SetWaitUI() {
        StartCoroutine(CoWaitUI());
    }

    private IEnumerator CoWaitUI() {
        _waitTimeText.gameObject.SetActive(true);
        while (true) {
            if (_count[2] == false && GameManager.Instance.WaitTime < 3) {
                _count[2] = true;
                ShareSfxController.instance.SetShareSfx(Define.ShareSfx.Three);
            }
            if (_count[1] == false && GameManager.Instance.WaitTime < 2) {
                _count[1] = true;
                ShareSfxController.instance.SetShareSfx(Define.ShareSfx.Two);
            }
            if (_count[0] == false && GameManager.Instance.WaitTime < 1) {
                _count[0] = true;
                ShareSfxController.instance.SetShareSfx(Define.ShareSfx.One);
            }
            _waitTimeText.text = (Mathf.CeilToInt(GameManager.Instance.WaitTime)).ToString();
            if(GameManager.Instance.InGame()) {
                _waitTimeText.gameObject.SetActive(false);
                ShareSfxController.instance.SetShareSfx(Define.ShareSfx.Fight);
                StartCoroutine(Co_TextGradualInvisible(_gameStateText, "START!", 1f, 1f, 1f, 1f));
                break;
            }
            yield return null;
        }
    }
}
