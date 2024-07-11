namespace Fusion.Addons.KCC.Editor
{
	using UnityEngine;
	using UnityEditor;

	[CustomPropertyDrawer(typeof(KCCProcessorReferenceAttribute))]
	public sealed class KCCProcessorReferenceDrawer : PropertyDrawer
	{
		// PropertyDrawer INTERFACE

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Object currentObject  = ResolveCurrentObject(property);
			Object selectedObject = EditorGUI.ObjectField(position, label, currentObject, typeof(UnityEngine.Object), true);

			if (ReferenceEquals(selectedObject, currentObject) == true)
				return;

			if (selectedObject == null)
			{
				property.objectReferenceValue = selectedObject;
				EditorUtility.SetDirty(property.serializedObject.targetObject);
				return;
			}

			bool isResolved = KCCUtility.ResolveProcessor(selectedObject, out IKCCProcessor processor, out GameObject gameObject, out Component component, out ScriptableObject scriptableObject);
			if (isResolved == false)
			{
				Debug.LogError($"Failed to resolve {nameof(IKCCProcessor)} in {selectedObject.name} ({selectedObject.GetType().FullName})", selectedObject);
				return;
			}

			if (ReferenceEquals(component, null) == false)
			{
				if (ReferenceEquals(component, currentObject) == false)
				{
					property.objectReferenceValue = component;
					EditorUtility.SetDirty(property.serializedObject.targetObject);
				}
			}
			else if (ReferenceEquals(scriptableObject, null) == false)
			{
				if (ReferenceEquals(scriptableObject, currentObject) == false)
				{
					property.objectReferenceValue = scriptableObject;
					EditorUtility.SetDirty(property.serializedObject.targetObject);
				}
			}
			else
			{
				Debug.LogError($"Failed to resolve serializable Unity object in {selectedObject.name} ({selectedObject.GetType().FullName})", selectedObject);
			}
		}

		// PRIVATE METHODS

		private Object ResolveCurrentObject(SerializedProperty property)
		{
			Object currentObject = property.objectReferenceValue;
			if (ReferenceEquals(currentObject, null) == true)
				return null;

			IKCCProcessor currentProcessor = currentObject as IKCCProcessor;
			if (ReferenceEquals(currentProcessor, null) == false)
			{
				Component currentComponent = currentObject as Component;
				if (ReferenceEquals(currentComponent, null) == false)
					return currentObject;

				ScriptableObject currentScriptableObject = currentObject as ScriptableObject;
				if (ReferenceEquals(currentScriptableObject, null) == false)
					return currentObject;
			}

			bool isResolved  = KCCUtility.ResolveProcessor(currentObject, out IKCCProcessor processor, out GameObject gameObject, out Component component, out ScriptableObject scriptableObject);
			if (isResolved == true)
			{
				if (ReferenceEquals(component, null) == false)
				{
					property.objectReferenceValue = component;
					EditorUtility.SetDirty(property.serializedObject.targetObject);
					return component;
				}
				else if (ReferenceEquals(scriptableObject, null) == false)
				{
					property.objectReferenceValue = scriptableObject;
					EditorUtility.SetDirty(property.serializedObject.targetObject);
					return scriptableObject;
				}
			}

			property.objectReferenceValue = null;
			EditorUtility.SetDirty(property.serializedObject.targetObject);
			return null;
		}
	}
}
