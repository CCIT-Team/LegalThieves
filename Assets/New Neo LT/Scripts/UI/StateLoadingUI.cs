using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateLoadingUI : MonoBehaviour
{
    [SerializeField]
    RectTransform rectTransform;

    [SerializeField,Range(0,100)]
    float speed = 10;

    [SerializeField]
    float errorScale = 10;

    bool isLooping = false;

    RectTransform screenTransform;

    private void Awake()
    {
        if(rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        screenTransform = transform.parent.GetComponent<RectTransform>();
        SetYPos();
    }

    public bool ChangeState(bool isClosed)
    {
        Debug.Log(isClosed);
        if(isClosed)
        {
            StartCoroutine(ToggleState());
        }
        else
        {
            StartCoroutine(OpenState());
        }

        isLooping = true;

        return !isClosed;
    }

    IEnumerator ToggleState()
    {
        if (isLooping)
            yield break;

        var goal = Vector2.zero;
        SetYPos();

        while ((rectTransform.anchoredPosition - goal).sqrMagnitude > errorScale)
        {
            yield return null;
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, goal, speed * 0.01f);
        }

        rectTransform.anchoredPosition = goal;

        yield return new WaitForSeconds(New_Neo_LT.Scripts.Game_Play.NewGameManager.Loadtime / 2);

        goal = new Vector2(0, GetParentUIHeight());

        while ((rectTransform.anchoredPosition - goal).sqrMagnitude > errorScale)
        {
            yield return null;
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, goal, speed * 0.01f);
        }

        rectTransform.anchoredPosition = goal;
        Debug.Log(rectTransform.position);
        Debug.Log(rectTransform.anchoredPosition);
        isLooping = false;
    }

    IEnumerator OpenState()
    {
        if (isLooping)
            yield break;

        var goal = new Vector2(0, GetParentUIHeight());

        while ((rectTransform.anchoredPosition - goal).sqrMagnitude > errorScale)
        {
            yield return null;
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, goal, speed * 0.01f);
        } 

        rectTransform.anchoredPosition = goal;
        isLooping = false;
    }

    public void SetYPos()
    {
        rectTransform.anchoredPosition = new Vector2(0, GetParentUIHeight());
    }

    private float GetParentUIHeight()
    {
        return transform.parent.GetComponent<RectTransform>().rect.height;
    }
}
