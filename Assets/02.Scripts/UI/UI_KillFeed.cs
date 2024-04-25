using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_KillFeed : MonoBehaviour
{
    [SerializeField] private Image _weaponImage;
    [SerializeField] private Sprite[] _weaponsprites;
    [SerializeField] private TextMeshProUGUI _killerText;
    [SerializeField] private TextMeshProUGUI _victimText;

    [SerializeField] private float _showTime;

    private CanvasGroup _canvasGroup;
    public void Init(Define.WeaponType type, string killerName, string victimName, Transform parent) {
        transform.parent = parent;
        _weaponImage.sprite = _weaponsprites[(int)type];
        _killerText.text = killerName;
        _victimText.text = victimName;
        _canvasGroup = GetComponent<CanvasGroup>();

        StartCoroutine(CoMinusAlpha(_showTime));
    }

    private IEnumerator CoMinusAlpha(float time) {
        float alpha = 1.0f;
        while(alpha > 0f) {
            alpha -= Time.deltaTime / time;
            _canvasGroup.alpha = alpha;
            yield return null;
        }
        PhotonNetwork.Destroy(gameObject);
    }
   
}
