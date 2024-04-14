using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Util : MonoBehaviour
{
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

    public static T GetorAddComponent<T>(GameObject go) where T : Component
    {
        var component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }
}
