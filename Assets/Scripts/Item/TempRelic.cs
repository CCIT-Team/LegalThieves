using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;
using UnityEngine.VFX;

namespace LegalThieves
{
    public class TempRelic : NetworkBehaviour
    {
        public RelicData data;
        [SerializeField] Transform visual;

        public override void Spawned()
        {
            
        }

        public override void Render()
        {
            
        }

        //플레이어가 유물을 획득할 때 호출
        public int GetNumber()
        {
            return data.dataIndex;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_ApplyForce(Vector3 force)
        {
            // 네트워크 권한이 있는 클라이언트에서만 힘을 적용
            GetComponent<NetworkRigidbody3D>().Rigidbody.AddForce(force * 3f, ForceMode.Impulse);
        }
    }
}