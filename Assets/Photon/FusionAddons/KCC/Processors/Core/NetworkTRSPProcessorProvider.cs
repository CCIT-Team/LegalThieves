namespace Fusion.Addons.KCC
{
	using UnityEngine;

	/// <summary>
	/// Default NetworkTRSPProcessor provider.
	/// </summary>
	[RequireComponent(typeof(NetworkObject))]
	public sealed class NetworkTRSPProcessorProvider : MonoBehaviour, IKCCProcessorProvider
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private NetworkTRSPProcessor _processor;

		// IKCCInteractionProvider INTERFACE

		bool IKCCInteractionProvider.CanStartInteraction(KCC kcc, KCCData data) => true;
		bool IKCCInteractionProvider.CanStopInteraction(KCC kcc, KCCData data)  => true;

		// IKCCProcessorProvider INTERFACE

		IKCCProcessor IKCCProcessorProvider.GetProcessor() => _processor;
	}
}
