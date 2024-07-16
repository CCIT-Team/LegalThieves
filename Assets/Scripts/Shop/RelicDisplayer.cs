using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using ExitGames.Client.Photon.StructWrapping;
using System;
using LegalThieves;
using System.Linq;

public class RelicDisplayer : NetworkBehaviour
{
    TempPlayer owner;
    [Networked, Capacity(30), OnChangedRender(nameof(CallChangeRelicList))]
    NetworkArray<int> DisplayedRelics { get; } = MakeInitializer(Enumerable.Repeat(-1, 30).ToArray());

    void CallChangeRelicList()
    {
        //UI 변경 넣을 예정
    }

    public void SetOwner(TempPlayer player)
    {
        owner = player;
    }

    public int AddRelics(int relicID , TempPlayer player)
    {
        if (player != owner || relicID == -1)
            return -1;
        for (int i = 0; i < 30;i++)
        {
            if(DisplayedRelics.Get(i) == -1)
            {
                Debug.Log(i);
                DisplayedRelics.Set(i, relicID);
                return i; //유물 넣은 인덱스 반환
            }
        }
        return -1; //진열대 초과 시 반환
    }

    public List<int> SellRelics()
    {
        List<int> relicList = new List<int>();
        for (int i = 0; i < 30; i++)
        {
            if (DisplayedRelics.Get(i) == -1)
                return relicList;
            relicList.Add(DisplayedRelics.Get(i));
            DisplayedRelics.Set(i, -1);
        }

        return relicList;
    }

    public Vector3 GetRelicPosition(int index)
    {
        int x = (int)MathF.Floor(index / 3);
        int y = index % 3;
        return transform.TransformPoint(new Vector3(0.45f -0.1f*x, 0.35f, 0.35f - 0.35f*y));
    }
}
