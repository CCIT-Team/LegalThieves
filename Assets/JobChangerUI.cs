using New_Neo_LT.Scripts.Game_Play;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using New_Neo_LT.Scripts.PlayerComponent;
using Fusion.Addons.KCC;
using UnityEngine.UI;
public class JobChangerUI : MonoBehaviour
{
    PlayerRef _player;
    
   
    [SerializeField]
    private GameObject[] jobButtons;
    public void JobChangerOpen(PlayerRef player, bool[] buttonIndex)
    {
        _player = player;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        for (int i = 0; i < buttonIndex.Length; i++)
        {
            jobButtons[i].GetComponent<Button>().interactable = buttonIndex[i];
        }
    }
    public void JobChangerRenew(bool[] buttonIndex) {
        for (int i = 0; i < buttonIndex.Length; i++)
        {
            jobButtons[i].GetComponent<Button>().interactable = buttonIndex[i];
        }
    }
    public void Archaeologist_Click()
    {
        NewGameManager.Instance.RPC_JobChange(_player, Job.Archaeologist, (int)Job.Archaeologist);
    }
    public void Linguist_Click()
    {
        NewGameManager.Instance.RPC_JobChange(_player, Job.Linguist, (int)Job.Linguist);
    }

    public void BusinessCultist_Click()
    {
        NewGameManager.Instance.RPC_JobChange(_player, Job.BusinessCultist, (int)Job.BusinessCultist);
    }

    public void Shamanist_Click()
    {
        NewGameManager.Instance.RPC_JobChange(_player, Job.Shamanist, (int)Job.Shamanist);
    }
  
}
