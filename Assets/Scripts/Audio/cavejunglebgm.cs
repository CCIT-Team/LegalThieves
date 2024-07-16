using LegalThieves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveJungleBGM : MonoBehaviour
{
    private bool isInCave = true;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("�Ҹ��� �ٲ���00");
        Debug.Log(other.gameObject.name);
        if ( other.transform.root.TryGetComponent(out TempPlayer player))
        {
            Debug.Log("�Ҹ��� �ٲ���0");
            if (isInCave)
            {
                Debug.Log("�Ҹ��� �ٲ���1");
                // �÷��̾ ���ۿ��� ������ ��
                AudioManager.instance.PlayJungleBgm(false);
                AudioManager.instance.PlayCaveBgm(true);
                Debug.Log("�Ҹ��� �ٲ���2");
            }
            else
            {
                Debug.Log("�Ҹ��� �ٲ���3");
                // �÷��̾ �������� ���۷� ����
                AudioManager.instance.PlayCaveBgm(false);
                AudioManager.instance.PlayJungleBgm(true);
                Debug.Log("�Ҹ��� �ٲ���4");
            }
            Debug.Log("�Ҹ��� �ٲ���5");
            isInCave = !isInCave;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �÷��̾ Ʈ���Ÿ� ���� �� �߰� ������ �ʿ��ϸ� ���⿡ �ۼ��մϴ�.
    }
}