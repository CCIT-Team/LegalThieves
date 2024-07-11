namespace Fusion.Addons.KCC
{
	using System;
	using UnityEngine;

	// This file contains remote procedure calls.
	public partial class KCC
	{
		// PUBLIC METHODS

		/// <summary>
		/// Teleport to a specific position with look rotation and immediately synchronize Transform.
		/// This RPC is for input authority only, state authority should use <c>SetPosition()</c> and <c>SetLookRotation()</c> instead.
		/// <c>KCCSettings.AllowClientTeleports</c> must be set to <c>true</c> for this to work.
		/// </summary>
		[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
		public void TeleportRPC(Vector3 position, float lookPitch, float lookYaw)
		{
			if (_settings.AllowClientTeleports == false)
				throw new InvalidOperationException($"{nameof(KCCSettings)}.{nameof(KCCSettings.AllowClientTeleports)} must be enabled to use {nameof(KCC)}.{nameof(TeleportRPC)}().");

			KCCUtility.ClampLookRotationAngles(ref lookPitch, ref lookYaw);

			_renderData.BasePosition       = position;
			_renderData.DesiredPosition    = position;
			_renderData.TargetPosition     = position;
			_renderData.HasTeleported      = true;
			_renderData.IsSteppingUp       = false;
			_renderData.IsSnappingToGround = false;
			_renderData.LookPitch          = lookPitch;
			_renderData.LookYaw            = lookYaw;

			_fixedData.BasePosition       = position;
			_fixedData.DesiredPosition    = position;
			_fixedData.TargetPosition     = position;
			_fixedData.HasTeleported      = true;
			_fixedData.IsSteppingUp       = false;
			_fixedData.IsSnappingToGround = false;
			_fixedData.LookPitch          = lookPitch;
			_fixedData.LookYaw            = lookYaw;

			SynchronizeTransform(_fixedData, true, true, false);
		}
	}
}
