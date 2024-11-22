using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class navigationElement : MonoBehaviour
{
    [SerializeField] private Transform targetObjectTransform;

    public Vector3 targetWorldPosition
    {
        get
        {   
            if (targetObjectTransform == null) return new Vector3(0, 0, 0);
            else return targetObjectTransform.position;
        }
        set
        {
            if (targetObjectTransform.position == value)
                return;
            targetObjectTransform.position = value;
        }
    }
}
