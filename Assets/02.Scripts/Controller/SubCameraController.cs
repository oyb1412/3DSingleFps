using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubCameraController : MonoBehaviour
{
    [SerializeField]private Vector3 _defaultPos;
    [SerializeField]private float _cameraSpeed;
    [SerializeField] private Transform _playerTrans;
    private void Start() {
    }

    private void OnEnable() {
        transform.position = _defaultPos + _playerTrans.position;
    }

    void Update()
    {
        transform.position += Vector3.up * _cameraSpeed * Time.deltaTime;
    }
}
