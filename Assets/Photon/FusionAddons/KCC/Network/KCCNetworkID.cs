namespace Fusion.Addons.KCC
{
	using System.Runtime.InteropServices;

	[StructLayout(LayoutKind.Explicit)]
	public unsafe struct KCCNetworkID
	{
		// CONSTANTS

		public const int WORD_COUNT = 2;

		// PUBLIC MEMBERS

		[FieldOffset(0)]
		public uint Value0;
		[FieldOffset(4)]
		public uint Value1;

		public bool IsValid => Value1 != default;

		// PUBLIC METHODS

		public bool Equals(KCCNetworkID other) => Value0 == other.Value0 && Value1 == other.Value1;

		public static KCCNetworkID GetNetworkID(NetworkObject networkObject)
		{
			if (networkObject == null)
				return default;

			KCCNetworkID networkID = new KCCNetworkID();

			if (networkObject.Id.IsValid == true)
			{
				networkID.Value0 = networkObject.Id.Raw;
				networkID.Value1 = 1U;
			}
			else
			{
				NetworkObjectTypeId networkTypeId = networkObject.NetworkTypeId;
				KCCNetworkID networkTypeIdAsKCCNetworkID = *((KCCNetworkID*)&networkTypeId);
				networkID.Value0 = networkTypeIdAsKCCNetworkID.Value0;
				networkID.Value1 = 2U | (networkTypeIdAsKCCNetworkID.Value1 << 2);
			}

			return networkID;
		}

		public static NetworkObject GetNetworkObject(NetworkRunner runner, KCCNetworkID networkID)
		{
			uint type = networkID.Value1 & 3U;
			if (type == 1U)
			{
				NetworkId networkId = new NetworkId();
				networkId.Raw = networkID.Value0;
				return runner.FindObject(networkId);
			}
			else if (type == 2U)
			{
				KCCNetworkID networkIDAsNetworkTypeId = new KCCNetworkID();
				networkIDAsNetworkTypeId.Value0 = networkID.Value0;
				networkIDAsNetworkTypeId.Value1 = networkID.Value1 >> 2;

				NetworkObjectTypeId networkObjectTypeId = *((NetworkObjectTypeId*)&networkIDAsNetworkTypeId);
				if (networkObjectTypeId.IsPrefab == true)
					return runner.Config.PrefabTable.Load(networkObjectTypeId.AsPrefabId, true);
			}

			return default;
		}
	}
}
