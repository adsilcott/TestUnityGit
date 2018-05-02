using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StellarDoor.SOCollection
{
	[Serializable]
	public class SOCollection
	{
		public List<ScriptableObject> collection = new List<ScriptableObject>();

		public static implicit operator List<ScriptableObject>(SOCollection f)
		{
			return f.collection;
		}

		bool Check(ScriptableObject obj)
		{
			Debug.Log("Test");
			return collection.Contains(obj);
		}
	}

}
