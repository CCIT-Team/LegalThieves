using Fusion;
using UnityEngine;

namespace LegalThieves
{
    public class TempRelic : NetworkBehaviour
    {
        public uint        relicNumber;
        public TempPlayer  owner; //필요한가?
        public uint        RoomNum { get; set; }
        public uint        GoldPoint { get; private set; }
        public uint        RenownPoint { get; private set; }
        
        public uint        Weight => GoldPoint + RenownPoint; //무게 -> 부피 변경 가능성 있음

        [Networked] private bool IsActive { get; set; }

        public override void Spawned()
        {
            IsActive = true;
        }

        public override void Render()
        {
            gameObject.SetActive(IsActive);
        }

        //유물을 position위치에 rotation 방향으로 이동
        //이동 후 force방향으로 force의 길이만큼 Add force
        //유물을 모두에게 보이도록 IsActive를 true로 변경
        public void SpawnRelic(Vector3 position, Quaternion rotation, Vector3 force)
        {
            if (IsActive || owner == null) return;
            owner = null;
            IsActive = true;
        }

        //플레이어가 유물을 획득할 때 호출
        public void GetRelic(TempPlayer getter)
        {
            if (!IsActive || owner != null) return;
            owner = getter;
            IsActive = false;
        }
    }
}