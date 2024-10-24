using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;
public class GraphMapGen : MonoBehaviour
{
  //  [SerializeField] DelaunayTriangulation1 DelaunayTriangulation;
    [SerializeField]
    TriangulationTest TriangulationTest;
    [SerializeField] Vector2Int area = new Vector2Int(20,20);
    [SerializeField] int cellSize = 1;
    [SerializeField] int cellCount = 10;
    [SerializeField] List<Vector3>  RoomDetectionList ;
    [SerializeField] float minDistance = 4.0f; // 방 사이 최소 거리
    [SerializeField] Transform Map;
    [SerializeField] GameObject mainRoom;
    [SerializeField] Transform startPoint;
    [SerializeField] int addEdgeCount;
    Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = Instantiate(Map);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RoomDetectionList = new List<Vector3>();
            RoomDetectionList.Add(startPoint.position);
            Destroy(parent.gameObject);
            parent = Instantiate(Map);
            SetRoom(RoomDetectionList);
            // DelaunayTriangulation.Triangulation(RoomDetectionList);
            TriangulationTest.StartTriangulation(RoomDetectionList);
         
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TriangulationTest.CreateMST();
        }
    
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TriangulationTest.AddRandomEdgesToCreateCycle(addEdgeCount);
        }

    }

    private void SetRoom(List<Vector3> list)
    {
        float halfSize = cellSize / 2;
            while (list.Count < cellCount)
            {
                float location_x = Random.Range(parent.transform.position.x + halfSize, parent.transform.position.x + area.x - halfSize);
                float location_z = Random.Range(parent.transform.position.y + halfSize, parent.transform.position.y + area.y - halfSize);

                Vector3 randomPos = new Vector3(location_x, 0, location_z);

                // 리스트에 있는 다른 방들과 충분한 거리가 있는지 확인
                bool isTooClose = false;
                foreach (var existingRoom in list)
                {
                    if (Vector3.Distance(existingRoom, randomPos) < minDistance)
                    {
                        isTooClose = true;
                        break;
                    }
                }

                if (!isTooClose)
                {
                    list.Add(randomPos);
                }
            }

        // 방 배치
        for (int i = 0; i < cellCount; i++)
        {
            Instantiate(mainRoom, transform.position + RoomDetectionList[i], Quaternion.identity, parent);


        }

    }


}
