using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Item : UI_Base
{
    [SerializeField] private GameObject _itemView;

    private void Start() {
        _player.CollideItemEvent += ((trigger) => _itemView.SetActive(trigger));
    }
}
