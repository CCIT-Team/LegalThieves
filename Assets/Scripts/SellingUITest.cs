using LegalThieves;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SellingUITest : MonoBehaviour
{
    public GameObject inventoryGrid;
    public GameObject sellingTableGrid;

    [SerializeField] private GameObject[] rellicIcons;

    private GameObject[] inventoryRellics = { };
    private GameObject[] sellingRellics = { };

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.I) && rellicIcons.Length <= 20)
        {
            int randomIndex = Random.Range(0, rellicIcons.Length);
            GameObject newRellic = Instantiate(rellicIcons[randomIndex], inventoryGrid.transform);
            inventoryRellics.Append(newRellic);
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
        if (selectedGrid == inventoryGrid) inventoryRellics.Append(rellic);
        else sellingRellics.Append(rellic);
    }

    public void SetInventoryGrid(int[] rellicsInInventory)
    {
        foreach(int rellic in rellicsInInventory)
        {
            //���⼭ �÷��̾� �κ��丮�κ��� UI�� �ش��ϴ� ���� �������� Ž���ؼ� �κ��丮 UI�� ���� ������ UI�� �ڽ����� Instantiate�ϰ� �迭�� �߰�
            UnityEngine.Debug.Log(rellic);
            if (rellic == -1) continue;
            GameObject newRellic = Instantiate(rellicIcons[rellic], inventoryGrid.transform);
            newRellic.name = "RelicIcon";
            newRellic.transform.localScale = new Vector3 (1, 1, rellic);
            inventoryRellics.Append(newRellic);
        }
    }

    public void SellSellingTableRellics()
    {
        var gp = 0;
        var rp = 0;
        foreach (GameObject rellic in inventoryRellics)
        {
            gp += RelicManager.Instance.GetRelicData((int)rellic.transform.localScale.z).GetGoldPoint();
            rp += RelicManager.Instance.GetRelicData((int)rellic.transform.localScale.z).GetRenownPoint();
            //���⼭ ���̺� �� ������ ����Ʈ �ѷ� �ջ�
        }
        
        ClearSellingTableGrid();
        //���⼭ ���̺� �� ������ ����Ʈ �ѷ� ��ȯ
    }

    public void ClearUI()
    {
        ClearSellingTableGrid();
        ClearInventoryGrid();
    }
}
