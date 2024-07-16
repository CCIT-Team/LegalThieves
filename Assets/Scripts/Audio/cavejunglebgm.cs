using LegalThieves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveJungleBGM : MonoBehaviour
{
    private bool isInCave = false;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && rb.gameObject.name == "TempPlayer" && rb.TryGetComponent<TempPlayer>(out TempPlayer player))
        {
            if (isInCave)
            {
                // 플레이어가 정글에서 동굴로 들어감
                AudioManager.instance.PlayJungleBgm(false);
                AudioManager.instance.PlayCaveBgm(true);
            }   
            else
            {
                // 플레이어가 동굴에서 정글로 나감
                AudioManager.instance.PlayCaveBgm(false);
                AudioManager.instance.PlayJungleBgm(true);
            }

            isInCave = !isInCave;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 트리거를 나갈 때 추가 로직이 필요하면 여기에 작성합니다.
    }
}