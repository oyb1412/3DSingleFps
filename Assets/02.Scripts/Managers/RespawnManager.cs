using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager 
{
    private const float ALLOW_RESPAWN_RANGE = 30f;
    private Transform _respawnPoint;

    public void Init() {
        _respawnPoint = GameObject.Find("@SpawnPoints").transform;
    }

    public Vector3 GetRandomPosition() {
        return _respawnPoint.GetChild(Random.Range(0, _respawnPoint.childCount - 1)).position;
    }

    public Vector3 GetRespawnPosition() {
        var unit = GameObject.FindObjectsByType<UnitBase>(FindObjectsSortMode.None);
        
       foreach(var u in unit) {
            Vector3 ran = GetRandomPosition();
            if (Vector3.Distance(u.transform.position, ran) > ALLOW_RESPAWN_RANGE) {
                return ran;
            }
        }
            
        return Vector3.zero;
    }

    public Vector3[] GetRandomRespawnPosition(int count) {
        Vector3[] pos = new Vector3[count];
        int c = 0;
        while (true) {
            for(int i = 0; i< count; i++) {
                pos[i] = GetRandomPosition();
            }

            for(int i = 0; i< count - 1; i++) {
                if (pos[i] != pos[i + 1])
                    c++;

                if(c >= count - 1) {
                    return pos;
                }
            }
            c = 0;
        }
    }
}
