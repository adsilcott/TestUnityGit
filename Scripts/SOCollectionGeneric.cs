using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StellarDoor.SOCollection
{
	[Serializable]
	public class SOCollectionGeneric<T> : SOCollectionBase where T : ScriptableObject
	{
		public List<T> collection = new List<T>();

		public static implicit operator List<T>(SOCollectionGeneric<T> f)
		{
			return f.collection;
		}
	}

}
