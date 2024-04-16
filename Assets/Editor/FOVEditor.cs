using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(EnemyFov))]
public class FOVEditor : Editor {
    private void OnSceneGUI() {
        EnemyFov fov = (EnemyFov)target;

        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);

        Handles.color = Color.green;

        Handles.DrawWireDisc(fov.transform.position, Vector3.up, fov.viewRange);

        Handles.color = new Color(1f, 1f, 1f, 0.2f);

        Handles.DrawSolidArc(fov.transform.position, Vector3.up, fromAnglePos, fov.viewAngle, fov.viewRange);

        Handles.Label(fov.transform.position + (fov.transform.forward * 2.0f), fov.viewAngle.ToString());
    }
}