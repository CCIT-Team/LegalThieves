using New_Neo_LT.Scripts.Game_Play;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using New_Neo_LT.Scripts.PlayerComponent;
using Fusion.Addons.KCC;
public class JobChangerUI : MonoBehaviour
{
    PlayerRef _player;

    public void JobChangerOpen(PlayerRef player)
    {
        _player = player;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
  
    }

    public void Archaeologist_Click()
    {
        NewGameManager.Instance.RPC_JobChange(_player, Job.Archaeologist);
        
        Debug.Log("Archaeologist");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }
    public void Linguist_Click()
    {
        NewGameManager.Instance.RPC_JobChange(_player, Job.Linguist);

        Debug.Log("2");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }

    public void BusinessCultist_Click()
    {
        NewGameManager.Instance.RPC_JobChange(_player, Job.BusinessCultist);
     
        Debug.Log("3");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }

    public void Shamanist_Click()
    {
        NewGameManager.Instance.RPC_JobChange(_player, Job.Shamanist);
     
        Debug.Log("4");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }
}
