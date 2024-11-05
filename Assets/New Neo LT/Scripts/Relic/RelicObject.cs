using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using New_Neo_LT.Scripts.Game_Play;

public class RelicObject : NetworkBehaviour, IInteractable
{
    public RelicData relicData;
    public string GetInteractPrompt()
    {
        return string.Format("Pickup {0}", relicData.displayName);
    }

    public void OnInteract(NetworkRunner runner)
    {
    
        if (HasStateAuthority)
        {
            Debug.Log($"Picked up {relicData.displayName}");
            if (PlayerRegistry.GetPlayer(runner.LocalPlayer).SetSlot(relicData))
            {
                runner.Despawn(Object);
            }
        }
    }

}
