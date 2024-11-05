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
    [SerializeField] CampPointUI campUI;

    [Networked] public TempPlayer owner { get; set; }

    [Networked, Capacity(30), OnChangedRender(nameof(CallChangeRelicList))]
    NetworkArray<int> DisplayedRelics { get; } = MakeInitializer(Enumerable.Repeat(-1, 30).ToArray());
    int lastrelic;

    [Networked]
    NetworkLinkedList<int> SoldRelics => default;

    int[] explainCount = new int[30];

    

    void CallChangeRelicList()
    {
        campUI.UpdatePlayerPoint(owner, DisplayedRelics, SoldRelics);
    }

    public int AddRelics(int relicID , TempPlayer player)
    {
        if (player != owner || relicID == -1)
            return -1;
        for (int i = 0; i < 30;i++)
        {
            if(DisplayedRelics.Get(i) == -1)
            {
                DisplayedRelics.Set(i, relicID);
                player.playerBoxItems[i] = relicID;

                int roomID = 0;//RelicManager.instance.GetTempRelicWithIndex(relicID).RoomNum;
                if (explainCount.Length < roomID)
                    Array.Resize(ref explainCount, roomID);
                explainCount[roomID] += 1;
                if(explainCount[roomID] >= 3)
                {
                    GameLogic gameLogic = (GameLogic)FindObjectOfType(typeof(GameLogic));
                    gameLogic.ExplainRoom(roomID,this);
                }
                return i; //유물 넣은 인덱스 반환
            }
        }
        return -1; //진열대 초과 시 반환
    }

    public void SellRelics(TempPlayer player)
    {
        if (player != owner)
            return;
        for (int i = 0; i < 30; i++)
        {
            if (DisplayedRelics.Get(i) == -1)
                return;
            SoldRelics.Add(DisplayedRelics.Get(i));
            DisplayedRelics.Set(i, -1);
        }
    }

    public Vector3 GetRelicPosition(int index)
    {
        int x = (int)MathF.Floor(index / 3);
        int y = index % 3;
        return transform.TransformPoint(new Vector3(0.45f -0.1f*x, 0.5f, 0.35f - 0.35f*y));
    }

    public List<int> GetAllRelics()
    {
        List<int> relicList = new List<int>();
        foreach(int id in DisplayedRelics)
        {
            relicList.Add(id);
        }
        foreach (int id in SoldRelics)
        {
            relicList.Add(id);
        }
        return relicList;
    }
}
