using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using New_Neo_LT.Scripts.Game_Play.Game_State;
using New_Neo_LT.Scripts.PlayerComponent;
using New_Neo_LT.Scripts.UI;
using New_Neo_LT.Scripts.Utilities;
using UnityEngine;

namespace New_Neo_LT.Scripts.Game_Play
{
	public class PlayerRegistry : NetworkBehaviour, INetworkRunnerCallbacks
	{
		private const byte Capacity = 4;

		public int Cap => Capacity;

		public static PlayerRegistry Instance { get; private set; }

		public static int Count => Instance.Players.Count;

		[Networked, Capacity(Capacity), OnChangedRender(nameof(OnPlayerRegistryChange))]
		NetworkDictionary<PlayerRef, PlayerCharacter> Players => default;

		
		public override void Spawned()
		{
			base.Spawned();
			Instance = this;
			Runner.AddCallbacks(this);
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			base.Despawned(runner, hasState);
			Instance = null;
			runner.RemoveCallbacks(this);
		}

		private bool GetAvailable(out byte index)
		{
			if (Players.Count == 0)
			{
				index = 0;
				return true;
			}
			else if (Players.Count == Capacity)
			{
				index = default;
				return false;
			}

			byte[] indices = Players.OrderBy(kvp => kvp.Value.Index).Select(kvp => kvp.Value.Index).ToArray();
		
			for (int i = 0; i < indices.Length - 1; i++)
			{
				if (indices[i + 1] > indices[i] + 1)
				{
					index = (byte)(indices[i] + 1);
					return true;
				}
			}

			index = (byte)(indices[indices.Length - 1] + 1);
			return true;
		}

		public static void Server_Add(NetworkRunner runner, PlayerRef pRef, PlayerCharacter pObj)
		{
			Debug.Assert(runner.IsServer);

			if (Instance.GetAvailable(out byte index))
			{
				Instance.Players.Add(pRef, pObj);
				pObj.Server_Init(pRef, index);
			}
			else
			{
				Debug.LogWarning($"Unable to register player {pRef}", pObj);
			}
		}

		public static void Server_Remove(NetworkRunner runner, PlayerRef pRef)
		{
			Debug.Assert(runner.IsServer);
			Debug.Assert(pRef.IsRealPlayer);

			if (Instance.Players.Remove(pRef) == false)
			{
				Debug.LogWarning("Could not remove player from registry");
			}
		}

		public static bool HasPlayer(PlayerRef pRef)
		{
			return Instance.Players.ContainsKey(pRef);
		}

		public static PlayerCharacter GetPlayer(PlayerRef pRef)
		{
			if (HasPlayer(pRef))
				return Instance.Players.Get(pRef);
			return null;
		}

		public static IEnumerable<PlayerCharacter> Where(System.Predicate<PlayerCharacter> match)
		{
			return Instance.Players.Where(kvp => match.Invoke(kvp.Value)).Select(kvp => kvp.Value);
		}

		public static void ForEach(System.Action<PlayerCharacter> action)
		{
			foreach(var kvp in Instance.Players)
			{
				action.Invoke(kvp.Value);
			}
		}

		public static void ForEachWhere(System.Predicate<PlayerCharacter> match, System.Action<PlayerCharacter> action)
		{
			foreach (var kvp in Instance.Players)
			{
				if (match.Invoke(kvp.Value))
					action.Invoke(kvp.Value);
			}
		}

		public static int CountWhere(System.Predicate<PlayerCharacter> match)
		{
			int count = 0;
			foreach (var kvp in Instance.Players)
			{
				if (match.Invoke(kvp.Value))
					count++;
			}
			return count;
		}

		public static bool Any(System.Predicate<PlayerCharacter> match)
		{
			foreach (var kvp in Instance.Players)
			{
				if (match.Invoke(kvp.Value)) return true;
			}
			return false;
		}

		public static PlayerCharacter GetRandom()
		{
			byte index = (byte)Random.Range(0, Count);
			byte i = 0;
			foreach (var kvp in Instance.Players)
			{
				PlayerCharacter pObj = kvp.Value;
				if (pObj != null)
				{
					if (i == index) return pObj;
					i++;
				}
			}
			throw new System.Exception("Something went inexplicably wrong");
		}

		public static PlayerCharacter[] GetRandom(int count)
		{
			var players = new List<PlayerCharacter>();
			foreach (var kvp in Instance.Players)
			{
				players.Add(kvp.Value);
			}
		
			return players.Grab(count).ToArray();
		}
		
		public static PlayerCharacter[] GetAllPlayers()
		{
			var players = new List<PlayerCharacter>();
			foreach (var kvp in Instance.Players)
			{
				players.Add(kvp.Value);
			}
		
			return players.ToArray();
		}

		public void OnPlayerRegistryChange()
		{
			//UIManager.Instance.scoreRankUI.SetJoinedPlayer(Players);
		}

		void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			if (runner.IsServer)
			{
				Server_Remove(runner, player);

				var jobIndex = GetPlayer(player).GetJobIndex();
				NewGameManager.Instance.EnableJobButton(jobIndex);
			}
		}

		#region INetworkRunnerCallbacks
		void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
		void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
		void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
		void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
		void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
		void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
		void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, Fusion.Sockets.NetAddress remoteAddress, Fusion.Sockets.NetConnectFailedReason reason) { }
		void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
		void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
		void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
		void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }
		void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
		void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

		public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
		}

		public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
		{
		}

		public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
		{
		}

		public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data)
		{
		}

		public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
		{
		}
		#endregion
	}
}