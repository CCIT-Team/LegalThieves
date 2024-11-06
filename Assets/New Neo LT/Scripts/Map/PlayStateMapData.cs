using UnityEngine;

namespace New_Neo_LT.Scripts.Map
{
    public class PlayStateMapData : MonoBehaviour
    {
        [SerializeField] protected Transform[] spawnPoints;
        [SerializeField] private Transform[] relicSpawnPoints;
        
        public int SpawnPointCount => spawnPoints.Length;
        public int RelicSpawnPointCount => relicSpawnPoints.Length;
        
        public Vector3 GetSpawnPosition(int index)
        {
            return spawnPoints[index].position;
        }
        
        public Vector3 GetRelicSpawnPosition(int index)
        {
            return relicSpawnPoints[index].position;
        }
    }
}