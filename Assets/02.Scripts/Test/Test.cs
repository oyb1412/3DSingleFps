using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public static Test Instance;

    public enum TestType {
        None,
        Real,
        Test,
    }

    public TestType testType;
    private void Awake() {
        Instance = this;

    }

    private void Start() {
        if(testType == TestType.None) {
            Debug.Log("�׽�Ʈ Ÿ�� ���� �ʿ�");
        }
    }
}
