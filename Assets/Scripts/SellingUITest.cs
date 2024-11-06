using LegalThieves;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using TMPro;
using Fusion;

public class SellingUITest : MonoBehaviour
{
    public GameObject inventoryGrid;
    public GameObject sellingTableGrid;
    public TMP_Text sellingPoint;

    [SerializeField] private GameObject[] rellicIcons;

    private GameObject[] inventoryRellics = { };
    private GameObject[] sellingRellics = { };

    [HideInInspector,Networked]
    public Shop shop { get; set; }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.I) && rellicIcons.Length <= 20)
        {
            int randomIndex = Random.Range(0, rellicIcons.Length);
            GameObject newRellic = Instantiate(rellicIcons[randomIndex], inventoryGrid.transform);
            inventoryRellics = inventoryRellics.Append(newRellic).ToArray();
        }
    }

    private void ClearInventoryGrid()
    {
        foreach (GameObject rellic in inventoryRellics)
        {
            Destroy(rellic);
        }
    }

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
        //��ȯ�� Transform���� ���콺 �������
    }

    public void SetSelectedRellicToGrid(GameObject rellic, GameObject selectedGrid)
    {
        rellic.transform.SetParent(selectedGrid.transform);
        if (selectedGrid == inventoryGrid) inventoryRellics = inventoryRellics.Append(rellic).ToArray();
        else sellingRellics = sellingRellics.Append(rellic).ToArray();
    }

    public void SetInventoryGrid(int[] rellicsInInventory)
    {
        foreach(int rellic in rellicsInInventory)
        {
            //���⼭ �÷��̾� �κ��丮�κ��� UI�� �ش��ϴ� ���� �������� Ž���ؼ� �κ��丮 UI�� ���� ������ UI�� �ڽ����� Instantiate�ϰ� �迭�� �߰�
            UnityEngine.Debug.Log(rellic);
            if (rellic == -1) continue;
            GameObject newRellic = Instantiate(rellicIcons[RelicManager.Instance.GetRelicData(rellic).GetTypeIndex()], inventoryGrid.transform);
            newRellic.name = "RelicIcon";
            newRellic.transform.localScale = new Vector3 (1, 1, rellic);
            inventoryRellics = inventoryRellics.Append(newRellic).ToArray();
        }
    }

    public void SellSellingTableRellics()
    {
        List<int> ids = new();
        var gp = 0;
        var rp = 0;
        foreach (GameObject rellic in inventoryRellics)
        {
            ids.Add((int)rellic.transform.localScale.z);
            gp += RelicManager.Instance.GetRelicData((int)rellic.transform.localScale.z).GetGoldPoint();
            rp += RelicManager.Instance.GetRelicData((int)rellic.transform.localScale.z).GetRenownPoint();
            //���⼭ ���̺� �� ������ ����Ʈ �ѷ� �ջ�
        }
        shop.AddPoint(gp,rp, ids.ToArray());

        ClearSellingTableGrid();
        //���⼭ ���̺� �� ������ ����Ʈ �ѷ� ��ȯ
    }

    public void ClearUI()
    {
        ClearSellingTableGrid();
        ClearInventoryGrid();
    }

    public void OffCusorSetting()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
