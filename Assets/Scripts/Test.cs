
using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Coroutine _coroutine;
    
    private IEnumerator MoveTest(GameObject obj, Vector3 targetPos, float time)
    {
        
            
        
        var startPosition = obj.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            obj.transform.position = Vector3.Lerp(startPosition, targetPos, elapsedTime / time);


            if (obj.transform.position == targetPos)
            {
                _coroutine = null;
                yield break;
            }
                
            
            yield return null;
        }

        // 최종 위치 보정
        obj.transform.position = targetPos;
    }

    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            
            _coroutine = StartCoroutine(MoveTest(gameObject, new Vector3(5f, 0.5f, 5f), 5f));
        }
        if (Input.GetKeyDown("s"))
        {
            _coroutine = StartCoroutine(MoveTest(gameObject, new Vector3(0f, 0.5f, 0f), 5f));
        }
    }
}
