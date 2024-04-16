using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem {
    abstract void Pickup(UnitBase unit);

    public Transform MyTransform { get; set; }

    public bool IsMine { get; set; }
}
