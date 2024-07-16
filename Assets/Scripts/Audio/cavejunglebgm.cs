using LegalThieves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveJungleBGM : MonoBehaviour
{
    private bool isInCave = true;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("소리좀 바뀌자00");
        Debug.Log(other.gameObject.name);
        if ( other.transform.root.TryGetComponent(out TempPlayer player))
        {
            Debug.Log("소리좀 바뀌자0");
            if (isInCave)
            {
                Debug.Log("소리좀 바뀌자1");
                // 플레이어가 정글에서 동굴로 들어감
                AudioManager.instance.PlayJungleBgm(false);
                AudioManager.instance.PlayCaveBgm(true);
                Debug.Log("소리좀 바뀌자2");
            }
            else
            {
                Debug.Log("소리좀 바뀌자3");
                // 플레이어가 동굴에서 정글로 나감
                AudioManager.instance.PlayCaveBgm(false);
                AudioManager.instance.PlayJungleBgm(true);
                Debug.Log("소리좀 바뀌자4");
            }
            Debug.Log("소리좀 바뀌자5");
            isInCave = !isInCave;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 플레이어가 트리거를 나갈 때 추가 로직이 필요하면 여기에 작성합니다.
    }
}