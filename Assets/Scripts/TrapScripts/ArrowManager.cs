using System.Collections;
using UnityEngine;

namespace TrapScripts
{
    public class ArrowManager : MonoBehaviour
    {
        private void Start()
        {
            Invoke(nameof(DestroyArrow), 5f);
        }

        private void DestroyArrow()
        {
            Destroy(gameObject);
        }

        public IEnumerator ShootArrow(Vector3 targetPos, float speed, float delta)
        {
            const int layerMask = ~(1 << 8);    //  8번 Ghost레이어
            var previousPos = transform.position;   //  이전 위치 초기화
            var targetDir = (targetPos - previousPos).normalized;   //  표적 방향 설정
            while (true)
            {
                var currentPos = transform.position;    //  현재 위치
                var velocity = currentPos - previousPos;    //  방향 초기화
                var distance = speed * delta;   //  프레임당 이동거리

                // 충돌 감지를 위해 Line cast를 수행합니다.
                var hit = Physics.Raycast(previousPos, velocity, out var hitInfo, (velocity.magnitude), layerMask);
                
                //  충돌 시 위치, 방향, 부모 오브젝트 설정 후 루틴 종료
                if (hit)
                {
                    var localTransform = hitInfo.point;
                    transform.SetParent(hitInfo.collider.transform);
                    transform.position = localTransform;
                    //transform.localRotation = Quaternion.Euler(velocity);
                    yield break;
                }
            
                //  위치, 회전 변경
                transform.position += targetDir.normalized * distance;
                if(velocity != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(velocity);
            
                //  이전 위치값에 현재위치 설정
                previousPos = currentPos;

                yield return null;
            }
        }
    }
}
