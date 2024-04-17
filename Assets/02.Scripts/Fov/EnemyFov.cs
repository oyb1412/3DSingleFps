using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class EnemyFov : MonoBehaviour {
    public float viewRange = 15f;
    [Range(0, 360)]
    public float viewAngle = 120f;

    public bool IsDead { get; set; }

    public Vector3 CirclePoint(float angle) {
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public UnitBase isTracePlayer() {
        if (IsDead)
            return null;

        var colls = Physics.OverlapSphere(transform.position, viewRange, LayerMask.GetMask("Unit"));
        if (colls.Length > 0) {
            foreach (var coll in colls) {
                if (coll.gameObject == gameObject)
                    continue;

                var dir = coll.transform.position - transform.position;
                dir = dir.normalized;
                if (Vector3.Angle(transform.forward, dir) < viewAngle * 0.5f) {

                    int mask = (1 << (int)LayerType.Unit) | (1 << (int)LayerType.Obstacle);
                    Debug.DrawRay(transform.position + Vector3.up, dir * 100f, Color.red);
                    bool hit = Physics.Raycast(transform.position + Vector3.up, dir, out var target, float.MaxValue, mask);

                    if (!hit)
                        continue;

                    if (target.collider.gameObject.layer == (int)LayerType.Obstacle)
                        continue;

                    else if (target.collider.gameObject.layer == (int)LayerType.Unit)
                        return coll.gameObject.GetComponent<UnitBase>();
                }
            }
        }
        return null;
    }

}
