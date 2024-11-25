using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactionUILabel;
    [SerializeField] private TextMeshProUGUI interactionUIKey;

    public void SetInteractionUI(string actionName, string keyName)
    {
        interactionUILabel.text = actionName;
        interactionUIKey.text = keyName;
    }
}
