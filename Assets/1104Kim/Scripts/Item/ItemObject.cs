using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ItemObject : NetworkBehaviour, IInteractable
{
    public ItemData item;
    public string GetInteractPrompt()
    {
        return string.Format("Pickup {0}", item.displayName);
    }

    public void OnInteract(NetworkRunner runner)
    {
        // 아이템 줍기 로직 처리
        // 여기서 서버에서만 아이템을 없애도록 하여 다른 플레이어와의 동기화를 유지합니다.
        if (HasStateAuthority)
        {
            Debug.Log($"Picked up {item.displayName}"); // 콘솔에 아이템 이름 출력
            // 아이템을 줍는 로직을 추가하세요 (예: 플레이어 인벤토리에 추가 등)

            // 아이템 오브젝트 삭제
            runner.Despawn(Object); // Fusion을 사용하여 네트워크에서 오브젝트 삭제
        }
    }
}
