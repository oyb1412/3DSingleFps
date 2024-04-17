using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_GameSystem : UI_Base
{
    [SerializeField] private TextMeshProUGUI _killNumberText;
    [SerializeField] private TextMeshProUGUI _deadNumberText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private TextMeshProUGUI _gameStateText;
    [SerializeField] private TextMeshProUGUI _waitTimeText;
    [SerializeField] private TextMeshProUGUI _continueKillText;

    private void Start() {
        _player.DeadEvent -= (() => _deadNumberText.text = _player.MyDead.ToString());
        _player.KillEvent -= (() => _killNumberText.text = _player.MyKill.ToString());
        _player.DoubleKillEvent -= (() => StartCoroutine("CoDoubleKill"));
        _player.TripleKillEvent -= TripleKillEvent;  
        
        _player.DeadEvent += (() => _deadNumberText.text = _player.MyDead.ToString());
        _player.KillEvent += (() => _killNumberText.text = _player.MyKill.ToString());
        _player.DoubleKillEvent += (() => StartCoroutine("CoDoubleKill"));
        _player.TripleKillEvent += TripleKillEvent;

        _deadNumberText.text = "0";
        _killNumberText.text = "0";
        Managers.GameManager.WaitStateEvent += SetWaitUI;
    }

    private void Update() {
        int min = (int)Managers.GameManager.GameTime / 60;
        int sec = (int)Managers.GameManager.GameTime % 60;

        _timeText.text = $"{min:D2}:{sec:D2}";
    }

    private void TripleKillEvent() {
        StopCoroutine("CoDoubleKill");
        StartCoroutine("CoTripleKill");
    }

    private void SetWaitUI() {
        StartCoroutine(CoWaitUI());
    }

    private IEnumerator CoDoubleKill() {
        float alpha = 1f;
        _continueKillText.text = "DOUBLE KILL!!";
        while (alpha > 0) {
            alpha -= Time.deltaTime;
            _continueKillText.color = new Color(1f, 0.7f, 0f, alpha);
            yield return null;
        }
    }

    private IEnumerator CoTripleKill() {
        float alpha = 1f;
        _continueKillText.text = "TRIPLE KILL!!!";
        while (alpha > 0) {
            alpha -= Time.deltaTime;
            _continueKillText.color = new Color(1f, 0f, 0f, alpha);
            yield return null;
        }
    }

    private IEnumerator CoWaitUI() {
        _waitTimeText.gameObject.SetActive(true);
        while(true) {
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
        _gameStateText.gameObject.SetActive(true);
        _gameStateText.text = "START!";
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
