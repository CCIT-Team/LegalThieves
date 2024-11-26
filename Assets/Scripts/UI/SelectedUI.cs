using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectedUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private AnimationCurve stampAnimation;

    private float timeCount;

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        timeCount = 0f;
        StartCoroutine(StartAnimation());
        Debug.Log("isStarted");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void SetPlayerNameText(string name)
    {
        playerNameText.text = name;
    }

    private IEnumerator StartAnimation()
    {
        Debug.Log("CoroutinisStarted");
        transform.localScale = stampAnimation.Evaluate(timeCount) * Vector3.one;
        timeCount += Time.deltaTime;
        //if (timeCount >= stampAnimation.keys[stampAnimation.length - 1].time)
        //{
        //    StopAllCoroutines();
        //    yield return null;
        //}
        yield return null;
        StartCoroutine(StartAnimation());
    }
}
