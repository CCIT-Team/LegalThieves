using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using LegalThieves;

public class RelicObject : NetworkBehaviour, IInteractable
{
    public LegalThieves.RelicData relicData;
    public string GetInteractPrompt()
    {
        return string.Format("Pickup {0}", relicData);
    }

    public void OnInteract(NetworkRunner runner)
    {

        Debug.Log($"Picked up {relicData.visualIndex}");
            if (PlayerRegistry.GetPlayer(runner.LocalPlayer).SetSlot(relicData.dataIndex))
            {
         
            RelicManager.instance.DeSpawnRelic(Object);
                //runner.Despawn(Object);
            }
        
    }

}
