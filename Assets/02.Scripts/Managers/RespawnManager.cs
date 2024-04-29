using System.Collections.Generic;
using UnityEngine;
using static Define;

public class RespawnManager 
{
    private Transform _respawnPoint;
    private List<Transform> _respawnList;
    public void Init() {
        _respawnPoint = GameObject.Find(NAME_SPAWNPOINT).transform;
        _respawnList = new List<Transform>(_respawnPoint.childCount);

        foreach (Transform t in _respawnPoint) {
            _respawnList.Add(t);
        }

        Shuffle(_respawnList);
    }

    public void Clear() {
        _respawnList?.Clear();
    }

    public Vector3 GetRandomPosition() {
        return _respawnList[Random.Range(0, _respawnPoint.childCount - 1)].position;
    }

    public Vector3 GetRespawnPosition() {
        float aollow = Managers.Scene.CurrentScene == Define.SceneType.WareHouse ? WARDHOUSE_ALLOW_RESPAWN_RANGE : PORT_ALLOW_RESPAWN_RANGE;
        var unit = Managers.GameManager.UnitsList;
        Shuffle(_respawnList);
        foreach (Transform pos in _respawnList) {
            bool isSafe = true;  
            foreach (var u in unit) {
                if (Vector3.Distance(pos.position, u.transform.position) <= aollow) {
                    isSafe = false;  
                    break;  
                }
            }
            if (isSafe) {  
                return pos.position;  
            }
        }

        Debug.Log("Vector3.zero ½ºÆù");
        return Vector3.zero;
    }

    public void Shuffle(List<Transform> list) {
        System.Random ran = new System.Random();
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = ran.Next(n + 1);
            var temp = list[k];
            list[k] = list[n];
            list[n] = temp;
        }
    }

}
