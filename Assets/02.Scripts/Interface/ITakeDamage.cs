using UnityEngine;

public interface ITakeDamage {
    void TakeDamage(int damage, Transform attackerTrans, Transform myTrans, bool headShot);
}