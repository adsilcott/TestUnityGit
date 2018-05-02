using System;
using System.Collections.Generic;
using UnityEngine;

namespace StellarDoor.SOCollection
{
	public abstract class SOCollector<T> : ScriptableObject where T : ScriptableObject
	{
		public List<T> collection = new List<T>();

		[NonSerialized] private List<T> _runtimeCollection;
		protected List<T> runtimeCollection
		{
			get 
			{
				if( _runtimeCollection == null || _runtimeCollection.Count == 0)
					_runtimeCollection = new List<T> (collection);
				return _runtimeCollection;
			}
			set
			{
				_runtimeCollection = value;
			}
		}

		public T GetCopy(string name)
		{
			T so = runtimeCollection.Find(F => F.name == name);
			
			if (so == null)
			{
				Debug.Log("Can't instantiate " + name + ", Object not found.");
				return null;
			}
			
			return Instantiate(so);
		}

		public T GetObject(string name)
		{
			T so = runtimeCollection.Find(F => F.name == name);
			// if (so == null)
			// 	Debug.Log(name + " Object not found.");
			
			return so;
		}
		
		public void AddObject(T newSO)
		{
			T so = runtimeCollection.Find(F => F.name == newSO.name);

			if (so == null)
				runtimeCollection.Add(newSO);
			else
				so = newSO;
		}

	}

}
