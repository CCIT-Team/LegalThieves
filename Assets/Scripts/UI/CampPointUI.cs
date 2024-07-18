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
    [SerializeField]
    TMP_Text[] playername;
    [SerializeField]
    TMP_Text[] playerpoint;

    [Networked,Capacity(4)]
    NetworkLinkedList<TempPlayer> players { get;}
    GameLogic gameLogic;

    public void Addplayer(TempPlayer player)
    {
        players.Add(player);
        playername[players.IndexOf(player)].text = player.Name;
        if(gameLogic == null)
            gameLogic = FindObjectOfType<GameLogic>();
    }

    public void UpdatePlayerInfo(TempPlayer player, NetworkArray<int> DisplayedRelics, NetworkLinkedList<int> Soldrelics)
    {
        float goldPoint = 0;
        float renownPoint = 0;
        if(Soldrelics.Count > 0)
            foreach (var relicID in Soldrelics)
            {
                goldPoint += RelicManager.Singleton.GetTempRelicWithIndex(relicID).GoldPoint;
                renownPoint += RelicManager.Singleton.GetTempRelicWithIndex(relicID).RenownPoint;
            }
        foreach (var relicID in DisplayedRelics)
        {
            if(relicID != -1)
            {
                goldPoint += RelicManager.Singleton.GetTempRelicWithIndex(relicID).GoldPoint;
                renownPoint += RelicManager.Singleton.GetTempRelicWithIndex(relicID).RenownPoint;
            }
        }
        renownPoint += gameLogic.ExplainPlayer.Count(a => a == players.IndexOf(player)) * 50;
        playerpoint[players.IndexOf(player)].text = goldPoint + " / " + renownPoint;
    }
}