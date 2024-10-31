using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public NetworkObject PlayerPrefab;

    public override void Spawned()
    {
        if (Runner.GameMode == GameMode.Shared)
        {
            // In Shared mode every player spawn the player object on their own.
            SpawnPlayer(Runner.LocalPlayer);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.IsServer == true)
        {
            // With Client-Server topology only the Server spawn player objects.
            // PlayerManager is a special helper class which iterates over list of active players (NetworkRunner.ActivePlayers) and call spawn/despawn callbacks on demand.
            PlayerManager<Player>.UpdatePlayerConnections(Runner, SpawnPlayer, DespawnPlayer);
        }
    }

    private void SpawnPlayer(PlayerRef playerRef)
    {
        // Create a unique position for the player
        Vector3 spawnPosition = new Vector3((playerRef.RawEncoded % Runner.Config.Simulation.PlayerCount) * 3, 1, 0);

        // Spawn the player object with correct input authority.
        NetworkObject player = Runner.Spawn(PlayerPrefab, spawnPosition, Quaternion.identity, playerRef);

        // Set the spawned instance as player object so we can easily get it from other locations using Runner.GetPlayerObject(playerRef).
        // This is optional, but it is a good practice as there is usually 1 main object spawned for each player.
        Runner.SetPlayerObject(playerRef, player);

        // Every player should be always interested to his player object to prevent accidentally getting out of Area of Interest.
        // This is valid only if the Interest Management is enabled in Network Project Config.
        Runner.SetPlayerAlwaysInterested(playerRef, player, true);
    }


    private void DespawnPlayer(PlayerRef playerRef, Player player)
    {
        // We simply despawn the player object. No other cleanup is needed here.
        Runner.Despawn(player.Object);
    }
}
