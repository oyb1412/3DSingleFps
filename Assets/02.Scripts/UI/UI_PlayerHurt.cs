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


    protected override void Init() {
        base.Init();

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
        StartCoroutine(Co_ImageGradualInvisible(HurtEffect, 2f, 1f, 1f, 1f, 0.5f));
        PlayerHitDirection(attackerTrans, myTrans);
    }

    private void PlayerHitDirection(Transform attackerTrans, Transform myTrans) {
        int count = 0;
        while(HitDirections.transform.childCount > count) {
            if(!HitDirections.transform.GetChild(count).gameObject.activeSelf) 
            {
                DirType rotate = Util.DirectionCalculation(attackerTrans, myTrans);
                HitDirections.transform.GetChild(count).transform.eulerAngles = new Vector3(0f, 0f, HURTIMAGE_ROTATE[(int)rotate]);
                StartCoroutine(Co_ImageGradualInvisible(HitDirections.transform.GetChild(count).GetComponent<Image>(),
                    2f, 1f, 1f, 1f));
                break;
            }
            count++;
        }
    }

    private IEnumerator CoPlayerDead(Image image) {
        float alpha = 0f;
        image.gameObject.SetActive(true);
        while (alpha < 1f) {
            alpha += Time.deltaTime / GameManager.Instance.RespawnTime;
            image.color = new Color(0f, 0f, 0f, alpha);
            yield return null;
        }
    }
}
