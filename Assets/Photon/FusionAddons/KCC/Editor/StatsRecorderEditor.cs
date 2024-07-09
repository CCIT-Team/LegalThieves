namespace Fusion.Addons.KCC.Editor
{
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(StatsRecorder), true)]
	public class StatsRecorderInspector : Editor
	{
		// CONSTANTS

		private const string RECORDER_TYPE_PROPERTY_NAME             = "_recorderType";
		private const string REFERENCE_REFRESH_RATE_PROPERTY_NAME    = "_referenceRefreshRate";
		private const string INTERPOLATION_DATA_SOURCE_PROPERTY_NAME = "_interpolationDataSource";

		// PRIVATE MEMBERS

		private static string[] _defaultExcludedProperties = new string[] { INTERPOLATION_DATA_SOURCE_PROPERTY_NAME, REFERENCE_REFRESH_RATE_PROPERTY_NAME };
		private static string[] _monitorExcludedProperties = new string[] { INTERPOLATION_DATA_SOURCE_PROPERTY_NAME };

		// Editor INTERFACE

		public override void OnInspectorGUI()
		{
			StatsRecorder      statsRecorder        = serializedObject.targetObject as StatsRecorder;
			SerializedProperty recorderTypeProperty = serializedObject.FindProperty(RECORDER_TYPE_PROPERTY_NAME);
			ERecorderType      recorderType         = (ERecorderType)recorderTypeProperty.intValue;

			if (recorderType != ERecorderType.None && statsRecorder.IsSupported(recorderType) == false)
			{
				EditorGUILayout.HelpBox($"{recorderType} is not supported by {statsRecorder.GetType().Name}!", MessageType.Error);

				DrawPropertiesExcluding(serializedObject, _defaultExcludedProperties);
			}
			else if (recorderType == ERecorderType.MonitorTime)
			{
				DrawPropertiesExcluding(serializedObject, _monitorExcludedProperties);
			}
			else
			{
				DrawPropertiesExcluding(serializedObject, _defaultExcludedProperties);
			}

			serializedObject.ApplyModifiedProperties();

			if (Application.isPlaying == true)
			{
				EditorGUILayout.Space();

				if (statsRecorder.IsActive == true)
				{
					if (DrawButton("Stop Recording", Color.red) == true)
					{
						statsRecorder.SetActive(false);
					}
				}
				else
				{
					if (DrawButton("Start Recording", Color.green) == true)
					{
						statsRecorder.SetActive(true);
					}
				}
			}
		}

		// PRIVATE METHODS

		private static bool DrawButton(string label, Color color)
		{
			Color backupBackgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			bool result = GUILayout.Button(label);
			GUI.backgroundColor = backupBackgroundColor;
			return result;
		}
	}
}
