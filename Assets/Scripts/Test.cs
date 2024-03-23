using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Test : MonoBehaviour
{
    private IEnumerator MoveTest(GameObject obj, Vector3 targetPos, float time)
    {
        if (obj.transform.position == targetPos)
            yield break;
        Vector3 startPosition = obj.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            obj.transform.position = Vector3.Lerp(startPosition, targetPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 최종 위치 보정
        obj.transform.position = targetPos;
    }

    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            StartCoroutine(MoveTest(this.gameObject, new Vector3(5f, 0.5f, 5f), 5f));
        }
        if (Input.GetKeyDown("s"))
        {
            StartCoroutine(MoveTest(this.gameObject, new Vector3(0f, 0.5f, 0f), 5f));
        }
    }
}
