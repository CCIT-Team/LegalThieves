using UnityEngine;

public class HollywoodTempPlayer : MonoBehaviour
{
    [SerializeField] private Rigidbody   rigidbody;
    [SerializeField] private Animator    animator;
    [SerializeField] private float       speed = 5f;
    [SerializeField] private Transform[] points;

    private Vector3 _startPosition;
    private int     _targetPointIndex;
    private bool    _isActing;
    
    private static readonly int MoveDirX = Animator.StringToHash("MoveDirX");
    private static readonly int MoveDirY = Animator.StringToHash("MoveDirY");

    private void Start()
    {
        _startPosition = transform.position;
        rigidbody ??= GetComponent<Rigidbody>();
        animator ??= GetComponent<Animator>();
        animator.SetFloat("IsCrouchSync", 1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) _isActing = !_isActing;
        
        if(_isActing)
            StartActing();
    }

    private void StartActing()
    {
        if(points[_targetPointIndex] == null)
            return;
        
        var dirVec = points[_targetPointIndex].position - transform.position;
        dirVec.y = 0f;

        transform.position += dirVec.normalized * (speed * Time.deltaTime);
        
        transform.forward += dirVec*(Time.deltaTime);
        
        animator.SetFloat(MoveDirY, _isActing ? 2 : 0);

        if (dirVec.sqrMagnitude < 1f)
        {
            _targetPointIndex++;
        }

        if (points.Length == _targetPointIndex)
        {
            ResetPosition();
            _targetPointIndex = 0;
        }
    }

    private void ResetPosition()
    {
        transform.position = _startPosition;
    }
}
