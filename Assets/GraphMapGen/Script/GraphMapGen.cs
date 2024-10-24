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
    [SerializeField] float minDistance = 4.0f; // �� ���� �ּ� �Ÿ�
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
        
            while (list.Count < cellCount)
            {
                int location_x = Random.Range((int)parent.transform.position.x , (int)parent.transform.position.x + area.x );
                int location_z = Random.Range((int)parent.transform.position.y , (int)parent.transform.position.y + area.y );

                Vector3 randomPos = new Vector3(location_x, 0, location_z);

                // ����Ʈ�� �ִ� �ٸ� ���� ����� �Ÿ��� �ִ��� Ȯ��
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

        // �� ��ġ
        for (int i = 0; i < cellCount; i++)
        {
            Instantiate(mainRoom, transform.position + RoomDetectionList[i], Quaternion.identity, parent);


        }

    }


}
