using UnityEditor;
using UnityEngine;

namespace StellarDoor.SOCollection
{
	public abstract class SOCollectorEditor<T> : Editor where T : ScriptableObject
	{
		SOCollector<T> manager;
		string newObjectName = "";
		
		bool focusOnNew = true;

		public override void OnInspectorGUI()
		{
			string objectTypeName = typeof(T).Name;

			manager = (SOCollector<T>)target;

			GUILayout.Label("Enter a name to create a new " + objectTypeName);

			EditorGUI.BeginChangeCheck();
			
			GUI.SetNextControlName("newSOName");
			newObjectName = EditorGUILayout.DelayedTextField("New " + objectTypeName, newObjectName);
			
			if (EditorGUI.EndChangeCheck())
			{
				AddSO();
			}

			focusOnNew = EditorPrefs.GetBool("focusOnNew");
			focusOnNew = GUILayout.Toggle(focusOnNew, "Shift focus to new");
			EditorPrefs.SetBool("focusOnNew", focusOnNew);

			// if (GUILayout.Button("Add " + objectTypeName))
			// {
			// 	AddSO();
			// }
			
			DrawDefaultInspector();
		}

		void AddSO()
		{
			if (newObjectName == "")
			{
				//EditorUtility.DisplayDialog("Empty Flag Name", "Please name the Flag.", "OK");
				Debug.LogError("Please name the " + typeof(T).Name + ".");
			}
			else if ((T)manager.collection.Find(F => F.name == newObjectName) != null)
			{
				//EditorUtility.DisplayDialog("Duplicate Flag Name", "A Flag with that name already exists.", "OK");
				Debug.LogError("A Flag with that name already exists!");
			}
			else
			{
				T newObj = ScriptableObject.CreateInstance<T>();
				newObj.name = newObjectName;
				AddObject(newObj);
				// manager.collection.Add(newObj);
				
				// AssetDatabase.AddObjectToAsset(newObj, AssetDatabase.GetAssetPath(manager));
				// AssetDatabase.SaveAssets();
				// AssetDatabase.Refresh();

				newObjectName = "";
				
				if (focusOnNew)
					Selection.SetActiveObjectWithContext(newObj, null);
				else
					EditorGUI.FocusTextInControl("newSOName");
			}
		}
		

		public void AddObject(T obj)
		{
			SOCollector<T> manager = (SOCollector<T>)target;

			manager.collection.Add(obj);
			AssetDatabase.AddObjectToAsset(obj, manager);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			//Debug.Log(AssetDatabase.path(obj));
		}

		public void RemoveObject(T obj)
		{
			SOCollector<T> manager = (SOCollector<T>)target;

			manager.collection.Remove(obj);
			DestroyImmediate(obj, true);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			Selection.SetActiveObjectWithContext(manager, null);
			// AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath())
		}
	}

}
