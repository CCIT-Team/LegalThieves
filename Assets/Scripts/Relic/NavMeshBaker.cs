using Fusion;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;  // NavMesh ���� Ŭ�������� ���⿡ ����

public class NavMeshBaker : NetworkBehaviour
{
    public Unity.AI.Navigation.NavMeshSurface navMeshSurface;  // UnityEngine.AI.NavMeshSurface ���
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
        Debug.Log("�׺�޽� ����ũ �Ϸ�!");

        NavMeshData navMeshData = navMeshSurface.navMeshData;

        if (navMeshData != null)
        {
            // NavMesh ��� ���� ���
            Bounds bounds = navMeshData.sourceBounds;
            Debug.Log("NavMesh ����:" + bounds);

            // �ﰢ�� ������ ���
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();
            Debug.Log("�ﰢ�� ����:" + triangulation.indices.Length / 3);
            Debug.Log("�ﰢ�� ��ǥ ��: " + triangulation.vertices.Length);

            //// ��� �ﰢ���� �߽� ��ǥ�� ����ġ Ȯ��
            //for (int i = 0; i < triangulation.indices.Length; i += 3)
            //{
            //    // �ﰢ���� �� ���� ��������
            //    Vector3 vertex1 = triangulation.vertices[triangulation.indices[i]];
            //    Vector3 vertex2 = triangulation.vertices[triangulation.indices[i + 1]];
            //    Vector3 vertex3 = triangulation.vertices[triangulation.indices[i + 2]];

            //    // �ﰢ���� �߽� ��ǥ ���
            //    Vector3 triangleCenter = (vertex1 + vertex2 + vertex3) / 3.0f;

            //    // �׺�޽� ���� ��ġ���� ���� ����ġ ��������
            //    NavMeshHit hit;
            //    if (NavMesh.SamplePosition(triangleCenter, out hit, 1.0f, NavMesh.AllAreas))
            //    {
            //        int areaIndex = hit.mask;
            //        float areaCost = NavMesh.GetAreaCost(areaIndex);
            //        Debug.Log($"�ﰢ�� �߽� ��ġ: {triangleCenter}, Area ID: {areaIndex}, ����ġ: {areaCost}");
            //    }
            //    else
            //    {
            //        Debug.LogWarning($"�ﰢ�� �߽� ��ġ {triangleCenter} ��ó���� �׺�޽��� ã�� �� �����ϴ�.");
            //    }
            //}

        }
        else
        {
            Debug.LogWarning("NavMeshData�� �����ϴ�.");
        }
    }
    // Update is called once per frame

}