using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject_k : MonoBehaviour, IInteractable
{
    public itemData_k item;

    public string GetInteractPrompt()
    {
      return string.Format(item.displayName);      
    }
    

    public void OnInteract()
    {
        Destroy(gameObject);
    }


}
