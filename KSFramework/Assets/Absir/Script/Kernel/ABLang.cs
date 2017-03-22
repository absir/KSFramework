using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ABLang
{

	public class Map<K, V> : Dictionary<K, V>
	{
		
		public void Add (K key, V value)
		{
			SaveAdd (key, value);
		}

		public void SaveAdd (K key, V value)
		{
			base.Remove (key);
			base.Add (key, value);
		}
	}

}
