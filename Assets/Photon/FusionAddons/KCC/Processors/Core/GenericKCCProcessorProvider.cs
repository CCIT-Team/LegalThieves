namespace Fusion.Addons.KCC
{
	using UnityEngine;

	/// <summary>
	/// Generic <c>IKCCProcessor</c> provider which stores processor reference as <c>UnityEngine.Object</c>.
	/// </summary>
	[RequireComponent(typeof(NetworkObject))]
	public sealed class GenericKCCProcessorProvider : MonoBehaviour, IKCCProcessorProvider
	{
		// PRIVATE MEMBERS

		[SerializeField][KCCProcessorReference]
		private Object _processor;

		// IKCCInteractionProvider INTERFACE

		bool IKCCInteractionProvider.CanStartInteraction(KCC kcc, KCCData data) => true;
		bool IKCCInteractionProvider.CanStopInteraction(KCC kcc, KCCData data)  => true;

		// IKCCProcessorProvider INTERFACE

		IKCCProcessor IKCCProcessorProvider.GetProcessor() => KCCUtility.ResolveProcessor(_processor, out IKCCProcessor processor) == true ? processor : default;
	}
}
