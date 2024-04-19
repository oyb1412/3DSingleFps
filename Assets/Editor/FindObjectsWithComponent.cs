using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using TMPro;

public class FindObjectsWithComponent : EditorWindow {
    private void OnGUI() {
        if (GUILayout.Button("Find All TextMeshProUGUI Objects")) {
            List<GameObject> foundObjects = new List<GameObject>();
            foreach (TextMeshProUGUI tmp in Resources.FindObjectsOfTypeAll<TextMeshProUGUI>()) {
                if (tmp.gameObject.scene.name != null)  // 씬에 있는 오브젝트인지 확인
                {
                    foundObjects.Add(tmp.gameObject);
                    tmp.raycastTarget = false;
                    Debug.Log("Found object with TextMeshProUGUI: " + tmp.gameObject.name, tmp.gameObject);
                }
            }
            EditorUtility.FocusProjectWindow();
            Selection.objects = foundObjects.ToArray();
        }
    }

    [MenuItem("Window/Custom Utilities/Find Objects With Component")]
    public static void ShowWindow() {
        GetWindow<FindObjectsWithComponent>("Find Objects With Component");
    }
}
