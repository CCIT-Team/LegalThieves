
using UnityEngine;
using Fusion;
using New_Neo_LT.Scripts.PlayerComponent;
using Unity.Mathematics;


public class ItemHitScan : NetworkBehaviour
{
    [SerializeField] EItemType itemHitType;
    [SerializeField] private ParticleSystem hitEffect;
    [SerializeField] private BoxCollider myCol;
    private void Start()
    {
        myCol = GetComponent<BoxCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (itemHitType)
        {

            case EItemType.Torch:
                if (other.CompareTag("Player"))
                {
                    var player = other.GetComponent<PlayerCharacter>();
                    player.ApplySlow(0.1f);
                    Debug.Log("맞았다리리");

                    // Collider의 ClosestPoint 사용해 위치 계산
                    Vector3 hitPosition = other.ClosestPoint(transform.position);
                    hitEffect.transform.position = hitPosition;
                    hitEffect.Emit(4); // 1개의 입자 방출
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
