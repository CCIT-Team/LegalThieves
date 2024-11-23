using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ELoadType
{
    None = -1,
    Down,
    Toggle,
    Up,
    Count
}

public class StateLoadingUI : MonoBehaviour
{
    [SerializeField]
    RectTransform rectTransform;

    [SerializeField]
    TMP_Text loadingText;

    [SerializeField,Range(0,100)]
    float speed = 10;

    [SerializeField]
    float errorScale = 10;

    bool isLooping = false;

    private void Awake()
    {
        if(rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        SetYPos();
    }

    public void SetLoadingText(string text)
    {
        loadingText.text = text;
    }

    public int ChangeState(ELoadType loadType)
    {
        switch (loadType)
        {
            case ELoadType.Down:
                StartCoroutine(CloseState());
                break;
            case ELoadType.Toggle:
                StartCoroutine(ToggleState());
                break;
            case ELoadType.Up:
                StartCoroutine(OpenState());
                break;
            default:
                break;
        }

        isLooping = true;

        return (int)loadType;
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

        yield return new WaitForSeconds(New_Neo_LT.Scripts.Game_Play.NewGameManager.Loadtime);

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

    IEnumerator CloseState()
    {
        if (isLooping)
            yield break;

        var goal = new Vector2(0,-10);
        SetYPos();

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
