
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System;
using System.Net;
using System.IO;
public class Item_Compass : ItemBase
{
//todo 네비메쉬 이용해서 길찾기 보여주는거 구현, 아이템 목록에 추가, 애니메이션 추가
    [SerializeField] GameObject CompassObject;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] Transform playerPos;
    [SerializeField] Transform Entrans;
    [SerializeField] Material pathMaterial;
    Mesh pathMesh;
    public override void Init()
    {
        ID = (int)EItemType.Compass;
    }

    #region ItemBaseLogic
        public override void UseItem(Animator animator, Animator armAnimator)
    {
        UsePathFinding(animator, armAnimator);
    }
    public override void EquipItem(Animator animator, Animator armAnimator)
    {
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(CompassObject, true, 1f));
        animator.SetBool("pickCompass", true);
        armAnimator.SetBool("pickCompass",true);

    }
    public override void UnequipItem(Animator animator, Animator armAnimator)
    {
        animator.SetBool("pickCompass", false);
        armAnimator.SetBool("pickCompass",false);
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(CompassObject, false, 1f));
    }
    #endregion

    public void UsePathFinding(Animator animator, Animator armAnimator)
    {
            animator.SetTrigger("UseItem");
            armAnimator.SetTrigger("UseItem");
            canUse = false;
            PathFinding();
       
            StartCoroutine(CoolDownDelay());
       
    }

    private void PathFinding()
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(playerPos.position, Entrans.position, NavMesh.AllAreas, path))
        {
            // 경로가 유효하면 Mesh 생성
            UpdatePathMesh(path);
        }
   
    }
    void UpdatePathMesh(NavMeshPath path)
    {
        // 경로 코너 가져오기
        Vector3[] corners = path.corners;

        // Vertex 및 Triangle 배열 생성
        Vector3[] vertices = new Vector3[corners.Length * 2];
        int[] triangles = new int[(corners.Length - 1) * 6];
        float lineWidth = 0.2f; // 경로의 두께
        Vector3 offset = Vector3.up * 0.1f; // 바닥에서 살짝 띄우기

        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 left = corners[i] - Vector3.right * lineWidth / 2;
            Vector3 right = corners[i] + Vector3.right * lineWidth / 2;

            vertices[i * 2] = left + offset;
            vertices[i * 2 + 1] = right + offset;

            if (i < corners.Length - 1)
            {
                int baseIndex = i * 6;
                triangles[baseIndex] = i * 2;
                triangles[baseIndex + 1] = i * 2 + 1;
                triangles[baseIndex + 2] = i * 2 + 2;

                triangles[baseIndex + 3] = i * 2 + 1;
                triangles[baseIndex + 4] = i * 2 + 3;
                triangles[baseIndex + 5] = i * 2 + 2;
            }
        }

        // 미리 생성된 Mesh 업데이트
        pathMesh = meshFilter.mesh;
        pathMesh.Clear();
        pathMesh.vertices = vertices;
        pathMesh.triangles = triangles;
        pathMesh.RecalculateNormals();

        // Material 설정
        meshFilter.GetComponent<MeshRenderer>().material = pathMaterial;
    }
}
