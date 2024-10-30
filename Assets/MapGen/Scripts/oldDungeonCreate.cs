using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dungeonCreate : MonoBehaviour
{
    [SerializeField] int areaSizeX = 20, areaSizeZ = 20;
    [SerializeField] int cellSize;
    [SerializeField] int cellCount = 10;
    [SerializeField] float minDistance = 4.0f; // �� ���� �ּ� �Ÿ�
    [SerializeField] GameObject mainRoom;
    [SerializeField] GameObject road;
    [SerializeField] GameObject crossRoad;
    [SerializeField] Transform Map;

    Transform parent;
    List<Vector3> RoomDetectionList;
    List<Vector3> roadDetectionList;

    private void Start()
    {
      parent = Instantiate(Map);
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            StartCoroutine(RecreateDungeon());
        }
    }

    IEnumerator RecreateDungeon()
    {
        Destroy(parent.gameObject);
        yield return new WaitForEndOfFrame(); // ������Ʈ�� ������ ������ ������ ���
        parent = Instantiate(Map);
        Create();
    }

    void Create()
    {
        
        RoomDetectionList = new List<Vector3>();
        roadDetectionList = new List<Vector3>();
        // ���� ���� ��ġ ����
        RandomCreate(RoomDetectionList);

        // �� ��ġ
        for (int i = 0; i < cellCount; i++)
        {
            Instantiate(mainRoom, transform.position + RoomDetectionList[i], Quaternion.identity, parent);


        }

        // ���� ��ġ
        for (int i = 0; i < RoomDetectionList.Count - 1; i++)
        {
            Vector3 start = RoomDetectionList[i];
            Vector3 end = RoomDetectionList[i + 1];

            float disX = Mathf.Abs(start.x - end.x);
            float disZ = Mathf.Abs(start.z - end.z);

            // X�� ���� ����
            for (int x = 1; x < disX; x++)
            {
                Vector3 roadPosition = new Vector3(start.x + Mathf.Sign(end.x - start.x) * x, 0, start.z);
                if (!roadDetectionList.Contains(roadPosition) && !RoomDetectionList.Contains(roadPosition))
                {
                    roadDetectionList.Add(roadPosition);
                    Instantiate(road, transform.position + roadPosition, Quaternion.identity, parent);
                }
               
            }

            Vector3 lastPosition = new Vector3(start.x + Mathf.Sign(end.x - start.x) * disX, 0, start.z);
            if (!roadDetectionList.Contains(lastPosition) && !RoomDetectionList.Contains(lastPosition))
            {
                roadDetectionList.Add(lastPosition);
                Instantiate(crossRoad, transform.position + lastPosition, Quaternion.identity, parent);
            }

            // Z�� ���� ����
            for (int z = 1; z < disZ; z++)
            {
                Vector3 roadPosition = new Vector3(end.x, 0, start.z + Mathf.Sign(end.z - start.z) * z);
                if (!roadDetectionList.Contains(roadPosition) && !RoomDetectionList.Contains(roadPosition))
                {
                    roadDetectionList.Add(roadPosition);
                    Instantiate(road, transform.position + roadPosition, Quaternion.AngleAxis(90, Vector3.up), parent);
                }

            }
            lastPosition = new Vector3(end.x, 0, start.z + Mathf.Sign(end.z - start.z) * disZ);
            if (!roadDetectionList.Contains(lastPosition) && !RoomDetectionList.Contains(lastPosition))
            {
                roadDetectionList.Add(lastPosition);
                Instantiate(crossRoad, transform.position + lastPosition, Quaternion.identity, parent);
            }

        }




    }

    // ���� ��ġ ���� �Լ� (��� ��� ����)
    void RandomCreate(List<Vector3> list)
    {

        while (list.Count < cellCount)
        {
            float location_x = Random.Range(cellSize, areaSizeX - cellSize);
            float location_z = Random.Range(cellSize, areaSizeZ - cellSize);

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
    }
}
