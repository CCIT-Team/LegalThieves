using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    private void Start()
    {
        Invoke("DestroyArrow", 5);
    }
    //public void ShootToTarget(Vector3 targetPos, float delta)
    //{
    //    //float distance = Vector3.Distance(transform.position, targetPos);
    //    //Vector3 shotVec = targetPos;
    //    //shotVec.y *= distance;
    //    //targetPos.y *= 1.5f;
    //    //rb.AddForce(targetPos * 1.5f, ForceMode.Impulse);
    //}

    private void DestroyArrow()
    {
        Destroy(gameObject);
    }

    public IEnumerator ShootArrow(Vector3 targetPos, float speed, float delta)
    {
        RaycastHit hit;
        Vector3 changedPos = (targetPos - this.transform.position) * speed * delta;
        
        if(Physics.Linecast(this.transform.position, changedPos, out hit, 8))  //레이어 마스크로 충돌 무시 추가 예정.
        {
            this.transform.position = hit.point;
            this.transform.parent = hit.collider.transform;
            yield break;
        }

        while (true)
        {
            this.transform.position = changedPos;
            yield return null;
        }

    }
}
