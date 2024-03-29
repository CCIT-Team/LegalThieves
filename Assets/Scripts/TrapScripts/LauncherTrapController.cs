using System.Collections;
using UnityEngine;

namespace TrapScripts
{
    public class LauncherTrapController : MonoBehaviour
    {
        public GameObject arrowPrefab;
        private GameObject _targetObj;  //발판 밟은 오브젝트

        private Transform _launcher;
        private Transform _plate;
    
        [SerializeField]
        private int fireCount;   //발사 횟수
        [SerializeField]
        private float fireInterval; //발사 간격
        [SerializeField] 
        private float arrowSpeed;   //발사체 속도

        [SerializeField] 
        private Vector3 targetLauncherPos;
        [SerializeField] 
        private Vector3 targetPlatePos;

        [SerializeField]
        private float plateSpeed;
        [SerializeField]
        private float launcherSpeed;

        private float _delta;
        private bool _isCoroutineStarted;

        private void OnEnable()
        {
            _launcher = transform.GetChild(0);
            _plate = transform.GetChild(1);

        }
        
        private void FixedUpdate()
        {
            _delta = Time.deltaTime;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            
            if (other.gameObject.CompareTag("Ghost") || _isCoroutineStarted) 
                return;
        
            _targetObj = other.gameObject;
            _isCoroutineStarted = true;
            StartCoroutine(nameof(ActivateTrapCoroutine));
        }
    
        
        private IEnumerator ActivateTrapCoroutine()
        {
            yield return StartCoroutine(PositionChange(_plate, targetPlatePos, plateSpeed));
            yield return StartCoroutine(PositionChange(_launcher, targetLauncherPos, launcherSpeed));
            yield return StartCoroutine(nameof(SpawnArrow));
        }
        
        private IEnumerator PositionChange(Transform objectToMove, Vector3 targetPosition, float speed)
        {
            var startPosition = objectToMove.position;
            var currentPosition = startPosition;
            
            var distance = Vector3.Distance(startPosition, targetPosition);
            var timeElapsed = 0f;

            while (timeElapsed < 1f)
            {
                currentPosition = Vector3.Lerp(startPosition, targetPosition, timeElapsed);
                objectToMove.position = currentPosition;

                
                timeElapsed += _delta * speed / distance;

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
                
                    var tarPos = _targetObj.transform.position;
                    var distanceSqr = tarPos - _launcher.position;
                    tarPos += Random.insideUnitSphere * distanceSqr.magnitude * 0.1f;
                    var arrow = Instantiate(
                        arrowPrefab, 
                        _launcher.position, 
                        Quaternion.LookRotation(tarPos));
                
                    var ar = arrow.GetComponent<ArrowManager>();
                    ar.StartCoroutine(ar.ShootArrow(tarPos, arrowSpeed, _delta));
                }
                intervalCount += _delta;
                if(count >= fireCount)
                    yield break;

                yield return null;
            }
        }
    }
}
