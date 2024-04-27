using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class Util : MonoBehaviour
{
    public static void SetLayer(GameObject go, LayerType type) {
        if( go == null)
            return;

        go.layer = (int)type;

        foreach(Transform child in go.transform) {
            if (child.gameObject.layer == (int)LayerType.Head ||
               child.gameObject.layer == (int)LayerType.Body)
                continue;

            SetLayer(child.gameObject, type);
        }
    }

    public static UnitBase FindPlayerByActorNumber(int actorNumber) {
        Debug.Log($"총 유닛 수는 {PhotonNetwork.PlayerList}명");
        foreach (PlayerController player in FindObjectsByType(typeof(PlayerController), FindObjectsSortMode.None)) {
            if (player.PV.OwnerActorNr == actorNumber) {
                {
                    Debug.Log($"{actorNumber}의 플레이어는 {player.name}");
                    return player;
                }
            }
        }
        return null; 
    }

    public static GameObject FindChild(GameObject go, string name, bool recursion) 
    {
        if (go == null || string.IsNullOrEmpty(name))
            return null;

        if (recursion) {
            foreach (Transform child in go.transform) {
                if (child.name == name)
                    return child.gameObject;

                GameObject found = FindChild(child.gameObject, name, recursion);
                if (found != null) return found;
            }
        } else {
            foreach (Transform child in go.transform) {
                if (child.name == name)
                    return child.gameObject;
            }
        }

        return null;
    }

    public static DirType DirectionCalculation(Transform attackerTrans, Transform myTrans) {
        Vector3 relativePosition = attackerTrans.position - myTrans.position;

        float forwardDot = Vector3.Dot(myTrans.forward, relativePosition.normalized);
        float rightDot = Vector3.Dot(myTrans.right, relativePosition.normalized);
        float backDot = Vector3.Dot(-myTrans.forward, relativePosition.normalized);
        float leftDot = Vector3.Dot(-myTrans.right, relativePosition.normalized);

        float maxDot = Mathf.Max(forwardDot, Mathf.Max(backDot, Mathf.Max(rightDot, leftDot)));

        if (maxDot == forwardDot)
            return DirType.Front;
        else if (maxDot == backDot)
            return DirType.Back;
        else if (maxDot == rightDot)
            return DirType.Right;
        else
            return DirType.Left;
    }


    public static T GetorAddComponent<T>(GameObject go) where T : Component
    {
        var component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static void QuitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public static List<Color> GenerateDistinctColors(int count) {
        List<Color> colors = new List<Color>();
        float hueStep = 1f / count;

        for (int i = 0; i < count; i++) {
            float hue = i * hueStep; 
            Color color = Color.HSVToRGB(hue, 0.8f, 0.8f);
            colors.Add(color);
        }

        return colors;
    }

    
}
