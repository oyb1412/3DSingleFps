using UnityEngine;

public class BulletController : MonoBehaviour {
    private Vector3 _dir;
    public void Init(Vector3 dir) {
        _dir = dir;
    }
    private void Update() {
        transform.Translate(_dir.normalized * 100f * Time.deltaTime);
    }
}