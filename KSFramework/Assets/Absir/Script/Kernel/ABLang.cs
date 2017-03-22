using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ABLang
{

	public static class Map<K, V> : Dictionary<K, V>
	{

		public override void Add (K key, V value)
		{
			base.Remove (key);
			base.Add (T, K);
		}
	}



}
