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
        // ������ �ݱ� ���� ó��
        // ���⼭ ���������� �������� ���ֵ��� �Ͽ� �ٸ� �÷��̾���� ����ȭ�� �����մϴ�.
        if (HasStateAuthority)
        {
            Debug.Log($"Picked up {item.displayName}"); // �ֿܼ� ������ �̸� ���
            // �������� �ݴ� ������ �߰��ϼ��� (��: �÷��̾� �κ��丮�� �߰� ��)

            // ������ ������Ʈ ����
            runner.Despawn(Object); // Fusion�� ����Ͽ� ��Ʈ��ũ���� ������Ʈ ����
        }
    }
}
