using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompassNavigationUIBehavior : MonoBehaviour
{
    [SerializeField] private Transform localPlayerTransform;
    [SerializeField] private RawImage CompassRawimage;
    [SerializeField] private RectTransform[] navigationElementTransforms;

    private Vector2 playerRight = new(0, 0);
    private float uiWidth;
    private float uiWidthHalf;

    private const float forwardUV = 0.75f;

    private void Start()
    {
        CompassRawimage.uvRect = new Rect(forwardUV, 0f, 1f, 1f);
        uiWidth = this.GetComponent<RectTransform>().rect.width;
        uiWidthHalf = uiWidth * 0.5f;
    }

    private void Update()
    {
        if(!localPlayerTransform)
            return; 
        
        playerRight = new Vector2(localPlayerTransform.right.x, localPlayerTransform.right.z).normalized;
        SetCompassDir();
        SetNavigation();
    }

    private void SetCompassDir()
    {
        float dir2uv = forwardUV + (localPlayerTransform.localEulerAngles.y / 360f);
        CompassRawimage.uvRect = new Rect(dir2uv, 0f, 1f, 1f);
    }

    private void SetNavigation()
    {
        foreach (RectTransform navigationElementTransform in navigationElementTransforms)
        {
            Vector3 targetWorldPosition = navigationElementTransform.GetComponent<navigationElement>().targetWorldPosition;
            Vector2 targetLocalPosition = new Vector2 ((targetWorldPosition - localPlayerTransform.position).x, (targetWorldPosition - localPlayerTransform.position).z).normalized;
            float dir2XPos = Mathf.Clamp(Vector2.Dot(playerRight, targetLocalPosition), 0.25f, 0.75f) * uiWidth - uiWidthHalf;
            
            navigationElementTransform.localPosition = new Vector3(dir2XPos, navigationElementTransform.localPosition.y, navigationElementTransform.localPosition.z);
        }
    }

    public void SetPlayerTransform( Transform playerTransform)
    {
        localPlayerTransform = playerTransform;
    }
}
