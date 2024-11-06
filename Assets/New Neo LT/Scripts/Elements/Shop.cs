using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using New_Neo_LT.Scripts.Relic;

public class Shop : NetworkBehaviour
{
    #region NetworkEvents
    public override void Spawned()
    {
        //Object.geta
    }

    public override void Render()
    {
        
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        
    }
    #endregion


    void Sell(PlayerRef pref, RelicObject[] relics)
    {
        var gp = 0;
        var rp = 0;
        foreach(var relic in relics)
        {
            if (relic == null) continue;
            //gp += relic.goldPoint;
            //rp += relic.renownPoint;
        }
        //NewGameManager.GetPlayer(pref).goldPoint += gp;
        //NewGameManager.GetPlayer(pref).renownPoint += rp;
    }
    

    bool checkItemSet()
    {

        return false;
    }
}
