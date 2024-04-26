using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_PlayerInfo : UI_Base
{
    [SerializeField]private TextMeshProUGUI _currentHpText;
    [SerializeField]private TextMeshProUGUI _currentWeaponNameText;
    [SerializeField]private TextMeshProUGUI _currentBulletNumberText;
    [SerializeField]private TextMeshProUGUI _maxBulletNumberText;
    [SerializeField]private TextMeshProUGUI _healText;
    [SerializeField]private TextMeshProUGUI _killInfomationText;

    [SerializeField]private Image _currentHpBarImage;
    [SerializeField]private Image _currentWeaponIconImage;
    [SerializeField]private Image _currentBulletBarImage;

    private Vector3 _healTextDefaultPos;
    protected override void Init() {
        base.Init();

        _player.HpEvent -= HpEvent;
        _player.BulletEvent -= BulletEvent;
        _player.ChangeEvent -= ChangeWeaponEvent;  
        _player.HpEvent += HpEvent;
        _player.BulletEvent += BulletEvent;
        _player.ChangeEvent += ChangeWeaponEvent;

        _player.RespawnEvent -= RespawnEvent;
        _player.RespawnEvent += RespawnEvent;

        _player.KillAndDeadEvent -= KillInfomationEvent;
        _player.KillAndDeadEvent += KillInfomationEvent;

        _currentHpText.text = _player.GetCurrentHp.ToString();
        _currentHpBarImage.fillAmount = (float)_player.GetCurrentHp / _player.GetMaxHp;
        _currentBulletBarImage.fillAmount = (float)_player.CurrentWeapon.CurrentBullet / _player.CurrentWeapon.RemainBullet;
        _currentBulletNumberText.text = _player.CurrentWeapon.CurrentBullet.ToString();
        _maxBulletNumberText.text = _player.CurrentWeapon.MaxBullet.ToString();
        _currentWeaponNameText.text = _player.CurrentWeapon.Name;

        _healTextDefaultPos = _healText.transform.position;
        _healText.gameObject.SetActive(false);
    }

    private void KillInfomationEvent(DirType dir, string name, bool kill, bool headShot) {
        string text = string.Empty;
        if(kill) {
            if(headShot) {
                text = $"Killer the {name} ({dir.ToString()}, HeadShot)";
            }
            else {
                text = $"Killer the {name} ({dir.ToString()})";
            }
        }
        else {
            if (headShot) {
                text = $"killed by that {name} ({dir.ToString()}, HeadShot)";
            } else {
                text = $"killed by that {name} ({dir.ToString()})";
            }
        }

        StartCoroutine(Co_TextGradualInvisible(_killInfomationText, text, .3f ,1f, 1f, 1f));
    }

    private void ChangeWeaponEvent(Player.WeaponController weapon) {
        _currentWeaponIconImage.sprite = weapon.WeaponIcon;
        _currentWeaponNameText.text = weapon.Name;
        BulletEvent(weapon.CurrentBullet, weapon.MaxBullet, weapon.RemainBullet);
    }

    private void RespawnEvent() {
        HpEvent(_player.GetCurrentHp, _player.GetMaxHp);
        BulletEvent(_player.CurrentWeapon.CurrentBullet, _player.CurrentWeapon.MaxBullet, _player.CurrentWeapon.RemainBullet);
        ChangeWeaponEvent(_player.CurrentWeapon);
    }

    private void HpEvent(int currentHp, int maxHp, int damage = 0) {
        _currentHpText.text = currentHp.ToString();
        _currentHpBarImage.fillAmount = (float)currentHp / maxHp;
        StartCoroutine(CoHealTextMove(damage));
    }

    private IEnumerator CoHealTextMove(int damage) {
        float time = 1f;
        _healText.gameObject.SetActive(true);
        _healText.transform.position = _healTextDefaultPos;
        _healText.text = $"+{damage:D2} HP";

        if (damage > 0)
            _healText.color = Color.green;
        else
            _healText.color = Color.red;

        while (time > 0f) {
            time -= Time.deltaTime * 2f;
            _healText.transform.position += (Vector3.up * (1 - time));
            yield return null;
        }
        _healText.gameObject.SetActive(false);
    }

    private void BulletEvent(int currentBullet, int maxBullet, int remainBullet) {
        _currentBulletNumberText.text = currentBullet.ToString();
        _maxBulletNumberText.text = maxBullet.ToString();
        _currentBulletBarImage.fillAmount = (float)currentBullet / remainBullet;
    }
}
