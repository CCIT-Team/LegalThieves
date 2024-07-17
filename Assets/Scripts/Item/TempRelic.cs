using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

namespace LegalThieves
{
    public class TempRelic : NetworkBehaviour
    {
        [SerializeField] public GameObject visual;
        
        public int         relicNumber;
        public Sprite      relicSprite;
        public TempPlayer  owner; //필요한가?
        [Networked] public int GoldPoint { get; set; }      // 금 포인트->7.18
        [Networked] public int RenownPoint { get; set; }    // 명성 포인트->7.18
        [Networked] public int RoomNum { get; set; }        // 방 번호->7.18
        [Networked] public int RelicNumber { get; set; }    // 유물 번호->7.18

        public enum Type { Normal, GoldRelic, RenownRelic } // 유물 타입->7.18
        [Networked] public Type RelicType { get; set; }     // 네트워크로 동기화할 유물 타입->7.18
        


        public int        Weight => GoldPoint + RenownPoint; //무게 -> 부피 변경 가능성 있음

        private NetworkRigidbody3D  _netRigidbody3D;
        private Collider            _collider;

        [Networked] private bool IsActive { get; set; }

        public override void Spawned()
        {
            IsActive = true;
            _netRigidbody3D = GetComponent<NetworkRigidbody3D>();
            _collider = GetComponent<Collider>();
        }

        public override void Render()
        {
            visual.SetActive(IsActive);
        }

        //유물을 position위치에 rotation 방향으로 이동
        //이동 후 force방향으로 force의 길이만큼 Add force
        //유물을 모두에게 보이도록 IsActive를 true로 변경
        public void SpawnRelic(Vector3 position, Quaternion rotation, Vector3 force)
        {
            if (IsActive || owner == null) return;
            
            owner = null;
            IsActive = true;
            
            _collider.enabled = true;
            _netRigidbody3D.RBIsKinematic = false;
            _netRigidbody3D.Teleport(position + force, rotation);
            RPC_ApplyForce(force);
        }

        //플레이어가 유물을 획득할 때 호출
        public void GetRelic(TempPlayer getter)
        {
            if (!IsActive || owner != null) return;
            
            owner = getter;
            IsActive = false;

            _collider.enabled = false;
            _netRigidbody3D.RBIsKinematic = true;
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ApplyForce(Vector3 force)
        {
            // 네트워크 권한이 있는 클라이언트에서만 힘을 적용
            _netRigidbody3D.Rigidbody.AddForce(force * 3f, ForceMode.Impulse);
        }
    }
}