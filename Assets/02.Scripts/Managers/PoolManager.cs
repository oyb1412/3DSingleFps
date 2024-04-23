using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PoolManager
{
    class Pool
    {
        public GameObject Original { get; private set; }
        public Transform Root { get; private set; }
        private Stack<Poolable> _poolStack = new Stack<Poolable>();

        public void Init(GameObject original, int count = 5)
        {
            Original = original;
            Root = new GameObject().transform;
            Root.name = original.name + "_root";

            for (int i = 0; i < count; i++)
            {
                Release(Create());
            }
        }

        private Poolable Create()
        {
            GameObject go = Object.Instantiate(Original);
            go.name = Original.name;
            return go.GetOrAddComponent<Poolable>();
        }

        public void Release(Poolable poolable)
        {
            if (poolable == null)
                return;

            poolable.transform.parent = Root;
            poolable.gameObject.SetActive(false);
            _poolStack.Push(poolable);
        }

        public Poolable Activation()
        {
            Poolable poolable;

            if (_poolStack.Count > 0)
                poolable = _poolStack.Pop();
            else
                poolable = Create();
            
            poolable.gameObject.SetActive(true);

            poolable.transform.parent = Managers.Scene.CurrentSceneManager.transform;

            poolable.transform.parent = Root;

            return poolable;
        }

    }
    
    private Transform _root;
    private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject("@Pool_root").transform;
            Object.DontDestroyOnLoad(_root.GameObject());
        }
    }

    public void Release(Poolable poolable)
    {
        string name = poolable.gameObject.name;

        if (_pools.ContainsKey(name) == false)
        {
            Object.Destroy(poolable.gameObject);
            return;
        }
        
        _pools[name].Release(poolable);
    }

    public Poolable Activation(GameObject original)
    {
        if(_pools.ContainsKey(original.name) == false)
            CreatePool(original);

        return _pools[original.name].Activation();
    }

    private void CreatePool(GameObject original, int count = 5)
    {
        Pool pool = new Pool();
        pool.Init(original,count);
        pool.Root.parent = _root;
        
        _pools.Add(original.name, pool);
    }

    public GameObject GetOriginal(string name)
    {
        if (_pools.ContainsKey(name) == false)
            return null;

        return _pools[name].Original;
    }
}
