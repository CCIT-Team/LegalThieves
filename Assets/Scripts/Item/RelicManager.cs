using System;
using System.Collections.Generic;
using System.Dynamic;
using Fusion;
using UnityEngine;
using static RelicCreation;
using Random = UnityEngine.Random;

namespace LegalThieves
{
    public enum RelicType { None = -1, Normal, Gold, Renown, Count } // 유물 타입 열거형

    [Serializable]
    public class RelicData
    {
        public RelicType relicType { get; private set; }
        public int goldPoint { get; private set; }
        public int renownPoint { get; private set; }
        public int visualIndex { get; private set; }
        public int dataIndex { get; set; }


        public RelicData(RelicType type, int gold, int renown,int visual = 0)
        {
            relicType = type;
            goldPoint = gold;
            renownPoint = renown;
            visualIndex = visual;
        }
    }


    public class RelicManager : NetworkBehaviour
    {
        public static RelicManager instance = null;

        [Header("Components")]
        [SerializeField] private NetworkPrefabRef[]  relicPrefab;
        [SerializeField] private TempRoom[]          rooms;
        [SerializeField] private Transform           relicPool;

        [SerializeField]
        List<RelicData> relicDatas = new();
        [Networked, Capacity(200)]
        NetworkLinkedList<TempRelic> relics => default;

        public TextAsset relicDataFile;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this);
        }

        public override void Spawned()
        {
            if (!HasStateAuthority)
                return;

            ReadCSV();
            Debug.Log("CSV 데이터 로드 완료: " + relicDatas.Count.ToString() + "개의 유물 데이터 로드됨");

            if (Runner.Mode != SimulationModes.Host)
                return;
            for (int i = 0; i < 20; i++)
            {
                SpawnRelic(Random.Range(0, relicDatas.Count),new Vector3(Random.Range(-20, 20), 3, Random.Range(-20, 20)));
            }
        }

        void ReadCSV()
        {
            string[] rows = relicDataFile.text.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < rows.Length; i++) // 헤더를 제외하고 시작
            {
                string[] data = rows[i].Split(new char[] { ',' }, StringSplitOptions.None);
                if (!Enum.TryParse(data[0], out RelicType type))
                {
                    Debug.LogError($"Invalid Type on line {i + 1}");
                    continue;
                }

                if (!int.TryParse(data[1], out int goldPoint) ||
                    !int.TryParse(data[2], out int renownPoint)||
                    !int.TryParse(data[3], out int visualIndex))
                {
                    Debug.LogError($"Data format error on line {i + 1}");
                    continue;
                }

                RelicData relic = new RelicData(type, goldPoint,renownPoint, visualIndex);

                relicDatas.Add(relic);
            }
        }

        //생성, 드랍, 진열 -> 스폰
        //줍기 -> 디스폰
        //팔기 -> 포인트


        public TempRelic SpawnRelic(int index = -1, Vector3 position = default)
        {
            var relic = Runner.Spawn(relicPrefab[relicDatas[index].visualIndex-1], position).GetComponent<TempRelic>();
            relic.data = relicDatas[index];
            relics.Add(relic);
            relic.data.dataIndex = relics.Count-1;
            return relic;
        }

        public bool DeSpawn(NetworkObject networkObject)
        {
            TempRelic relic;
            if (networkObject.TryGetComponent<TempRelic>(out relic))
            {
                relics.Remove(relic);
                DeSpawn(networkObject);
                return true;
            }
            return false;
        }

        public RelicData GetRelicData(int index)
        {
            return relicDatas[index];
        }
    }
}
