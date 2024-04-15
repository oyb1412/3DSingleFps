using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerInfo : UI_Base
{
    [SerializeField]private TextMeshProUGUI _currentHpText;
    [SerializeField]private TextMeshProUGUI _currentWeaponNameText;
    [SerializeField]private TextMeshProUGUI _currentBulletNumberText;
    [SerializeField]private TextMeshProUGUI _maxBulletNumberText;
    [SerializeField]private Image _currentHpBarImage;
    [SerializeField] private Image _currentWeaponIconImage;

    private PlayerStatus _status;
    protected override void Init() {
        base.Init();
        _player.HpEvent += HpEvent;
        _player.BulletEvent += BulletEvent;
        _player.ChangeEvent += ChangeWeaponEvent;

        _status = _player.Status;
        _currentHpText.text = _status._currentHp.ToString();
        _currentHpBarImage.fillAmount = (float)_status._currentHp / _status._maxHp;
        _currentBulletNumberText.text = _player.CurrentWeapon.CurrentBullet.ToString();
        _maxBulletNumberText.text = _player.CurrentWeapon.MaxBullet.ToString();
        _currentWeaponNameText.text = _player.CurrentWeapon.Name;
    }

    private void ChangeWeaponEvent(WeaponController weapon) {
        _currentWeaponIconImage.sprite = weapon.WeaponIcon;
        _currentWeaponNameText.text = weapon.Name;
        BulletEvent(weapon.CurrentBullet, weapon.MaxBullet);
    }

    private void HpEvent(int currentHp, int maxHp) {
        _currentHpText.text = currentHp.ToString();
        _currentHpBarImage.fillAmount = (float)currentHp / maxHp;
    }

    private void BulletEvent(int currentBullet, int maxBullet) {
        _currentBulletNumberText.text = currentBullet.ToString();
        _maxBulletNumberText.text = maxBullet.ToString();
    }
}
