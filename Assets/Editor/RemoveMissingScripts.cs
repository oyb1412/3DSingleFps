using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RemoveMissingScripts : EditorWindow {
    [MenuItem("Tools/Remove Missing Scripts")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(RemoveMissingScripts));
    }

    void OnGUI() {
        if (GUILayout.Button("Find and Remove All Missing Scripts in Project")) {
            FindAndRemoveMissingScripts();
        }
    }

    private static void FindAndRemoveMissingScripts() {
        GameObject[] allObjects = GetAllObjectsInProject();
        int totalCount = 0;
        foreach (GameObject go in allObjects) {
            int count = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            if (count > 0) {
                Debug.Log($"Removed {count} missing script(s) from {go.name}", go);
                totalCount += count;
            }
        }
        Debug.Log($"Total removed missing scripts: {totalCount}");
    }

    private static GameObject[] GetAllObjectsInProject() {
        List<GameObject> objectsInProject = new List<GameObject>();
        foreach (GameObject go in Resources.FindObjectsOfTypeAll<GameObject>()) {
            if (string.IsNullOrEmpty(AssetDatabase.GetAssetPath(go)) && go.hideFlags == HideFlags.None) {
                objectsInProject.Add(go);
            }
        }
        return objectsInProject.ToArray();
    }
}

