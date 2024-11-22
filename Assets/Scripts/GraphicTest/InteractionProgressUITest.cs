using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionProgressUITest : MonoBehaviour
{
    [SerializeField] private GameObject testingUI;
    [SerializeField] private InteractionProgressUITemp InteractionProgressUITemp;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            testingUI.SetActive(true);
            InteractionProgressUITemp.StartProgress(10f, "���� �Ѽ� ��", "���� �Ѽ� �Ϸ�!");
        }
    }
}
