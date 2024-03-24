using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowManager : MonoBehaviour
{
    
    private void Start()
    {
        Invoke(nameof(DestroyArrow), 5);
    }

    private void DestroyArrow()
    {
        Destroy(gameObject);
    }

    public IEnumerator ShootArrow(Vector3 targetPos, float speed, float delta)
    {
        var currentPos = transform.position;
        var launchAngle = (targetPos - currentPos).normalized;
        var upForce = Vector3.up;
        while (true)
        {
            //if(Physics.Linecast(transform.position, changedPos, out var hit, 8))
            //{
            //    transform.position = hit.point;
            //    transform.parent = hit.collider.transform;
            //    yield break;
            //}
            
            
            
            transform.position  += launchAngle * (speed * delta);
            
            yield return null;
        }

    }
}
