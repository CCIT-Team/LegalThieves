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
        // 아이템 줍기 로직 처리
        // 여기서 서버에서만 아이템을 없애도록 하여 다른 플레이어와의 동기화를 유지합니다.
        if (HasStateAuthority)
        {
            Debug.Log($"Picked up {relicData.displayName}");
            if (PlayerRegistry.GetPlayer(runner.LocalPlayer).SetSlot(relicData.id))
            {
                runner.Despawn(Object);
            }
        }
    }
}
