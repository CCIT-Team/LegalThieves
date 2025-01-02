
using UnityEngine;
using Fusion;
using New_Neo_LT.Scripts.PlayerComponent;


public class ItemHitScan : NetworkBehaviour
{
    [SerializeField] EItemType itemHitType;
    private void OnTriggerEnter(Collider other)
    {
        switch(itemHitType){

            case EItemType.Torch:
                    if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerCharacter>();
                player.ApplySlow(0.1f);
                Debug.Log("맞았다리리");
            }
            
            // if (other.CompareTag("Bat"))// 박쥐 용
            // {
            //     var player = other.GetComponent<PlayerCharacter>();
            // }
            break;

            case EItemType.WoodStick:

            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<PlayerCharacter>();
                player.ApplySlow(0.1f);
                Debug.Log("맞았다리리");
            }
            break;
        }
    }
}
