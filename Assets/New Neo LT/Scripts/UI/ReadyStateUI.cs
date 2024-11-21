using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using New_Neo_LT.Scripts.Game_Play;
using New_Neo_LT.Scripts.PlayerComponent;

public class ReadyStateUI : MonoBehaviour
{
    [SerializeField]
    GameObject UI;

    List<GameObject> readyUIs = new List<GameObject>();
    [Networked, OnChangedRender(nameof(ToggleUI))]
    public NetworkBool IsActive { get; set; } = false;

    public void ToggleReady(PlayerCharacter pc)
    {
        Debug.Log("SETUI" + pc.Index);
        readyUIs[pc.Index].transform.GetChild(0).GetComponent<TMP_Text>().text = pc.IsReady ? "Ready" : "Wait";
    }

    public void PlayerJoined()
    {
        if (PlayerRegistry.Instance == null)
            return;
        //var pColor = pc.Runner.LocalPlayer == pc.Ref ? Color.white : NewGameManager.Instance.playerHairMaterials[pc.GetPlayerColor()].color;
        while(PlayerRegistry.Count > readyUIs.Count)
        {
            Debug.Log(PlayerRegistry.Count+","+ readyUIs.Count);
            GameObject ui = Instantiate(UI, transform.GetChild(0));
            ui.name = "Player" + readyUIs.Count;
            readyUIs.Add(ui);
        }
    }

    public void PlayerLeft(PlayerCharacter pc)
    {
        var ui = readyUIs[pc.Index];
        readyUIs.Remove(ui);
        Destroy(ui);
    }

    void ToggleUI()
    {
        transform.GetChild(0).gameObject.SetActive(IsActive);
        foreach(var ui in readyUIs)
        {
            ui.transform.GetChild(0).GetComponent<TMP_Text>().text = "Ready";
        }
    }
}
