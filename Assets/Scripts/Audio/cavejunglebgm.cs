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
                // �÷��̾ ���ۿ��� ������ ��
                AudioManager.instance.PlayJungleBgm(false);
                AudioManager.instance.PlayCaveBgm(true);
            }   
            else
            {
                // �÷��̾ �������� ���۷� ����
                AudioManager.instance.PlayCaveBgm(false);
                AudioManager.instance.PlayJungleBgm(true);
            }

            isInCave = !isInCave;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �÷��̾ Ʈ���Ÿ� ���� �� �߰� ������ �ʿ��ϸ� ���⿡ �ۼ��մϴ�.
    }
}