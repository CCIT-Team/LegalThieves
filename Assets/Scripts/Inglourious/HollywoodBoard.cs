using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class HollywoodBoard : MonoBehaviour
{
    [Serializable]
    private struct PointDisplay
    {
        public Transform Rect;
        public TMP_Text Name;
        public TMP_Text Gold;
        public TMP_Text Renown;
        public int      total => int.Parse(Gold.text) + int.Parse(Renown.text);
    }

    [SerializeField] private List<PointDisplay>   pointDisplays;

    private float[] _fixedYPos = { 1.5f, 0.5f, -0.5f, -1.5f };
    private int[][] dp;
    
    private void Awake()
    {
        SortPlayerBoards();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1)) AddPoint(0);
        if (Input.GetKeyDown(KeyCode.Keypad2)) AddPoint(1);
        if (Input.GetKeyDown(KeyCode.Keypad3)) AddPoint(2);
        if (Input.GetKeyDown(KeyCode.Keypad4)) AddPoint(3);
        if (Input.GetKeyDown(KeyCode.Keypad0)) AddPoint(Random.Range(0, 4));


        //UpdateDisplay();
    }

    //private void UpdateDisplay()
    //{
    //    var tempPb = pointDisplays;
    //    tempPb.Sort((a, b) => b.total.CompareTo(a.total));
    //    for (int i = 0; i < 4; i++)
    //    {
    //        pointDisplays[i].Name.text = tempPb[i].Name;
    //        pointDisplays[i].Gold.text = tempPb[i].Gold.ToString();
    //        pointDisplays[i].Renown.text = tempPb[i].Renown.ToString();
    //    }
    //}

    //private void ChangeTotal(int index)
    //{
    //    var pb = playerBoards[index];
    //    pb.Gold += Random.Range(100, 300);
    //    pb.Renown += Random.Range(100, 300);
    //    playerBoards[index] = pb;
    //    UpdateDisplay();
    //}

    private void SortPlayerBoards()
    {
        pointDisplays.Sort((a, b) => b.total.CompareTo(a.total));
        foreach (var pd in pointDisplays)
        {
            //ChangeYPos(pd.Rect, pointDisplays.IndexOf(pd));
            StartCoroutine(ChangeYPosCR(pd.Rect, pointDisplays.IndexOf(pd)));
        }
    }

    private void ChangeYPos(Transform tf, int index)
    {
        tf.localPosition = new Vector3(0f, _fixedYPos[index], 0f);
    }

    private IEnumerator ChangeYPosCR(Transform tf, int index)
    {
        float startTime = Time.time;
        Vector3 startPos = tf.localPosition;
        Vector3 endPos = new Vector3(0, _fixedYPos[index], 0);
        while (Time.time < startTime + 0.3f)
        {
            float t = (Time.time - startTime) / 0.3f;
            tf.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }
        
        tf.localPosition = endPos;
    }

    private void AddPoint(int index)
    {
        var pd = pointDisplays[index];
        pd.Gold.text = (int.Parse(pd.Gold.text) + Random.Range(100, 300)).ToString();
        pd.Renown.text = (int.Parse(pd.Renown.text) + Random.Range(100, 300)).ToString();

        SortPlayerBoards();
    }
}
