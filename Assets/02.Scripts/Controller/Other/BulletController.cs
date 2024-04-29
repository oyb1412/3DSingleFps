using UnityEngine;
using static Define;

public class BulletController : MonoBehaviour
{
    private Vector3 _dir;
    private Collider _parentCollider;
    public void Init(Vector3 pos, Vector3 dir, GameObject go) {
        transform.position = pos;
        _dir = dir;
        _parentCollider = go.GetComponent<Collider>();
    }

    private void Start() {
        Invoke("DestroyObject", .5f);
    }

    private void DestroyObject() {
        Managers.Resources.Destroy(gameObject);
    }
    private void Update() {
        transform.Translate(_dir * Time.deltaTime * 100f);
    }

    private void OnTriggerEnter(Collider c) {
        if (c == _parentCollider)
            return;

        if(c.CompareTag(TAG_UNIT) ||
            c.CompareTag(TAG_WALL) ||
            c.CompareTag(TAG_OBSTACLE))
            DestroyObject();
    }
}
