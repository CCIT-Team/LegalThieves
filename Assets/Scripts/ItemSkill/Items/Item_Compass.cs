
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
    [SerializeField] GameObject CompassObjectLocal;
    [SerializeField] Transform Entrans;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float effectTime = 5;
    public override void Init()
    {
        ID = (int)EItemType.Compass;
        Entrans = ItemManager.Instance.NavigationPoint;
    }

    #region ItemBaseLogic
    public override void UseItem(Animator animator, Animator armAnimator)
    {
        UsePathFinding(animator, armAnimator);
    }
    public override void EquipItem(Animator animator, Animator armAnimator)
    {
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(CompassObject, true, 1f));
  
            StartCoroutine(ChangeObjectAfterDelay(CompassObjectLocal, true, 1f));

        armAnimator?.SetBool("pickCompass", true);
        animator.SetBool("pickCompass", true);


    }
    public override void UnequipItem(Animator animator, Animator armAnimator)
    {
        animator.SetBool("pickCompass", false);
        armAnimator?.SetBool("pickCompass", false);
        animationCoroutine = StartCoroutine(ChangeObjectAfterDelay(CompassObject, false, 1f));
            StartCoroutine(ChangeObjectAfterDelay(CompassObjectLocal, false, 1f));
        
    }
    #endregion

    public void UsePathFinding(Animator animator, Animator armAnimator)
    {
        animator.SetTrigger("UseItem");
        armAnimator?.SetTrigger("UseItem");

        canUse = false;
        PathFinding();

        StartCoroutine(CoolDownDelay());

    }

    private void PathFinding()
    {
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, Entrans.position, NavMesh.AllAreas, path))
        {
            StartCoroutine(UpdatePathMesh(path));
        }

    }
    IEnumerator UpdatePathMesh(NavMeshPath path)
    {
        lineRenderer.positionCount = path.corners.Length;
        lineRenderer.SetPositions(path.corners);

        float lineLength = 0;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            var temp =  Vector3.Distance(lineRenderer.GetPosition(i), lineRenderer.GetPosition(i + 1));
            lineLength += temp;
        }

        var average = lineLength /  lineRenderer.positionCount;
        var normal = average / lineLength;
        lineRenderer.textureScale = new Vector2(1-normal,1);
    lineRenderer.alignment = LineAlignment.TransformZ;
        lineRenderer.enabled = true;

        yield return new WaitForSeconds(effectTime);
        lineRenderer.enabled=false;
    }
}
