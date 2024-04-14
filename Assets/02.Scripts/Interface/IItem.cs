using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItem {
    abstract void Pickup(PlayerController player);
}
