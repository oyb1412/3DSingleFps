using UnityEngine;

public class UI_Item : UI_Base
{
    [SerializeField] private GameObject _itemView;

    protected override void Init() {
        base.Init();
        _player.CollideItemEvent += ((trigger) => _itemView.SetActive(trigger));
    }
   
}
