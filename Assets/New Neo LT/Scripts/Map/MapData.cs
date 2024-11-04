using UnityEngine;

namespace New_Neo_LT.Scripts.Map
{
    public class MapData : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPoints;
        
        public Vector3 GetSpawnPosition(int index)
        {
            return spawnPoints[index].position;
        }
    }
}
