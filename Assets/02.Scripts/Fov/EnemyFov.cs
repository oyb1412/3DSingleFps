using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class EnemyFov : MonoBehaviour {
    public float viewRange = 15f;
    [Range(0, 360)]
    public float viewAngle = 120f;

    private UnitBase _unit;
    public bool IsDead { get; set; }



    private void Awake() {
        _unit = GetComponent<UnitBase>();
    }
    public Vector3 CirclePoint(float angle) {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public UnitBase isTracePlayer() {
        if (IsDead)
            return null;

        var colls = Physics.OverlapSphere(_unit.FirePoint.position, viewRange, LayerMask.GetMask("Unit"));
        if (colls.Length > 0) {
            foreach (var coll in colls) {
                if (coll.gameObject == gameObject)
                    continue;

                var dir = coll.transform.position - transform.position;
                dir = dir.normalized;
                if (Vector3.Angle(transform.forward, dir) < viewAngle * 0.5f) {

                    int mask = (1 << (int)LayerType.Unit) | (1 << (int)LayerType.Obstacle) | (1 << (int)LayerType.Wall);
                    Debug.DrawRay(_unit.FirePoint.position, dir * 100f, Color.red);
                    bool hit = Physics.Raycast(_unit.FirePoint.position, dir, out var target, float.MaxValue, mask);

                    if (!hit)
                        continue;

                    if (target.collider.gameObject.layer == (int)LayerType.Obstacle ||
                        target.collider.gameObject.layer == (int)LayerType.Wall)
                        continue;

                    if (target.collider.gameObject.layer == (int)LayerType.Unit)
                        return coll.gameObject.GetComponent<UnitBase>();
                }
            }
        }
        return null;
    }

}
