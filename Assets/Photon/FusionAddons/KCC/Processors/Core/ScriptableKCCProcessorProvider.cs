namespace Fusion.Addons.KCC
{
	using UnityEngine;
	using Fusion;

	/// <summary>
	/// Default ScriptableKCCProcessor provider.
	/// </summary>
	[RequireComponent(typeof(NetworkObject))]
	public sealed class ScriptableKCCProcessorProvider : MonoBehaviour, IKCCProcessorProvider
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private ScriptableKCCProcessor _processor;

		// IKCCInteractionProvider INTERFACE

		bool IKCCInteractionProvider.CanStartInteraction(KCC kcc, KCCData data) => true;
		bool IKCCInteractionProvider.CanStopInteraction(KCC kcc, KCCData data)  => true;

		// IKCCProcessorProvider INTERFACE

		IKCCProcessor IKCCProcessorProvider.GetProcessor() => _processor;
	}
}
