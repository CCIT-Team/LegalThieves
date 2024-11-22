using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassNavigationUIBehavior : MonoBehaviour
{
    [SerializeField] private Transform clientPlayerTransform;
    [SerializeField] private RawImage CompassRawimage;
    [SerializeField] private RectTransform[] navigationElementTransforms;

    private float uiWidth;
    private float uiWidthHalf;

    private const float forwardUV = 0.75f;

    private void Start()
    {
        CompassRawimage.uvRect = new Rect(forwardUV, 0f, 1f, 1f);
        uiWidth = GetChildren(this.transform)[0].GetComponent<RectTransform>().rect.width;
        uiWidthHalf = uiWidth * 0.5f;
    }

    private void Update()
    {
        SetCompassDir();
        SetNavigation();
    }

    private void SetCompassDir()
    {
        float dir2uv = forwardUV + (clientPlayerTransform.localEulerAngles.y / 360f);
        CompassRawimage.uvRect = new Rect(dir2uv, 0f, 1f, 1f);
    }

    private void SetNavigation()
    {
        foreach (RectTransform navigationElementTransform in navigationElementTransforms)
        {
            Vector3 targetWorldPosition = navigationElementTransform.GetComponent<navigationElement>().targetWorldPosition;
            Vector2 targetLocalPosition = new Vector2((targetWorldPosition - clientPlayerTransform.position).x, (targetWorldPosition - clientPlayerTransform.position).z).normalized;
            Vector2 playerForward = new Vector2(clientPlayerTransform.forward.x, clientPlayerTransform.forward.z).normalized;
            Vector2 playerRight = new Vector2(clientPlayerTransform.right.x, clientPlayerTransform.right.z).normalized;


            float xPosOffest = (Vector2.Dot(playerRight, targetLocalPosition) * (Vector2.Dot(playerForward, targetLocalPosition) - 1f)) * uiWidthHalf;
            float dir2xPos = (Vector2.Dot(playerRight, targetLocalPosition) * uiWidthHalf) - xPosOffest;


            navigationElementTransform.localPosition = new Vector3(dir2xPos, 0f, 0f);
        }
    }

    private Transform[] GetChildren(Transform parent)
    {
        Transform[] children = new Transform[parent.childCount];

        for (int i = 0; i < parent.childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }

        return children;
    }
}
