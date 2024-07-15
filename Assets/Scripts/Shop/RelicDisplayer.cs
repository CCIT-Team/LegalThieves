using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class RelicDisplayer : NetworkBehaviour
{
    int currentPlayer = -1; //-1 : ÀÌ¿כ x
    [Networked, Capacity(120), OnChangedRender(nameof(CallChangeRelicList))]
    NetworkLinkedList<int> Relics => default;
    //0~29, 30~59, 60~89, 90~119

    public override void Render()
    {
        
    }

    void CallChangeRelicList()
    {
        GameManager.Instance.CallVerify();
    }

    public void ContectPlayer(int playerNumber)
    {
        currentPlayer = playerNumber;
    }

    public void DisconnectPlayer()
    {
        currentPlayer = -1;
    }
}
