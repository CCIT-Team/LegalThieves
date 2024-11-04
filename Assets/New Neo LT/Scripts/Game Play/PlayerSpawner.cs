using Fusion;

namespace New_Neo_LT.Scripts.Game_Play
{
    public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft
    {
        public NetworkObject playerObject;

        public void PlayerJoined(PlayerRef player)
        {
            // Only the server can spawn.
            if (Runner.IsServer)
            {
                Runner.Spawn(playerObject, position: NewGameManager.Instance.GetPregameSpawnPosition(player.AsIndex), inputAuthority: player);
            }
        }

        public void PlayerLeft(PlayerRef player)
        {
            // Only the server can despawn.
            if (!Runner.IsServer) 
                return;
            
            var leftPlayer = NewGameManager.GetPlayer(player);
            if (leftPlayer != null)
            {
                Runner.Despawn(leftPlayer.Object);
            }
        }
    }
}