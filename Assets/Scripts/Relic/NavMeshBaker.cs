using Fusion;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;  // NavMesh 관련 클래스들이 여기에 있음

public class NavMeshBaker : NetworkBehaviour
{
    public Unity.AI.Navigation.NavMeshSurface navMeshSurface;  // UnityEngine.AI.NavMeshSurface 사용
    public LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void BakeNavMesh()
    {
        navMeshSurface.RemoveData();

        navMeshSurface.layerMask = layerMask;

        navMeshSurface.BuildNavMesh();
        OnNavMeshBaked();
    }

    void OnNavMeshBaked()
    {
        Debug.Log("네비메쉬 베이크 완료!");

        NavMeshData navMeshData = navMeshSurface.navMeshData;

        if (navMeshData != null)
        {
            // NavMesh 경계 정보 출력
            Bounds bounds = navMeshData.sourceBounds;
            Debug.Log("NavMesh 범위:" + bounds);

            // 삼각형 데이터 출력
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            Debug.Log("삼각형 개수:" + triangulation.indices.Length / 3);
            Debug.Log("삼각형 좌표 수: " + triangulation.vertices.Length);

            //// 모든 삼각형의 중심 좌표별 가중치 확인
            //for (int i = 0; i < triangulation.indices.Length; i += 3)
            //{
            //    // 삼각형의 세 정점 가져오기
            //    Vector3 vertex1 = triangulation.vertices[triangulation.indices[i]];
            //    Vector3 vertex2 = triangulation.vertices[triangulation.indices[i + 1]];
            //    Vector3 vertex3 = triangulation.vertices[triangulation.indices[i + 2]];

            //    // 삼각형의 중심 좌표 계산
            //    Vector3 triangleCenter = (vertex1 + vertex2 + vertex3) / 3.0f;

            //    // 네비메쉬 샘플 위치에서 영역 가중치 가져오기
            //    NavMeshHit hit;
            //    if (NavMesh.SamplePosition(triangleCenter, out hit, 1.0f, NavMesh.AllAreas))
            //    {
            //        int areaIndex = hit.mask;
            //        float areaCost = NavMesh.GetAreaCost(areaIndex);
            //        Debug.Log($"삼각형 중심 위치: {triangleCenter}, Area ID: {areaIndex}, 가중치: {areaCost}");
            //    }
            //    else
            //    {
            //        Debug.LogWarning($"삼각형 중심 위치 {triangleCenter} 근처에서 네비메쉬를 찾을 수 없습니다.");
            //    }
            //}

        }
        else
        {
            Debug.LogWarning("NavMeshData가 없습니다.");
        }
    }
    // Update is called once per frame

}