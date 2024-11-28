using New_Neo_LT.Scripts.Game_Play;
using UnityEngine;
using Fusion;
using New_Neo_LT.Scripts.UI;
using UnityEngine.UI;
using New_Neo_LT.Scripts.PlayerComponent;

public class JobChangerUI : MonoBehaviour
{
    PlayerRef _player;

    [SerializeField] private GameObject[] jobButtons;

    [SerializeField] private Transform[] selectedStamp;

    public void SetRenderTexture(Camera[] cameras)
    {
        for (var i = 0; i < jobButtons.Length; i++)
        {
            var renderTexture = cameras[i].targetTexture;
            var rawImage      = jobButtons[i].GetComponent<RawImage>();
            
            rawImage.texture = renderTexture;
        }
    }

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
            UIManager.Instance.jobChangerUI.GetSelectedStamp()[i].gameObject.SetActive(!buttonIndex[i]);
        }
    }
    public void Archaeologist_Click()
    {
        NewGameManager.Instance.RPC_JobChange(_player, Job.Archaeologist, (int)Job.Archaeologist);
        UIManager.Instance.resultUIController.SetSelectAnimation((int)Job.Archaeologist);
    }
    public void Linguist_Click()
    {
        NewGameManager.Instance.RPC_JobChange(_player, Job.Linguist, (int)Job.Linguist);
        UIManager.Instance.resultUIController.SetSelectAnimation((int)Job.Linguist);
    }

    public void BusinessCultist_Click()
    {
        NewGameManager.Instance.RPC_JobChange(_player, Job.BusinessCultist, (int)Job.BusinessCultist);
        UIManager.Instance.resultUIController.SetSelectAnimation((int)Job.BusinessCultist);
    }

    public void Shamanist_Click()
    {
        NewGameManager.Instance.RPC_JobChange(_player, Job.Shamanist, (int)Job.Shamanist);
        UIManager.Instance.resultUIController.SetSelectAnimation((int)Job.Shamanist);
    }

    public Transform[] GetSelectedStamp()
    {
        return selectedStamp;
    }
}
