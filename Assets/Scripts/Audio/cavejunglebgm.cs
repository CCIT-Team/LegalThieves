using Fusion;
using LegalThieves;
using New_Neo_LT.Scripts.PlayerComponent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaveJungleBGM : MonoBehaviour
{
    [Networked,OnChangedRender(nameof(ChangeSoundPack))]
    private int changeTrigger { get; set; } = 0;
    private bool isInCave;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out PlayerCharacter player))
        {
            if (!player.HasInputAuthority)
                return;

            ChangeSoundPack(player);
        }
    }

    private void ChangeSoundPack(PlayerCharacter player)
    {
        if (!player.HasInputAuthority)
            return;

        if (isInCave)
        {
            AudioManager.instance.SetSoundPack(AudioManager.instance.themes[(int)EField.Temple]);
        }
        else
        {
            AudioManager.instance.SetSoundPack(AudioManager.instance.themes[(int)EField.Camp]);
        }
        AudioManager.instance.PlayLoop(ESoundType.Ambiance);
        isInCave = !isInCave;
    }
}