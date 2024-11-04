using UnityEngine;

public class LookCam : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null)
        {
            return;
        }
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
