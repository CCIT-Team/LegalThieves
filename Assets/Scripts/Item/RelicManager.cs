using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.Relic;
using UnityEngine;

namespace LegalThieves
{
    public class RelicManager : NetworkBehaviour
    {
        public static RelicManager Instance;

        [Header("Components")]
        [SerializeField] private NetworkPrefabRef    relicPrefab;
        [SerializeField] private Transform           relicPool;
        
        [Header("Relic Data")]
        [Header("Gold Relics")]
        [SerializeField] private string[]            goldRelicNames;
        [SerializeField] private GameObject[]        goldRelicVisuals;
        [SerializeField] private Sprite[]            goldRelicSprites;
        [Header("Renown Relics")]
        [SerializeField] private string[]            renownRelicNames;
        [SerializeField] private GameObject[]        renownRelicVisuals;
        [SerializeField] private Sprite[]            renownRelicSprites;
        
        

        [Networked, Capacity(200)]
        NetworkLinkedList<RelicObject> Relics => default;

        private void Start()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
        }

        public override void Spawned()
        {
            if (!HasStateAuthority)
                return;

            //SpawnAllRelics();
        }
        
        public void SpawnAllRelics()
        {
            for(var i = 0; i < NewGameManager.Instance.playMapData.RelicSpawnPointCount; i++)
                SpawnRelic(NewGameManager.Instance.playMapData.GetRelicSpawnPosition(i));
        }

        public void SpawnRelic(Vector3 position = default)
        {
            var netObj = Runner.Spawn(relicPrefab, position, Quaternion.identity, null, OnBeforeRelicSpawned);
            netObj.transform.SetParent(relicPool);
            var relic = netObj.GetComponent<RelicObject>();
            
            Relics.Add(relic);
        }

        // 유물 스폰 전에 유물 데이터를 설정합니다.
        private void OnBeforeRelicSpawned(NetworkRunner runner, NetworkObject obj)
        {
            
        }

        public RelicObject GetRelicData(int index)
        {
            return Relics.Get(index);
        }
        
        public int GetRelicIndex(RelicObject relic)
        {
            return Relics.IndexOf(relic);
        }

        public Mesh GetRelicMesh(int index)
        {
            return index < goldRelicVisuals.Length ? 
                goldRelicVisuals[index].GetComponent<MeshFilter>().sharedMesh : 
                renownRelicVisuals[index - goldRelicVisuals.Length].GetComponent<MeshFilter>().sharedMesh;
        }
        
        public Material[] GetRelicMaterial(int index)
        {
            return index < goldRelicVisuals.Length ? 
                goldRelicVisuals[index].GetComponent<MeshRenderer>().sharedMaterials : 
                renownRelicVisuals[index - goldRelicVisuals.Length].GetComponent<MeshRenderer>().sharedMaterials;
        }

        public Sprite GetRelicSprite(int index)
        {
            return index < goldRelicVisuals.Length ? 
                goldRelicSprites[index] : 
                renownRelicSprites[index - goldRelicVisuals.Length];
        }
        
        public string GetRelicName(int index)
        {
            return index < goldRelicVisuals.Length ? 
                goldRelicNames[index] : 
                renownRelicNames[index - goldRelicVisuals.Length];
        }
        
        public void DespawnAllRelics()
        {
            foreach (var relic in Relics)
                Runner.Despawn(relic.Object);
            Relics.Clear();
        }

        public int GetRelicTypeCount()
        {
            return goldRelicVisuals.Length + renownRelicVisuals.Length;
        }

        public int GetGoldRelicCount()
        {
            return goldRelicVisuals.Length;
        }

        public int GetRenownRelicCount()
        {
            return renownRelicVisuals.Length;
        }
    }
}
