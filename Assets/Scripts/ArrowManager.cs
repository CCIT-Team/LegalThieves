using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    Rigidbody rb;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Invoke("DestroyArrow", 5);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Ghost")
        {
            rb.isKinematic = true;
        }
    }

    public void ShootToTarget(Vector3 targetPos)
    {
        float distance = Vector3.Distance(transform.position, targetPos);
        Vector3 shotVec = targetPos;
        shotVec.y *= distance;
        targetPos.y *= 1.5f;
        rb.AddForce(targetPos * 1.5f, ForceMode.Impulse);
    }

    private void DestroyArrow()
    {
        Destroy(gameObject);
    }
}
