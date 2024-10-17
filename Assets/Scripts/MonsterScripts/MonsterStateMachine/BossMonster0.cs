//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class BossMonster0 : MonsterBase
//{   [SerializeField]
//    bool isRoundStart = true;
//    bool isRoundEnd = false;
//    float BossGauge = 0;
//    // Start is called before the first frame update
//    void Start()
//    {

//       Init();
//       StartCoroutine(CheckRoundStart());
//    }

//    // Update is called once per frame
//    IEnumerator CheckRoundStart()
//    {
//        while(true)
//        {
//            if (isRoundStart)
//            {
//                StartCoroutine(BossTimer());
//                break;
//            }
//            yield return 0.5f;
//        }
//    }



//    IEnumerator BossTimer()
//    {
//        while(BossGauge <= 100)
//        {
//            BossGauge++;
//            Debug.Log(BossGauge);
//                switch (BossGauge)
//                {
//                case 30:
//                    //30 경고로직
//                    break;
//                case 60:
//                    //60 경고로직

//                    break;
//                case 90:
//                    //90 경고로직
//                    break;
//            }
//            yield return new WaitForSeconds(2f);
//        }
//        Near = CheckNearPlayer();
//        StartCoroutine(Monster0Move());
//    }

//    IEnumerator Monster0Move() // 쳐다보면 멈추는 초고속 괴물
//    {
//        while (true)
//        {
//            if (isRoundEnd) break;
//            // 적이 플레이어의 화면에 들어왔는지 확인
//            Vector3 screenPoint = _camera.WorldToViewportPoint(transform.position);
            
//            if (screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1 && screenPoint.z > 0)
//            {
//                //화면에 들어오면 진행
//                Physics.Raycast(transform.position + new Vector3(0, 0.5f, 0), Near.position - transform.position, out RaycastHit hit, Mathf.Infinity);
//                //  몹과 플레이어 사이에 벽이 없을 경우에 진행

//                if (hit.collider.gameObject.CompareTag("Player"))
//                {

//                    agent.SetDestination(transform.position);
//                }
//                else
//                {
//                    Near = CheckNearPlayer();
//                    //다시 안보일 경우 쫓아옴
//                    agent.SetDestination(Near.position);
//                }
//            }
//            else
//            {
//                // 화면에 들어오지 않으면 플레이어 쪽으로 이동
//                agent.SetDestination(Near.position);
//            }
//            yield return new WaitForSeconds(.3f);
//        }
//    }
//}
