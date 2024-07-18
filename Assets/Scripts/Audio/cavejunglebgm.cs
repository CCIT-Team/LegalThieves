using LegalThieves;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveJungleBGM : MonoBehaviour
{
    private bool isInCave = true;
    public static int cavein;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if ( other.transform.root.TryGetComponent(out TempPlayer player))
        {
            if (isInCave)
            {
              
                // �÷��̾ ���ۿ��� ������ ��
                AudioManager.instance.PlayJungleBgm(false);
                AudioManager.instance.PlayCaveBgm(true);
                cavein = 1;
            }
            else
            {
                // �÷��̾ �������� ���۷� ����
                AudioManager.instance.PlayCaveBgm(false);
                AudioManager.instance.PlayJungleBgm(true);
                cavein = 0;
            }
            isInCave = !isInCave;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // �÷��̾ Ʈ���Ÿ� ���� �� �߰� ������ �ʿ��ϸ� ���⿡ �ۼ��մϴ�.
    }
}