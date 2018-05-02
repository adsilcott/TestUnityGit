using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StellarDoor.SOCollection
{
	[CustomPropertyDrawer(typeof(SOCollection))]
	public class SOCollectionDrawer : PropertyDrawer 
	{
		int listCount;
		float propHeight = 22;

		const float lineH = 20;
		const float colA = 25;
		const float pickW = 36;
		const float space = 3;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return propHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty collection = property.FindPropertyRelative("collection");

			SerializedProperty listProp = collection.FindPropertyRelative("Array");
			
			float colB = position.width - colA - pickW;
			Rect firstRect = new Rect(position.x, position.y, position.width - 90, lineH);

			Rect addButRect = new Rect(position.width - 80, position.y, 35, lineH);
			Rect cloneButRect = new Rect(position.width - 45, position.y, 45, lineH);

			Rect butRect = new Rect(position.x, position.y + space, colA, lineH);
			Rect nameRect = new Rect(position.x + colA, position.y + space, colB, lineH);
			Rect listRect = new Rect(position.x + colA + colB, position.y + space, pickW, lineH);

			EditorGUI.BeginChangeCheck();

			EditorGUI.PropertyField(firstRect, collection, new GUIContent(property.name)); // + " - drop here"
			
			if (EditorGUI.EndChangeCheck() && listProp.arraySize > listCount)
			{
				//Object has been added to the list by dropping it on the property
				collection.isExpanded = true;
				ScriptableObject obj = listProp.GetArrayElementAtIndex(listCount).objectReferenceValue as ScriptableObject;
				if (AssetDatabase.IsMainAsset(obj))
					listProp.GetArrayElementAtIndex(listCount).objectReferenceValue = AddObject(obj, property);
				else
				{
					listProp.GetArrayElementAtIndex(listCount).objectReferenceValue = null;
					listProp.arraySize--;
					Debug.LogWarning("Can't add an object that's already in a collection.");
				}
			}

			if (GUI.Button(addButRect, "Add"))
			{
				//Add a null element to the end of the list
				listProp.arraySize++;
				listProp.GetArrayElementAtIndex(listProp.arraySize - 1).objectReferenceValue = null;
			}

			if (GUI.Button(cloneButRect, "Clone"))
			{
				//Add a duplicate element to the end of the list
				Object newObj = (listProp.arraySize == 0) ? null : ScriptableObject.Instantiate(listProp.GetArrayElementAtIndex(listProp.arraySize - 1).objectReferenceValue);
				
				if (newObj != null)
					newObj = AddObject(newObj as ScriptableObject, property);

				listProp.arraySize++;
				listProp.GetArrayElementAtIndex(listProp.arraySize - 1).objectReferenceValue = newObj;
			}
			
			if (collection.isExpanded)
			{
				for (int i = 0; i < listProp.arraySize; i++)
				{
					listRect.y += lineH;
					butRect.y += lineH;
					nameRect.y += lineH;
					
					var prop = listProp.FindPropertyRelative(string.Format("data[{0}]", i));

					ScriptableObject oldObj = prop.objectReferenceValue as ScriptableObject;
					EditorGUI.PropertyField(listRect, prop, GUIContent.none);
					ScriptableObject newObj = prop.objectReferenceValue as ScriptableObject;
					if (oldObj != newObj)
					{
						//Object is either being dropped into object element, 
						//or object menu is being used to replace an object on the list
						if (AssetDatabase.IsMainAsset(newObj))
						{
							if (oldObj != null)
								RemoveObject(oldObj, property);

							if (newObj != null)
								prop.objectReferenceValue = AddObject(newObj, property);
						}
						else
						{
							prop.objectReferenceValue = oldObj;
							Debug.LogWarning("Can't add an object that's already in a collection.");
						}
					}

					EditorGUI.BeginChangeCheck();
					if (prop.objectReferenceValue != null)
						prop.objectReferenceValue.name = EditorGUI.DelayedTextField(nameRect, prop.objectReferenceValue.name);
					else
						EditorGUI.DelayedTextField(nameRect, "");

					if (EditorGUI.EndChangeCheck())
					{
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
					}
					
					if (GUI.Button(butRect, "X"))
					{
						//Object is being removed from the list
						ScriptableObject obj = prop.objectReferenceValue as ScriptableObject;
						prop.objectReferenceValue = null;
						listProp.DeleteArrayElementAtIndex(i);
						i++;
						if (obj != null)
							RemoveObject(obj, property);
					}
				}
			}

			propHeight = (listRect.y + lineH + space) - position.y;
			listCount = listProp.arraySize;
		}

		ScriptableObject AddObject(ScriptableObject obj, SerializedProperty property)
		{
			ScriptableObject copy = Editor.Instantiate(obj);
			copy.name = copy.name.Replace("(Clone)", "");
			AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(obj));
			AssetDatabase.AddObjectToAsset(copy, AssetDatabase.GetAssetPath(property.serializedObject.targetObject));
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			return copy;
		}

		void RemoveObject(ScriptableObject obj, SerializedProperty property)
		{
			ScriptableObject copy = Editor.Instantiate(obj);
			Editor.DestroyImmediate(obj, true);

			//Make sure this is a reference to the main asset object in case this object was added to a nested SOCollector
			Object mainObject = property.serializedObject.targetObject;
			if (!AssetDatabase.IsMainAsset(mainObject))
				mainObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(property.serializedObject.targetObject));
			string assetPath = AssetDatabase.GetAssetPath(mainObject).Replace(mainObject.name + ".asset", "") + copy.name.Replace("(Clone)", ".asset");

			//Objects can have the same name in an asset file, but they have to be unique once removed
			string newPath = assetPath;
			int duplicateNum = 1;
			while (AssetDatabase.LoadAssetAtPath<ScriptableObject>(newPath) != null && duplicateNum < 20) //20 = Sanity check, 
				newPath = assetPath.Insert(assetPath.Length - 6, (duplicateNum++.ToString()));
			
			AssetDatabase.CreateAsset(copy, newPath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}

}
