using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemController : MonoBehaviour, IItem {
    public abstract void Pickup(PlayerController player);
}
