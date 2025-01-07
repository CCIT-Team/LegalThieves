using UnityEngine;

namespace New_Neo_LT.Scripts.Map
{
    public class PlayStateMapData : MonoBehaviour
    {
        [SerializeField] protected Transform[] spawnPoints;

        
        public int SpawnPointCount => spawnPoints.Length;

        public Vector3 GetSpawnPosition(int index)
        {
            return spawnPoints[index].position;
        }
        
    }
}