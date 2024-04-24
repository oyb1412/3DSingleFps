using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourcesManager 
{
    public Object Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(GameObject))
        {
            string name = path;
            int index = name.LastIndexOf("/");
            if (index > 0)
                name = name.Substring(index + 1);

            GameObject go = Managers.Pool.GetOriginal(name);
            if (go != null)
                return go as T;
        }
        
        return Resources.Load<T>(path);
    }

    public GameObject Instantiate(string path, Transform parent)
    {
        GameObject obj = Load<GameObject>($"Prefabs/{path}").GameObject();

        if (obj == null)
        {
            Debug.Log($"Failed Search Path : {path}");
            return null;
        }

        if (obj.GetComponent<Poolable>() != null)
            return Managers.Pool.Activation(obj).gameObject;
        
        GameObject go = Object.Instantiate(obj, parent);
        go.name = obj.name;
        return go;
    }

    public GameObject Instantiate(GameObject obj, Transform parent) {

        if (obj == null) {
            return null;
        }

        if (obj.GetComponent<Poolable>() != null)
            return Managers.Pool.Activation(obj).gameObject;

        GameObject go = Object.Instantiate(obj, parent);
        go.name = obj.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
        {
            Debug.Log($"Failed Search GameObject : {go.name}");
            return;
        }

        Poolable poolable = go.GetComponent<Poolable>();
        if (poolable != null)
        {
            Managers.Pool.Release(poolable);
            return;
        }
        
        Object.Destroy(go);
    }
}
