using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using New_Neo_LT.Scripts.Relic;
using LegalThieves;
using System.Linq;

public class Shop : NetworkBehaviour,IInteractable
{
    [Networked, Capacity(6),SerializeField]
    NetworkArray<NetworkPrefabRef> relicModelPrefab => default;

    [Networked,Capacity(100), SerializeField]
    NetworkLinkedList<NetworkObject> relicModels => default;

    [Networked, Capacity(100),SerializeField]
    NetworkLinkedList<int> ID => default;

    #region NetworkEvents
    //public override void Spawned()
    //{
    //    //Object.geta
    //}

    //public override void Render()
    //{

    //}

    //public override void Despawned(NetworkRunner runner, bool hasState)
    //{

    //}

    #endregion


    //void Sell(PlayerRef pref, RelicObject[] relics)
    //{
    //    var gp = 0;
    //    var rp = 0;
    //    foreach(var relic in relics)
    //    {
    //        if (relic == null) continue;
    //        //gp += relic.goldPoint;
    //        //rp += relic.renownPoint;
    //    }
    //    //NewGameManager.GetPlayer(pref).goldPoint += gp;
    //    //NewGameManager.GetPlayer(pref).renownPoint += rp;
    //}


    //bool checkItemSet()
    //{

    //    return false;
    //}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddRelicModels(ID.ToArray());
            ID.Clear();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_AddPoint(int gold, int renown, PlayerRef pref)
    {
        //PlayerRegistry.GetPlayer(pref).SetPoint(gold,renown);
    }

    public void AddPoint(int gold, int renown, int[] ids)
    {
        foreach(int id in ids)
        {
            ID.Add(id);
        }
        RPC_AddPoint(gold, renown,Runner.LocalPlayer);
        //AddRelicModels(ID.ToArray());
        //ID.Clear();
    }

    public void AddRelicModels(int[] relicids)
    {
        Vector3 startPosition = new Vector3(transform.position.x - transform.localScale.x / 2, transform.position.y - transform.localScale.y / 2, transform.position.z - transform.localScale.z / 2);
        foreach (int relicid in relicids)
        {
            var re = Runner.Spawn(relicModelPrefab[RelicManager.Instance.GetRelicData(relicid).GetTypeIndex()], (startPosition + new Vector3((relicModels.Count + 1) / 3 * 0.6f, 1.5f, (relicModels.Count + 1) % 3)));
            relicModels.Add(re);
            RelicManager.Instance.GetRelicData(relicid).GetTypeIndex();
        }
    }

    public string GetInteractPrompt()
    {
        throw new System.NotImplementedException();
    }

    public void SetPoints()
    {

    }

    public void OnInteract(PlayerRef player)
    {
        PlayerRegistry.GetPlayer(player).ToggleShop(this);
    }
}
