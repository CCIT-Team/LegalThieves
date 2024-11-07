using System;
using LegalThieves;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using New_Neo_LT.Scripts.Game_Play;
using Fusion;
using New_Neo_LT.Scripts.UI;
using New_Neo_LT.Scripts.PlayerComponent;

public class SellingUITest : MonoBehaviour
{
    public GameObject inventoryGrid;
    public GameObject sellingTableGrid;

    [SerializeField] private GameObject[] rellicIcons;

    private GameObject[] inventoryRellics = { };
    private GameObject[] sellingRellics = { };

    private void Update()
    {
        //if (Input.GetKeyUp(KeyCode.I) && rellicIcons.Length <= 20)
        //{
        //    int randomIndex = Random.Range(0, rellicIcons.Length);
        //    GameObject newRellic = Instantiate(rellicIcons[randomIndex], inventoryGrid.transform);
        //    inventoryRellics.Append(newRellic);
        //}
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        var player = New_Neo_LT.Scripts.UI.UIManager.Instance.GetLocalPlayerTransform();
        SetInventoryGrid(player.GetComponent<PlayerCharacter>().GetInventorys());
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = true;
        //ClearUI();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void ClearInventoryGrid()
    {
        foreach (GameObject rellic in inventoryRellics)
        {
            Destroy(rellic);
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void ClearSellingTableGrid()
    {
        foreach (GameObject rellic in sellingRellics)
        {
            Destroy(rellic);
        }
    }

    public Transform GetSelectedRellic(GameObject rellic)
    {
        rellic.transform.SetParent(GameObject.Find("Canvas").transform);

        return rellic.transform;
        //반환된 Transform으로 마우스 따라댕기게
    }

    public void SetSelectedRellicToGrid(GameObject rellic, GameObject selectedGrid)
    {
        rellic.transform.SetParent(selectedGrid.transform);
        if (selectedGrid == inventoryGrid) inventoryRellics.Append(rellic);
        else sellingRellics.Append(rellic);
    }

    public void SetInventoryGrid(int[] rellicsInInventory)
    {
        foreach(int rellic in rellicsInInventory)
        {
            //여기서 플레이어 인벤토리로부터 UI에 해당하는 유물 아이콘을 탐색해서 인벤토리 UI에 유물 아이콘 UI를 자식으로 Instantiate하고 배열에 추가
            UnityEngine.Debug.Log(rellic);
            if (rellic == -1) continue;
            GameObject newRellic = Instantiate(rellicIcons[LegalThieves.RelicManager.Instance.GetRelicData(rellic).GetTypeIndex()], inventoryGrid.transform);
            newRellic.name = "RelicIcon";
            newRellic.transform.localScale = new Vector3 (1, 1, rellic);
            inventoryRellics.Append(newRellic);
        }
    }

    public void SellSellingTableRellics()
    {
        var gp = 0;
        var rp = 0;
        foreach (GameObject rellic in sellingRellics)
        {
            gp += LegalThieves.RelicManager.Instance.GetRelicData((int)rellic.transform.localScale.z).GetGoldPoint();
            rp += LegalThieves.RelicManager.Instance.GetRelicData((int)rellic.transform.localScale.z).GetRenownPoint();
            //여기서 테이블 위 유물들 포인트 총량 합산
        }
        //New_Neo_LT.Scripts.UI.UIManager.Instance.GetLocalPlayerTransform(out var player);
        //player.GetComponent<PlayerCharacter>().AddGoldPoint(gp);
        //player.GetComponent<PlayerCharacter>().AddRenownPoint(rp);

        ClearSellingTableGrid();
        //여기서 테이블 위 유물들 포인트 총량 반환
    }

    public void ClearUI()
    {
        ClearSellingTableGrid();
        ClearInventoryGrid();
    }
}
