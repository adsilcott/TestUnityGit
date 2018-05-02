using UnityEditor;
using UnityEngine;

namespace StellarDoor.SOCollection
{
	public abstract class SOCollectibleEditor<T> : Editor where T : ScriptableObject
	{

		bool isNew = true;

		// static SOCollectibleEditor()
		// {
		// 	SceneView.onSceneGUIDelegate += OnScene;
		// }

		public override void OnInspectorGUI()
		{
			T obj = (T)target;

			if (isNew)
			{
				EditorGUI.FocusTextInControl("renameBox");
				isNew = false;
			}

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Remove " + typeof(T).Name))
			{
				EditorApplication.delayCall += () => {RemoveObject();};
			}

			if (GUILayout.Button("Duplicate"))
			{
				SOCollector<T> manager = (SOCollector<T>)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(obj));
				T copyObj = Instantiate(obj);
				
				manager.collection.Add(copyObj);
				AssetDatabase.AddObjectToAsset(copyObj, AssetDatabase.GetAssetPath(manager));
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
				Selection.SetActiveObjectWithContext(copyObj, null);
			}

			GUILayout.EndHorizontal();

			EditorGUI.BeginChangeCheck();

			//GUI.SetNextControlName("renameBox");
			obj.name = EditorGUILayout.DelayedTextField("Rename", obj.name);

			if (EditorGUI.EndChangeCheck())
			{
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}


			DrawDefaultInspector();
		}
		// static void OnScene( SceneView sceneView ) 
		// {
		// 	//GUIUtility.hotControl = 0;
		// 	if (Event.current != null && 
		// 		Event.current.isKey && 
		// 		Event.current.type.Equals(EventType.KeyDown) && 
		// 		Event.current.keyCode == KeyCode.L) {
				
		// 		//Delete code here
		// 		Debug.Log("TEst");
		// 		Event.current.Use();
		// 	}

     	// }

		void RemoveObject()
		{
			T obj = (T)target;
			SOCollector<T> manager = (SOCollector<T>)AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GetAssetPath(obj));
			SOCollectorEditor<T> me = (SOCollectorEditor<T>)Editor.CreateEditor(manager);
			me.RemoveObject(obj);
		}
	}

}