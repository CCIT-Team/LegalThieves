using Fusion;
using New_Neo_LT.Scripts.PlayerComponent;
using New_Neo_LT.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColider : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerCharacter>(out PlayerCharacter player))
        {
            if(transform.root.TryGetComponent(out MonsterBatStateMachine bat))
            {
                Debug.Log(other.gameObject.name + "is stole relic");
                List<int> reliclist = new List<int>();
                for (int i = 0; i < 10; i++)
                {
                    int relicid = player.RelicInventory[i];
                    if (relicid != -1 )
                        reliclist.Add(relicid);
                }
                if (reliclist.Count > 0)
                {
                    bat.currentRelic = player.RelicInventory.IndexOf(reliclist[UnityEngine.Random.Range(0, reliclist.Count)]);
                    player.RemoveRelicFromInventory(bat.currentRelic);
                    CallBatUI(player);
                }
            }
            else
            {
                Debug.Log(other.gameObject.name + "'s points Down");
                player.AddGoldPoint(-300);
                player.AddRenownPoint(-300);
            }
            gameObject.SetActive(false);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All, Channel = RpcChannel.Reliable)]
    void CallBatUI(PlayerCharacter player)
    {
        Debug.Log("IN");
        if(player.HasInputAuthority)
        {
            Debug.Log("INPUT");
            UIManager.Instance.SetActiveUI(UIType.BatAttackUI, false);
            UIManager.Instance.SetActiveUI(UIType.BatAttackUI, true);
        }
    }
}
