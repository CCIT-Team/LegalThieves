using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
using LegalThieves;
using System.Linq;
using System;

public class CampPointUI : NetworkBehaviour
{
    [SerializeField] TMP_Text[] playerNames;
    [SerializeField] TMP_Text[] goldPoints;
    [SerializeField] TMP_Text[] remownPoints;
    [SerializeField] GameLogic gameLogic;

    [Networked,Capacity(4)] public NetworkLinkedList<TempPlayer> players { get;}

    

    public void AddPlayer(TempPlayer player)
    {
        players.Add(player);
        playerNames[players.IndexOf(player)].text = player.Name;
        UpdatePlayerName();
    }

    private void UpdatePlayerName()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].Name == "")
                playerNames[i].text = "Player"+ i;
            else
                playerNames[i].text = players[i].Name;
        }
    }

    public void UpdatePlayerPoint(TempPlayer player, NetworkArray<int> DisplayedRelics, NetworkLinkedList<int> Soldrelics)
    {
        float goldPoint = 0;
        float renownPoint = 0;
        if(Soldrelics.Count > 0)
            foreach (var relicID in Soldrelics)
            {
                //goldPoint += RelicManager.instance.GetTempRelicWithIndex(relicID).GoldPoint;
                //renownPoint += RelicManager.instance.GetTempRelicWithIndex(relicID).RenownPoint;
            }
        foreach (var relicID in DisplayedRelics)
        {
            if(relicID != -1)
            {
                //goldPoint += RelicManager.instance.GetTempRelicWithIndex(relicID).GoldPoint;
                //renownPoint += RelicManager.instance.GetTempRelicWithIndex(relicID).RenownPoint;
            }
        }
        renownPoint += gameLogic.ExplainedRooms.Count(a => a == players.IndexOf(player)) * 50;
        goldPoints[players.IndexOf(player)].text = goldPoint.ToString();
        remownPoints[players.IndexOf(player)].text = renownPoint.ToString();
    }
}