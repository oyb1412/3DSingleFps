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

    public Vector3 GetValidSpawnPosition() {
        UnitBase[] units = Object.FindObjectsOfType<UnitBase>();
        List<Vector3> possiblePositions = new List<Vector3>();

        for (int i = 0; i < _respawnPoint.childCount; i++) {
            possiblePositions.Add(_respawnPoint.GetChild(i).position);
        }

        int n = possiblePositions.Count;
        while (n > 1) {
            n--;
            int k = Random.Range(0, n + 1);
            Vector3 value = possiblePositions[k];
            possiblePositions[k] = possiblePositions[n];
            possiblePositions[n] = value;
        }

        foreach (var pos in possiblePositions) {
            foreach (var unit in units) {
                if (Vector3.Distance(unit.transform.position, pos) >= ALLOW_RESPAWN_RANGE) {
                    return pos;
                }
            }
        }

        return Vector3.zero; 
    }

    public Vector3 GetRespawnPosition() {
        var unit = Object.FindObjectsOfType<UnitBase>();
        
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
