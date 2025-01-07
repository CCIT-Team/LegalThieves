using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Mono.Cecil.Cil;

public class NavMeshSpawner : MonoBehaviour
{
    public float minDistance = 1.5f;

    private Vector3[] navMeshVertices;
    private int[] navMeshIndices;
    private int[] navMeshAreas;
    Vector3[] relicPosList ;

    private int roomArea;
    private int corridorArea;

    public void Init()
    {
        roomArea = NavMesh.GetAreaFromName("Room");
        corridorArea = NavMesh.GetAreaFromName("Corridor");

        LoadNavMeshData();
    }

    void LoadNavMeshData()
    {
        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
        navMeshVertices = triangulation.vertices;
        navMeshIndices = triangulation.indices;
        navMeshAreas = triangulation.areas;

        Debug.Log("NavMesh 데이터 로드 완료");
    }

    public Vector3[] SpawnRelics(int totalRelics)
    {
        relicPosList = new Vector3[totalRelics];
        int totalWeight = CalculateTotalWeight();

        for (int i = 0; i < totalRelics; i++)
        {
            int triangleIndex = GetWeightedTriangleIndex(totalWeight);

            if (triangleIndex == -1)
            {
                Debug.LogWarning("유효한 삼각형을 찾지 못했습니다.");
                continue;
            }

            Vector3 v1 = navMeshVertices[navMeshIndices[triangleIndex]];
            Vector3 v2 = navMeshVertices[navMeshIndices[triangleIndex + 1]];
            Vector3 v3 = navMeshVertices[navMeshIndices[triangleIndex + 2]];

            Vector3 spawnPosition = GetRandomPointInTriangle(v1, v2, v3);

            if (IsPositionValid(spawnPosition))
            {
                relicPosList[i] = spawnPosition;
            }
            else
            {
                i--;
            }
        }
        Debug.Log($"{totalRelics}개의 유물이 생성되었습니다.");

        return relicPosList;
    }

    int CalculateTotalWeight()
    {
        int totalWeight = 0;
        for (int i = 0; i < navMeshIndices.Length / 3; i++)
        {
            int areaIndex = navMeshAreas[i];
            if (areaIndex == roomArea || areaIndex == corridorArea)
            {
                float cost = NavMesh.GetAreaCost(areaIndex);
                totalWeight += Mathf.RoundToInt(10f / cost);
            }
        }
        return totalWeight;
    }

    int GetWeightedTriangleIndex(int totalWeight)
    {
        int randomValue = Random.Range(0, totalWeight);
        int cumulativeWeight = 0;

        for (int i = 0; i < navMeshIndices.Length / 3; i++)
        {
            int areaIndex = navMeshAreas[i];
            if (areaIndex == roomArea || areaIndex == corridorArea)
            {
                float cost = NavMesh.GetAreaCost(areaIndex);
                int weight = Mathf.RoundToInt(10f / cost);
                cumulativeWeight += weight;

                if (randomValue < cumulativeWeight)
                {
                    return i * 3;
                }
            }
        }

        return -1;
    }

    Vector3 GetRandomPointInTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float r1 = Random.value;
        float r2 = Random.value;

        if (r1 + r2 > 1f)
        {
            r1 = 1f - r1;
            r2 = 1f - r2;
        }

        return v1 + r1 * (v2 - v1) + r2 * (v3 - v1);
    }

    bool IsPositionValid(Vector3 position)
    {
        foreach (Vector3 spawnedPosition in relicPosList)
        {
            if (Vector3.Distance(position, spawnedPosition) < minDistance)
            {
                return false;
            }
        }
        return true;
    }
}
