using New_Neo_LT.Scripts.Game_Play;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using New_Neo_LT.Scripts.PlayerComponent;
using Fusion.Addons.KCC;
public class JobChangerUI : MonoBehaviour
{
    PlayerCharacter _player;

    public void JobChangerInit(PlayerCharacter player)
    {
        _player = player;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
  
    }

    public void Archaeologist_Click()
    {
        _player.ChangeJob(Job.Archaeologist);
        Debug.Log("Archaeologist");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }
    public void Linguist_Click()
    {
        _player.ChangeJob(Job.Linguist);
        Debug.Log("2");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }

    public void BusinessCultist_Click()
    {
        _player.ChangeJob(Job.BusinessCultist);
        Debug.Log("3");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }

    public void Shamanist_Click()
    {
        _player.ChangeJob(Job.Shamanist);
        Debug.Log("4");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
    }
}
