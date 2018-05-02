using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace StellarDoor.SOCollection
{
	[CustomPropertyDrawer(typeof(SOCollectionBase), true)]
	public class SOCollectionBaseDrawer : PropertyDrawer 
	{
		int listCount;
		float propHeight = 22;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return propHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			SerializedProperty collection = property.FindPropertyRelative("collection");
			//Debug.Log(collection);

			SerializedProperty listProp = collection.FindPropertyRelative("Array");
			
			float lineH = 20;
			float colW = 25;
			float space = 3;
			Rect firstRect = new Rect(position.x, position.y, position.width - 40, lineH);
			Rect addButRect = new Rect(position.width - colW, position.y, colW, lineH);
			Rect butRect = new Rect(position.x, position.y + space, colW, lineH);
			Rect listRect = new Rect(position.x + colW, position.y + space, position.width - colW, lineH);

			EditorGUI.BeginChangeCheck();

			EditorGUI.PropertyField(firstRect, collection, new GUIContent(property.name + " - drop asset here"));
			
			if (EditorGUI.EndChangeCheck() && listProp.arraySize > listCount)
			{
				//Object has been added to the list by dropping it on the property
				collection.isExpanded = true;
				ScriptableObject obj = listProp.GetArrayElementAtIndex(listCount).objectReferenceValue as ScriptableObject;
				listProp.GetArrayElementAtIndex(listCount).objectReferenceValue = AddObject(obj, property);
			}

			if (GUI.Button(addButRect, "+"))
			{
				//Add a null element to the end of the list
				listProp.arraySize++;
				listProp.GetArrayElementAtIndex(listProp.arraySize - 1).objectReferenceValue = null;
			}
			
			if (collection.isExpanded)
			{
				for (int i = 0; i < listProp.arraySize; i++)
				{
					listRect.y += lineH;
					butRect.y += lineH;
					
					var prop = listProp.FindPropertyRelative(string.Format("data[{0}]", i));

					ScriptableObject oldObj = prop.objectReferenceValue as ScriptableObject;
					EditorGUI.PropertyField(listRect, prop, GUIContent.none);
					ScriptableObject newObj = prop.objectReferenceValue as ScriptableObject;
					if (oldObj != newObj)
					{
						//Object is either being dropped into object element, 
						//or object menu is being used to replace an object on the list
						if (oldObj != null)
							RemoveObject(oldObj, property);
						if (newObj != null)
							prop.objectReferenceValue = AddObject(newObj, property);
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
