using UnityEngine;

public class PlayerStatus : StatusBase {
    [SerializeField] public float _rotateSpeed;
    [SerializeField] public float _jumpValue;
    [SerializeField] public float _boundValue;
    [SerializeField] public float _boundTime = 0.1f;
}