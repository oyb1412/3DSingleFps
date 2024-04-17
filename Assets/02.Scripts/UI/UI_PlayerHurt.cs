using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_PlayerHurt : UI_Base
{
    private readonly float[] HURTIMAGE_ROTATE = new float[] { 90f, 180f, 270f, 0f };
    [SerializeField] private GameObject HitDirections;
    [SerializeField] private GameObject DeathEffect;
    [SerializeField] private Image HurtEffect;
    private Coroutine _hurtCoroutine;

    private void Update() {
        _player.HurtEvent -= PlayerHurt;
        _player.HurtEvent += PlayerHurt;

        _player.DeadEvent -= PlayerDead;
        _player.DeadEvent += PlayerDead;

        _player.RespawnEvent -= PlayerRespawn;
        _player.RespawnEvent += PlayerRespawn;
    }

    private void PlayerRespawn() {
        DeathEffect.SetActive(false);
        HurtEffect.gameObject.SetActive(false);

        foreach(Transform item in HitDirections.transform) {
            item.gameObject.SetActive(false);
        }
    }

    private void PlayerDead() {
        DeathEffect.SetActive(true);
        StartCoroutine(CoPlayerDead(DeathEffect.transform.GetChild(0).GetComponent<Image>()));
    }

    private void PlayerHurt(Transform attackerTrans, Transform myTrans) {
        if(_hurtCoroutine != null) {
            StopCoroutine(_hurtCoroutine);
        }
        _hurtCoroutine = StartCoroutine(CoPlayerHurt(HurtEffect));
        PlayerHitDirection(attackerTrans, myTrans);
    }

    private void PlayerHitDirection(Transform attackerTrans, Transform myTrans) {
        int count = 0;
        while(HitDirections.transform.childCount > count) {
            if(!HitDirections.transform.GetChild(count).gameObject.activeSelf) 
            {
                DirType rotate = DirectionCalculation(attackerTrans, myTrans);
                HitDirections.transform.GetChild(count).transform.eulerAngles = new Vector3(0f, 0f, HURTIMAGE_ROTATE[(int)rotate]);
                StartCoroutine(CoPlayerHurt(HitDirections.transform.GetChild(count).GetComponent<Image>()));
                break;
            }
            count++;
        }
    }

    private DirType DirectionCalculation(Transform attackerTrans, Transform myTrans) {
        Vector3 relativePosition = attackerTrans.position - myTrans.position;

        float forwardDot = Vector3.Dot(myTrans.forward, relativePosition.normalized);
        float rightDot = Vector3.Dot(myTrans.right, relativePosition.normalized);
        float backDot = Vector3.Dot(-myTrans.forward, relativePosition.normalized);
        float leftDot = Vector3.Dot(-myTrans.right, relativePosition.normalized);

        float maxDot = Mathf.Max(forwardDot, Mathf.Max(backDot, Mathf.Max(rightDot, leftDot)));

        if (maxDot == forwardDot)
            return DirType.Front;
        else if (maxDot == backDot)
            return DirType.Back;
        else if (maxDot == rightDot)
            return DirType.Right;
        else
            return DirType.Left;
    }

    private IEnumerator CoPlayerHurt(Image image) {
        float alpha = 1f;
        image.gameObject.SetActive(true);
        while (alpha > 0f) {
            alpha -= Time.deltaTime * 2f;
            image.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }
        image.gameObject.SetActive(false);
    }

    private IEnumerator CoPlayerDead(Image image) {
        float alpha = 0f;
        image.gameObject.SetActive(true);
        while (alpha < 1f) {
            alpha += Time.deltaTime / _player._RespawnTime;
            image.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
    }
}
