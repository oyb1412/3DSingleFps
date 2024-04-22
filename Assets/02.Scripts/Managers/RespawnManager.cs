using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager 
{
    private const float ALLOW_RESPAWN_RANGE = 15f;
    private Transform _respawnPoint;
    private List<Transform> _respawnList;
    public void Init() {
        _respawnPoint = GameObject.Find("@SpawnPoints").transform;
        _respawnList = new List<Transform>(_respawnPoint.childCount);
        foreach (Transform t in _respawnPoint) {
            _respawnList.Add(t);
        }

        Shuffle(_respawnList);
    }

    public Vector3 GetRandomPosition() {
        return _respawnList[Random.Range(0, _respawnPoint.childCount - 1)].position;
    }

    public Vector3 GetRespawnPosition() {
        var unit = Managers.GameManager.UnitsList;
        foreach(Transform pos in _respawnList) {
            int count = 0;
            foreach(var u in unit) {
                if (Vector3.Distance(pos.position, u.transform.position) > ALLOW_RESPAWN_RANGE) {
                    count++;
                } 
                else {
                    count = 0;
                    continue;
                }

                if (count >= unit.Count) {
                    Debug.Log($"유닛 소환 위치 : {pos.position}");
                    return pos.position;
                }
            }
        }

        Debug.Log("스폰 위치 찾지 못함");
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
