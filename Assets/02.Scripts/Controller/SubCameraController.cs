using UnityEngine;
using static Define;

public class SubCameraController : MonoBehaviour
{
    private Vector3 _defaultPos;
    [SerializeField] private Transform _playerTrans;

    private void Awake() {
        _defaultPos = SUBCAMERA_DEFAULT_POSITION; 
    }
    private void OnEnable() {
        transform.position = _defaultPos + _playerTrans.position;
    }

    void Update()
    {
        if (!Managers.GameManager.InGame())
            return;

        transform.position += Vector3.up * SUBCAMERA_MOVESPEED * Time.deltaTime;
    }

}
