using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemController : MonoBehaviour, IItem {
    [SerializeField] private float _itemRotateSpeed;

    public Transform MyTransform { get; set; }
    public bool IsMine { get ; set ; }


    public abstract void Pickup(UnitBase unit);

    public void Init(Vector3 pos, bool trigger) {
        transform.position = pos;
        IsMine = trigger;
    }
    private void Start() {
        MyTransform = transform;
    }
    private void Update() {
        transform.Rotate(0f, _itemRotateSpeed, 0f);
    }
}
