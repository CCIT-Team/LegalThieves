using System.Collections;
using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

namespace LegalThieves
{
    public class TempRelic : NetworkBehaviour
    {
        public int         relicNumber;
        public GameObject  relicVisual;
        public TempPlayer  owner; //필요한가?
<<<<<<< HEAD
        public int        RoomNum;// { get; set; }
        public int      GoldPoint;// { get; set; }
        public int    RenownPoint;// { get; set; }
        public enum Type { NormalRelic, GoldRelic, RenownRelic }; //유물의 종류
        public Type type;


        public int        Weight => GoldPoint + RenownPoint; //무게 -> 부피 변경 가능성 있음
=======
        public int         RoomNum { get; set; }
        public int         GoldPoint { get; private set; }
        public int         RenownPoint { get; private set; }
        
        public int         Weight => GoldPoint + RenownPoint; //무게 -> 부피 변경 가능성 있음
>>>>>>> Player_Interaction

        private NetworkRigidbody3D  _netRigidBody3D;
        private Collider            _collider;

        [Networked, OnChangedRender(nameof(SetActiveRelic))] private bool IsActive { get; set; }

        public override void Spawned()
        {
            IsActive = true;
            _netRigidBody3D = GetComponent<NetworkRigidbody3D>();
            _collider = GetComponent<Collider>();
        }

        public void SetActiveRelic()
        {
            relicVisual.SetActive(IsActive);
            _collider.enabled = IsActive;
            _netRigidBody3D.RBIsKinematic = !IsActive;
        }

        //유물을 position위치에 rotation 방향으로 이동
        //이동 후 force방향으로 force의 길이만큼 Add force
        //유물을 모두에게 보이도록 IsActive를 true로 변경
        public void SpawnRelic(Vector3 position, Quaternion rotation, Vector3 force)
        {
            if (IsActive || owner == null) return;
            
            owner = null;
            IsActive = true;
            
            _netRigidBody3D.Teleport(position + force, rotation);
            
            //힘 적용 안됨. 나중에 할지동?
            //RPC_ApplyForce(force);
        }

        //플레이어가 유물을 획득할 때 호출
        public void GetRelic(TempPlayer getter)
        {
            if (!IsActive || owner != null) return;
            
            IsActive = false;
            
            owner = getter;
        }


        #region RPC callback

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ApplyForce(Vector3 force)
        {
            // 네트워크 권한이 있는 클라이언트에서만 힘을 적용
            _netRigidBody3D.Rigidbody.AddForce(force * 3f, ForceMode.Impulse);
        }
        
        #endregion
    }
}