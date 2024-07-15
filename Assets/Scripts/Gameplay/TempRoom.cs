using Fusion;
using UnityEngine;

namespace LegalThieves
{
    public class TempRoom : NetworkBehaviour
    {
        [SerializeField] private Transform  tempRelicSpawnPointsHolder;
        [SerializeField] private bool       isFixedFirstRoom;
        
        public Transform[]  tempRelicSpawnPoints;
        public uint         depth;

        public override void Spawned()
        {
            if(!GetRelicSpawnPoints()) Debug.Log("유물 스폰위치를 설정하지 못했습니다.");
        }

        private bool GetRelicSpawnPoints()
        {
            if (tempRelicSpawnPointsHolder == null) return false;
            
            var t = tempRelicSpawnPointsHolder.GetComponentsInChildren<Transform>();
            tempRelicSpawnPoints = new Transform[t.Length - 1];
            for (var i = 1; i < t.Length; i++)
            {
                tempRelicSpawnPoints[i - 1] = t[i];
            }

            return true;
        }
    }
}