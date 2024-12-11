using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EItemType
{
    Null = -1,
    Torch,
    Flashlight,
    Count
}

public abstract class ItemBase : MonoBehaviour
{
    public virtual void UseItem(){}
    public virtual void EquipItem(){}
    public virtual void UnequipItem(){}
}
