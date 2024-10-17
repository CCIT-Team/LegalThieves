using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

 
public class MonsterSpawnManager : NetworkBehaviour
{
    public GameObject monsterPrefab;
    public List<Transform> worldPatrolPoints;
    public void PatrollSet()
    {
        worldPatrolPoints = GameObject.FindWithTag("MobPatrollParent").GetComponentsInChildren<Transform>().ToList();
     
    }
    public void SpawnMonster()
    {
        if (Object.HasStateAuthority) // 서버에서만 몬스터 스폰
        {
            // 서버에서 몬스터를 스폰하고 모든 클라이언트에 동기화
            NetworkObject monster = Runner.Spawn(monsterPrefab, worldPatrolPoints[1].position, Quaternion.identity);
            monster.GetComponent<Monster>().patrolPoints = worldPatrolPoints;
        }
    }
    
}
