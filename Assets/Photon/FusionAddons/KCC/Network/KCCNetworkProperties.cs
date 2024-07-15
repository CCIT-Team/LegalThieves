namespace Fusion.Addons.KCC
{
	using System.Collections.Generic;
	using System.Runtime.CompilerServices;
	using UnityEngine;

	public sealed unsafe class KCCNetworkProperties : KCCNetworkProperty<KCCNetworkContext>
	{
		// CONSTANTS

		private const int TRSP_POSITION_ACCURACY   = 1 << 10;
		private const int PROPERTIES_WORD_COUNT    = 10;
		private const int INTERACTIONS_BITS_SHIFT  = 8;
		private const int MAX_INTERACTIONS_SINGLE  = 1 << INTERACTIONS_BITS_SHIFT;
		private const int INTERACTIONS_MASK_SINGLE = MAX_INTERACTIONS_SINGLE - 1;

		private int _maxTotalInteractions;
		private int _interactionsWordCount;

		// CONSTRUCTORS

		public KCCNetworkProperties(KCCNetworkContext context) : base(context, GetTotalWordCount(context))
		{
			GetInteractionsWordCount(context, out _maxTotalInteractions, out _interactionsWordCount);
		}

		// PUBLIC METHODS

		public static void ReadPositions(NetworkBehaviourBuffer fromBuffer, NetworkBehaviourBuffer toBuffer, out Vector3 fromTargetPosition, out Vector3 toTargetPosition)
		{
			fromTargetPosition = fromBuffer.ReinterpretState<NetworkTRSPData>().Position;
			toTargetPosition   = toBuffer.ReinterpretState<NetworkTRSPData>().Position;

			KCCInterpolationInfo interpolationInfo = new KCCInterpolationInfo();
			interpolationInfo.FromBuffer = fromBuffer;
			interpolationInfo.ToBuffer   = toBuffer;
			interpolationInfo.Offset     = NetworkTRSPData.WORDS;

			ReadVector3s(ref interpolationInfo, out Vector3 fromPositionExtension, out Vector3 toPositionExtension);

			fromTargetPosition += fromPositionExtension;
			toTargetPosition   += toPositionExtension;
		}

		public static void ReadTransforms(NetworkBehaviourBuffer fromBuffer, NetworkBehaviourBuffer toBuffer, out Vector3 fromTargetPosition, out Vector3 toTargetPosition, out float fromLookPitch, out float toLookPitch, out float fromLookYaw, out float toLookYaw)
		{
			fromTargetPosition = fromBuffer.ReinterpretState<NetworkTRSPData>().Position;
			toTargetPosition   = toBuffer.ReinterpretState<NetworkTRSPData>().Position;

			int offset = NetworkTRSPData.WORDS + 3; // NetworkTRSPData + Position Extension (3)

			fromLookPitch = fromBuffer.ReinterpretState<float>(offset);
			toLookPitch   = toBuffer.ReinterpretState<float>(offset);

			offset += 1;

			fromLookYaw = fromBuffer.ReinterpretState<float>(offset);
			toLookYaw   = toBuffer.ReinterpretState<float>(offset);
		}

		// KCCNetworkProperty INTERFACE

		public override void Read(int* ptr)
		{
			KCCData       data     = Context.Data;
			KCCSettings   settings = Context.Settings;
			NetworkRunner runner   = Context.KCC.Runner;

			Vector3 basePosition = ((NetworkTRSPData*)ptr)->Position;

			ptr += NetworkTRSPData.WORDS;

			data.TargetPosition = basePosition + ReadVector3(ref ptr);
			data.LookPitch      = ReadFloat(ref ptr);
			data.LookYaw        = ReadFloat(ref ptr);

			int combinedSettings = ReadInt(ref ptr);

			data.IsActive                 = ((combinedSettings >>  0) & 0b1) == 1;
			data.IsGrounded               = ((combinedSettings >>  1) & 0b1) == 1;
			data.WasGrounded              = ((combinedSettings >>  2) & 0b1) == 1;
			data.IsSteppingUp             = ((combinedSettings >>  3) & 0b1) == 1;
			data.WasSteppingUp            = ((combinedSettings >>  4) & 0b1) == 1;
			data.IsSnappingToGround       = ((combinedSettings >>  5) & 0b1) == 1;
			data.WasSnappingToGround      = ((combinedSettings >>  6) & 0b1) == 1;
			data.HasTeleported            = ((combinedSettings >>  7) & 0b1) == 1;
			data.JumpFrames               = ((combinedSettings >>  8) & 0b1);
			settings.IsTrigger            = ((combinedSettings >>  9) & 0b1) == 1;
			settings.AllowClientTeleports = ((combinedSettings >> 10) & 0b1) == 1;

			settings.Shape                  =             (EKCCShape)((combinedSettings >> 11) & 0b11);
			settings.InputAuthorityBehavior = (EKCCAuthorityBehavior)((combinedSettings >> 13) & 0b11);
			settings.StateAuthorityBehavior = (EKCCAuthorityBehavior)((combinedSettings >> 15) & 0b11);
			settings.ProxyInterpolationMode = (EKCCInterpolationMode)((combinedSettings >> 17) & 0b11);
			settings.ColliderLayer          =                        ((combinedSettings >> 19) & 0b11111);
			settings.Features               =          (EKCCFeatures)((combinedSettings >> 24) & 0b11111);

			settings.CollisionLayerMask = ReadInt(ref ptr);

			settings.Radius = ReadFloat(ref ptr);
			settings.Height = ReadFloat(ref ptr);
			settings.Extent = ReadFloat(ref ptr);

			ReadInteractions(runner, data, _maxTotalInteractions, ptr);
			ptr += _interactionsWordCount;
		}

		public override void Write(int* ptr)
		{
			KCCData       data     = Context.Data;
			KCCSettings   settings = Context.Settings;
			NetworkRunner runner   = Context.KCC.Runner;

			Vector3 fullPrecisionPosition = data.TargetPosition;

			NetworkTRSPData* networkTRSPData = (NetworkTRSPData*)ptr;
			networkTRSPData->Parent   = NetworkBehaviourId.None;
			networkTRSPData->Position = fullPrecisionPosition;

			ptr += NetworkTRSPData.WORDS;

			Vector3 positionExtension = default;

			if (settings.CompressNetworkPosition == false)
			{
				Vector3 networkBufferPosition;
				networkBufferPosition.x = FloatUtils.Decompress(FloatUtils.Compress(fullPrecisionPosition.x, TRSP_POSITION_ACCURACY), TRSP_POSITION_ACCURACY);
				networkBufferPosition.y = FloatUtils.Decompress(FloatUtils.Compress(fullPrecisionPosition.y, TRSP_POSITION_ACCURACY), TRSP_POSITION_ACCURACY);
				networkBufferPosition.z = FloatUtils.Decompress(FloatUtils.Compress(fullPrecisionPosition.z, TRSP_POSITION_ACCURACY), TRSP_POSITION_ACCURACY);

				positionExtension = fullPrecisionPosition - networkBufferPosition;
			}

			WriteVector3(positionExtension, ref ptr);
			WriteFloat(data.LookPitch, ref ptr);
			WriteFloat(data.LookYaw, ref ptr);

			int combinedSettings = 0;

			if (data.IsActive                 != default) { combinedSettings |= 1 <<  0; }
			if (data.IsGrounded               != default) { combinedSettings |= 1 <<  1; }
			if (data.WasGrounded              != default) { combinedSettings |= 1 <<  2; }
			if (data.IsSteppingUp             != default) { combinedSettings |= 1 <<  3; }
			if (data.WasSteppingUp            != default) { combinedSettings |= 1 <<  4; }
			if (data.IsSnappingToGround       != default) { combinedSettings |= 1 <<  5; }
			if (data.WasSnappingToGround      != default) { combinedSettings |= 1 <<  6; }
			if (data.HasTeleported            != default) { combinedSettings |= 1 <<  7; }
			if (data.JumpFrames               != default) { combinedSettings |= 1 <<  8; }
			if (settings.IsTrigger            != default) { combinedSettings |= 1 <<  9; }
			if (settings.AllowClientTeleports != default) { combinedSettings |= 1 << 10; }

			combinedSettings |= ((int)settings.Shape                  & 0b11)    << 11;
			combinedSettings |= ((int)settings.InputAuthorityBehavior & 0b11)    << 13;
			combinedSettings |= ((int)settings.StateAuthorityBehavior & 0b11)    << 15;
			combinedSettings |= ((int)settings.ProxyInterpolationMode & 0b11)    << 17;
			combinedSettings |=      (settings.ColliderLayer          & 0b11111) << 19;
			combinedSettings |= ((int)settings.Features               & 0b11111) << 24;

			WriteInt(combinedSettings, ref ptr);
			WriteInt(settings.CollisionLayerMask, ref ptr);

			WriteFloat(settings.Radius, ref ptr);
			WriteFloat(settings.Height, ref ptr);
			WriteFloat(settings.Extent, ref ptr);

			WriteInteractions(runner, data, _maxTotalInteractions, ptr);
			ptr += _interactionsWordCount;
		}

		public override void Interpolate(KCCInterpolationInfo interpolationInfo)
		{
			KCCData       data     = Context.Data;
			KCCSettings   settings = Context.Settings;
			NetworkRunner runner   = Context.KCC.Runner;

			Vector3 fromTargetPosition = interpolationInfo.FromBuffer.ReinterpretState<NetworkTRSPData>().Position;
			Vector3 toTargetPosition   = interpolationInfo.ToBuffer.ReinterpretState<NetworkTRSPData>().Position;

			interpolationInfo.Offset += NetworkTRSPData.WORDS;

			ReadVector3s(ref interpolationInfo, out Vector3 fromPositionExtension, out Vector3 toPositionExtension);

			fromTargetPosition += fromPositionExtension;
			toTargetPosition   += toPositionExtension;

			data.BasePosition    = fromTargetPosition;
			data.DesiredPosition = toTargetPosition;
			data.TargetPosition  = Vector3.Lerp(fromTargetPosition, toTargetPosition, interpolationInfo.Alpha);

			ReadFloats(ref interpolationInfo, out float fromLookPitch, out float toLookPitch);
			data.LookPitch = Mathf.Lerp(fromLookPitch, toLookPitch, interpolationInfo.Alpha);

			ReadFloats(ref interpolationInfo, out float fromLookYaw, out float toLookYaw);
			data.LookYaw = KCCUtility.InterpolateRange(fromLookYaw, toLookYaw, -180.0f, 180.0f, interpolationInfo.Alpha);

			// Following properties are not interpolated, they are set from Read() method.
			// Combined Settings
			// KCCSettings.CollisionLayerMask
			// KCCSettings.Radius
			// KCCSettings.Height
			// KCCSettings.Extent
			interpolationInfo.Offset += 5;
			interpolationInfo.Offset += _interactionsWordCount;

			// Teleport detection.

			int ticks = interpolationInfo.ToBuffer.Tick - interpolationInfo.FromBuffer.Tick;
			if (ticks > 0)
			{
				Vector3 positionDifference = toTargetPosition - fromTargetPosition;
				if (positionDifference.sqrMagnitude > settings.TeleportThreshold * settings.TeleportThreshold * ticks * ticks)
				{
					data.HasTeleported  = true;
					data.TargetPosition = toTargetPosition;
					data.RealVelocity   = Vector3.zero;
					data.RealSpeed      = 0.0f;
				}
				else
				{
					data.RealVelocity = positionDifference / (data.DeltaTime * ticks);
					data.RealSpeed    = data.RealVelocity.magnitude;
				}
			}
			else
			{
				data.RealVelocity = Vector3.zero;
				data.RealSpeed    = 0.0f;
			}
		}

		// PRIVATE METHODS

		private static void ReadInteractions(NetworkRunner runner, KCCData data, int maxTotalInteractions, int* ptr)
		{
			if (maxTotalInteractions <= 0)
				return;

			int  interactionCount = *ptr;
			int* interactionPtr   = ptr + 1;
			int  collisionCount   = (interactionCount >> (INTERACTIONS_BITS_SHIFT * 0)) & INTERACTIONS_MASK_SINGLE;
			int  modifierCount    = (interactionCount >> (INTERACTIONS_BITS_SHIFT * 1)) & INTERACTIONS_MASK_SINGLE;
			int  ignoreCount      = (interactionCount >> (INTERACTIONS_BITS_SHIFT * 2)) & INTERACTIONS_MASK_SINGLE;

			data.Collisions.Clear();
			for (int i = 0; i < collisionCount; ++i)
			{
				KCCNetworkID networkID = KCCNetworkUtility.ReadNetworkID(interactionPtr);
				interactionPtr += KCCNetworkID.WORD_COUNT;

				if (networkID.IsValid == true)
				{
					data.Collisions.Add(KCCNetworkID.GetNetworkObject(runner, networkID), networkID);
				}
			}

			data.Modifiers.Clear();
			for (int i = 0; i < modifierCount; ++i)
			{
				KCCNetworkID networkID = KCCNetworkUtility.ReadNetworkID(interactionPtr);
				interactionPtr += KCCNetworkID.WORD_COUNT;

				if (networkID.IsValid == true)
				{
					data.Modifiers.Add(KCCNetworkID.GetNetworkObject(runner, networkID), networkID);
				}
			}

			data.Ignores.Clear();
			for (int i = 0; i < ignoreCount; ++i)
			{
				KCCNetworkID networkID = KCCNetworkUtility.ReadNetworkID(interactionPtr);
				interactionPtr += KCCNetworkID.WORD_COUNT;

				if (networkID.IsValid == true)
				{
					data.Ignores.Add(KCCNetworkID.GetNetworkObject(runner, networkID), networkID);
				}
			}
		}

		private static void WriteInteractions(NetworkRunner runner, KCCData data, int maxTotalInteractions, int* ptr)
		{
			if (maxTotalInteractions <= 0)
				return;

			int* interactionPtr   = ptr + 1;
			int  interactionCount = 0;
			int  collisionCount   = 0;
			int  modifierCount    = 0;
			int  ignoreCount      = 0;

			if (interactionCount < maxTotalInteractions)
			{
				List<KCCCollision> collisions = data.Collisions.All;
				for (int i = 0, count = collisions.Count; i < count; ++i)
				{
					KCCCollision collision = collisions[i];
					if (collision.NetworkID.IsValid == false)
						continue;

					KCCNetworkUtility.WriteNetworkID(interactionPtr, collision.NetworkID);
					interactionPtr += KCCNetworkID.WORD_COUNT;

					++interactionCount;
					++collisionCount;

					if (collisionCount >= MAX_INTERACTIONS_SINGLE || interactionCount >= maxTotalInteractions)
						break;
				}
			}

			if (interactionCount < maxTotalInteractions)
			{
				List<KCCModifier> modifiers = data.Modifiers.All;
				for (int i = 0, count = modifiers.Count; i < count; ++i)
				{
					KCCModifier modifier = modifiers[i];
					if (modifier.NetworkID.IsValid == false)
						continue;

					KCCNetworkUtility.WriteNetworkID(interactionPtr, modifier.NetworkID);
					interactionPtr += KCCNetworkID.WORD_COUNT;

					++interactionCount;
					++modifierCount;

					if (modifierCount >= MAX_INTERACTIONS_SINGLE || interactionCount >= maxTotalInteractions)
						break;
				}
			}

			if (interactionCount < maxTotalInteractions)
			{
				List<KCCIgnore> ignores = data.Ignores.All;
				for (int i = 0, count = ignores.Count; i < count; ++i)
				{
					KCCIgnore ignore = ignores[i];
					if (ignore.NetworkID.IsValid == false)
						continue;

					KCCNetworkUtility.WriteNetworkID(interactionPtr, ignore.NetworkID);
					interactionPtr += KCCNetworkID.WORD_COUNT;

					++interactionCount;
					++ignoreCount;

					if (ignoreCount >= MAX_INTERACTIONS_SINGLE || interactionCount >= maxTotalInteractions)
						break;
				}
			}

			interactionCount = default;
			interactionCount |= collisionCount << (INTERACTIONS_BITS_SHIFT * 0);
			interactionCount |= modifierCount  << (INTERACTIONS_BITS_SHIFT * 1);
			interactionCount |= ignoreCount    << (INTERACTIONS_BITS_SHIFT * 2);

			*ptr = interactionCount;
		}

		private static void GetInteractionsWordCount(KCCNetworkContext context, out int maxInteractions, out int interactionsWordCount)
		{
			if (context.Settings.NetworkedInteractions > 0)
			{
				maxInteractions       = context.Settings.NetworkedInteractions;
				interactionsWordCount = 1 + KCCNetworkID.WORD_COUNT * maxInteractions;
			}
			else
			{
				maxInteractions       = 0;
				interactionsWordCount = 0;
			}
		}

		private static int GetTotalWordCount(KCCNetworkContext context)
		{
			int wordCount = NetworkTRSPData.WORDS + PROPERTIES_WORD_COUNT;

			GetInteractionsWordCount(context, out int maxInteractions, out int interactionsWordCount);
			wordCount += interactionsWordCount;

			return wordCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int ReadInt(ref int* ptr)
		{
			int value = *ptr;
			++ptr;
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int ReadInt(ref int* ptrFrom, ref int* ptrTo, float alpha)
		{
			int value = alpha < 0.5f ? *ptrFrom : *ptrTo;
			++ptrFrom;
			++ptrTo;
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void WriteInt(int value, ref int* ptr)
		{
			*ptr = value; ++ptr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float ReadFloat(ref int* ptr)
		{
			float value = *(float*)ptr;
			++ptr;
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float ReadFloat(ref int* ptrFrom, ref int* ptrTo, float alpha)
		{
			float value = alpha < 0.5f ? *(float*)ptrFrom : *(float*)ptrTo;
			++ptrFrom;
			++ptrTo;
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void WriteFloat(float value, ref int* ptr)
		{
			*(float*)ptr = value; ++ptr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool CompareAndWriteFloat(float value, ref int* ptr)
		{
			bool result = true;

			if (*(float*)ptr != value)
			{
				*(float*)ptr = value;
				result = false;
			}
			++ptr;

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Vector3 ReadVector3(ref int* ptr)
		{
			Vector3 value;
			value.x = *(float*)ptr; ++ptr;
			value.y = *(float*)ptr; ++ptr;
			value.z = *(float*)ptr; ++ptr;
			return value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void WriteVector3(Vector3 value, ref int* ptr)
		{
			*(float*)ptr = value.x; ++ptr;
			*(float*)ptr = value.y; ++ptr;
			*(float*)ptr = value.z; ++ptr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool CompareAndWriteVector3(Vector3 value, ref int* ptr)
		{
			bool result = true;

			if (*(float*)ptr != value.x)
			{
				*(float*)ptr = value.x;
				result = false;
			}
			++ptr;

			if (*(float*)ptr != value.y)
			{
				*(float*)ptr = value.y;
				result = false;
			}
			++ptr;

			if (*(float*)ptr != value.z)
			{
				*(float*)ptr = value.z;
				result = false;
			}
			++ptr;

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int InterpolateInt(ref KCCInterpolationInfo interpolationInfo)
		{
			int fromValue = interpolationInfo.FromBuffer.ReinterpretState<int>(interpolationInfo.Offset);
			int toValue   = interpolationInfo.ToBuffer.ReinterpretState<int>(interpolationInfo.Offset);

			++interpolationInfo.Offset;

			return interpolationInfo.Alpha < 0.5f ? fromValue : toValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static float InterpolateFloat(ref KCCInterpolationInfo interpolationInfo)
		{
			float fromValue = interpolationInfo.FromBuffer.ReinterpretState<float>(interpolationInfo.Offset);
			float toValue   = interpolationInfo.ToBuffer.ReinterpretState<float>(interpolationInfo.Offset);

			++interpolationInfo.Offset;

			return interpolationInfo.Alpha < 0.5f ? fromValue : toValue;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ReadFloats(ref KCCInterpolationInfo interpolationInfo, out float fromValue, out float toValue)
		{
			fromValue = interpolationInfo.FromBuffer.ReinterpretState<float>(interpolationInfo.Offset);
			toValue   = interpolationInfo.ToBuffer.ReinterpretState<float>(interpolationInfo.Offset);

			++interpolationInfo.Offset;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void ReadVector3s(ref KCCInterpolationInfo interpolationInfo, out Vector3 fromValue, out Vector3 toValue)
		{
			int offset = interpolationInfo.Offset;

			fromValue.x = interpolationInfo.FromBuffer.ReinterpretState<float>(offset + 0);
			fromValue.y = interpolationInfo.FromBuffer.ReinterpretState<float>(offset + 1);
			fromValue.z = interpolationInfo.FromBuffer.ReinterpretState<float>(offset + 2);

			toValue.x = interpolationInfo.ToBuffer.ReinterpretState<float>(offset + 0);
			toValue.y = interpolationInfo.ToBuffer.ReinterpretState<float>(offset + 1);
			toValue.z = interpolationInfo.ToBuffer.ReinterpretState<float>(offset + 2);

			interpolationInfo.Offset += 3;
		}
	}
}
