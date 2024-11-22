
using New_Neo_LT.Scripts.PlayerComponent;
using New_Neo_LT.Scripts.Relic;
using New_Neo_LT.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using New_Neo_LT.Scripts.Game_Play;
public class PlayerRelicScan : NetworkBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (HasStateAuthority && other.CompareTag("Relics")) // Host
        {
            int relicID = other.GetComponent<RelicObject>().RelicID;
            RPC_SetRelicUI(relicID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (HasStateAuthority && other.CompareTag("Relics")) // Host
        {
            RPC_ClearRelicUI();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void RPC_SetRelicUI(int relicID)
    {
  
        UIManager.Instance.RelicScanUI.SetUIPoint(relicID);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void RPC_ClearRelicUI()
    {
   
        UIManager.Instance.RelicScanUI.SetUIPoint(-1);
    }
}
  

