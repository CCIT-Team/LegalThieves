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

    private void Awake()
    {
        if(rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
        rectTransform.position = new Vector3(Screen.width / 2, Screen.height * 1.5f);
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

        var goal = new Vector3(Screen.width / 2, Screen.height / 2);
        rectTransform.position = new Vector3(Screen.width / 2, Screen.height * 1.5f);

        while ((rectTransform.position - goal).sqrMagnitude > errorScale)
        {
            yield return null;
            rectTransform.position = Vector3.Lerp(rectTransform.position, goal, speed * 0.01f);
        }

        rectTransform.position = goal;

        yield return new WaitForSeconds(New_Neo_LT.Scripts.Game_Play.NewGameManager.Loadtime / 2);

        goal = new Vector3(Screen.width / 2, Screen.height * 1.5f);

        while ((rectTransform.position - goal).sqrMagnitude > errorScale)
        {
            yield return null;
            rectTransform.position = Vector3.Lerp(rectTransform.position, goal, speed * 0.01f);
        }

        rectTransform.position = goal;
        isLooping = false;
    }

    IEnumerator OpenState()
    {
        if (isLooping)
            yield break;

        var goal = new Vector3(Screen.width/2, Screen.height*1.5f);

        while ((rectTransform.position - goal).sqrMagnitude > errorScale)
        {
            yield return null;
            rectTransform.position = Vector3.Lerp(rectTransform.position, goal, speed * 0.01f);
        } 

        rectTransform.position = goal;
        isLooping = false;
    }
}
