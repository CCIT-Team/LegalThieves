using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicObjectDisplaying : MonoBehaviour
{
    [SerializeField][Range(-100f, 100f)] private float rotationSpeed;

    private void Update()
    {
        float yRot = Time.deltaTime * rotationSpeed;
        transform.Rotate(0f, yRot, 0f);
    }
}
