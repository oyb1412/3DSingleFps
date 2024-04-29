using System.Collections;
using TMPro;
using UnityEngine;
using static Define;

public class UI_GameSystem : UI_Base
{
    [SerializeField] private TextMeshProUGUI _killNumberText;
    [SerializeField] private TextMeshProUGUI _deadNumberText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _gameStateText;
    [SerializeField] private TextMeshProUGUI _waitTimeText;
    [SerializeField] private TextMeshProUGUI _continueKillText;

    private bool[] _count = new bool[3];

    private void Awake() {

        Managers.GameManager.WaitStateEvent -= SetWaitUI;
        Managers.GameManager.WaitStateEvent += SetWaitUI;
    }

    protected override void Init() {
        base.Init();
        _player.DeadEvent -= (() => _deadNumberText.text = _player.MyDead.ToString());
        _player.KillEvent -= KillEvent;

        _player.DeadEvent += (() => _deadNumberText.text = _player.MyDead.ToString());
        _player.KillEvent += KillEvent;

        _deadNumberText.text = "0";
        _killNumberText.text = "0";
    }

    private void KillEvent(int continueKillNumber) {
        _killNumberText.text = _player.MyKill.ToString();
        switch (continueKillNumber) {
            case 2:
                StartCoroutine("CoDoubleKill");
                break;
            case 3:
                StopCoroutine("CoDoubleKill");
                StartCoroutine("CoTripleKill");
                break;
        }
    }
    

    private void Update() {
        int min = (int)Managers.GameManager.GameTime / 60;
        int sec = (int)Managers.GameManager.GameTime % 60;

        _timeText.text = $"{min:D2}:{sec:D2}";
    }

    private void SetWaitUI() {
        StartCoroutine(CoWaitUI());
    }

    private IEnumerator CoDoubleKill() {
        float alpha = 1f;
        _continueKillText.text = LOGO_DOUBLEKILL;
        while (alpha > 0) {
            alpha -= Time.deltaTime;
            _continueKillText.color = new Color(1f, 0.7f, 0f, alpha);
            yield return null;
        }
    }

    private IEnumerator CoTripleKill() {
        float alpha = 1f;
        _continueKillText.text = LOGO_TRIPLEKILL;
        while (alpha > 0) {
            alpha -= Time.deltaTime;
            _continueKillText.color = new Color(1f, 0f, 0f, alpha);
            yield return null;
        }
    }

    private IEnumerator CoWaitUI() {
        _waitTimeText.gameObject.SetActive(true);
        while (true) {
            if (_count[2] == false && Managers.GameManager.WaitTime < 3) {
                _count[2] = true;
                PersonalSfxController.instance.SetShareSfx(Define.ShareSfx.Three);
            }
            if (_count[1] == false && Managers.GameManager.WaitTime < 2) {
                _count[1] = true;
                PersonalSfxController.instance.SetShareSfx(Define.ShareSfx.Two);
            }
            if (_count[0] == false && Managers.GameManager.WaitTime < 1) {
                _count[0] = true;
                PersonalSfxController.instance.SetShareSfx(Define.ShareSfx.One);
            }
            _waitTimeText.text = (Mathf.CeilToInt(Managers.GameManager.WaitTime)).ToString();
            if(Managers.GameManager.InGame()) {
                _waitTimeText.gameObject.SetActive(false);
                StartCoroutine(CoStartUI());
                break;
            }
            yield return null;
        }
    }
        
    private IEnumerator CoStartUI() {
        PersonalSfxController.instance.SetShareSfx(Define.ShareSfx.Fight);
        _gameStateText.gameObject.SetActive(true);
        _gameStateText.text = LOGO_START;
        float alpha = 1f;
        while(true) {
            alpha -= Time.deltaTime;
            _gameStateText.color = new Color(1f, 1f, 1f, alpha);
            if(alpha <= 0f) {
                _gameStateText.gameObject.SetActive(false);
                break;
            }
            yield return null;
        }
    }
}
