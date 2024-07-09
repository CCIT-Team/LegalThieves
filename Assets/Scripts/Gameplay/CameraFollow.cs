using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Singleton
    {
        get => _singleton;
        private set
        {
            if (value == null)
                _singleton = null;
            else if (_singleton == null)
                _singleton = value;
            else if (_singleton != value)
            {
                Destroy(value);
                Debug.LogError($"{nameof(CameraFollow)}는(은) 단 한번만 인스턴싱되어야 합니다!");
            }
        }
    }
    private static CameraFollow _singleton;

    private Transform _target;

    private void Awake()
    {
        Singleton = this;
    }

    private void OnDestroy()
    {
        if (Singleton == this)
            Singleton = null;
    }

    private void LateUpdate()
    {
        if(_target != null)
            transform.SetPositionAndRotation(_target.position, _target.rotation);
    }

    public void SetTarget(Transform newTargetTransform)
    {
        _target = newTargetTransform;
    }
}
