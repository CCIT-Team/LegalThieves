namespace Fusion.Addons.KCC.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(EKCCAuthorityBehavior))]
	public sealed class KCCAuthorityBehaviorDrawer : PropertyDrawer
	{
		// PRIVATE MEMBERS

		private static readonly int[]        _behaviorIDs   = new int[] { (int)EKCCAuthorityBehavior.PredictFixed_InterpolateRender, (int)EKCCAuthorityBehavior.PredictFixed_PredictRender };
		private static readonly GUIContent[] _behaviorNames = new GUIContent[] { new GUIContent("Predict Fixed   |   Interpolate Render"), new GUIContent("Predict Fixed   |   Predict Render") };

		// PropertyDrawer INTERFACE

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			int storedBehaviorIndex = _behaviorIDs.IndexOf(property.intValue);
			if (storedBehaviorIndex < 0)
			{
				storedBehaviorIndex = 0;
				property.intValue = _behaviorIDs[0];
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}

			int selectedBehaviorIndex = EditorGUI.Popup(position, label, storedBehaviorIndex, _behaviorNames);
			if (selectedBehaviorIndex >= 0 && selectedBehaviorIndex != storedBehaviorIndex)
			{
				property.intValue = _behaviorIDs[selectedBehaviorIndex];
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}
		}
	}
}
