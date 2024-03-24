using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class LauncherTrapManager : MonoBehaviour
{
    public GameObject arrowPrefab;
    [FormerlySerializedAs("targetPos")] [SerializeField]
    private GameObject targetObj;  //발판 밟은 오브젝트

    private Transform _launcher;
    private Transform _plate;
    
    [SerializeField]
    private int fireCount;   //발사 횟수
    [SerializeField]
    private float fireInterval; //발사 간격

    
    [SerializeField]
    private float targetPlatePosY;

    private Vector3 _currentDispenserPosition;
    private Vector3 _targetDispenserPosition;

    private float _delta;

    private bool _isCoroutineStarted;

    private void OnEnable()
    {
        _launcher = transform.GetChild(0);
        _plate = transform.GetChild(1);


        _targetDispenserPosition = _launcher.position;
        _targetDispenserPosition.y += 2.2f;

    }

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ghost") || _isCoroutineStarted) 
            return;
        
        targetObj = other.gameObject;
        _isCoroutineStarted = true;
        StartCoroutine(nameof(ActivateTrapCoroutine));
    }
    
    private void FixedUpdate()
    {
        _delta = Time.deltaTime;
        
    }

    

    private IEnumerator ActivateTrapCoroutine()
    {
        yield return StartCoroutine(nameof(PlatePositionChange));
        yield return StartCoroutine(nameof(LauncherPositionChange));
        yield return StartCoroutine(nameof(SpawnArrow));
    }
    
    private IEnumerator PlatePositionChange()
    {
        while (_plate.position.y > targetPlatePosY)
        {
            _plate.position -= Vector3.up * 0.1f * _delta;

            yield return null;
        }
    }

    private IEnumerator LauncherPositionChange() //발사기 이동
    {
        var movePos = new Vector3{y = 0.9f};
        
        while (_launcher.position.y < 0.9f)
        {
            _launcher.position += movePos * _delta;

            yield return null;
        }
    }
    
    private IEnumerator SpawnArrow()   //발사 간격, 발사 횟수에 따라 화살 생성 및 발사
    {
        float intervalCount = 0;
        var count = 0;
        while (true)
        {
            if (intervalCount >= fireInterval || intervalCount == 0)
            {
                intervalCount = 0;
                count++;
                
                var position = targetObj.transform.position;
                var arrow = Instantiate(
                    arrowPrefab, 
                    _launcher.position + Vector3.up * 0.5f, 
                    Quaternion.LookRotation(position));
                
                var ar = arrow.GetComponent<ArrowManager>();
                ar.StartCoroutine(ar.ShootArrow(position, 10f, _delta));
            }
            intervalCount += _delta;
            if(count >= fireCount)
                yield break;

            yield return null;
        }
    }
}
