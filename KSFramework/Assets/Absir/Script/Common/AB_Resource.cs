using UnityEngine;
using System.Collections;

namespace Absir
{

	public class AB_Resource
	{
		public static T LoadModule<T> (string name) where T : Component
		{
			Object obj = Resources.Load ("Module/" + name);
			if (obj != null) {
				GameObject gameObject = GameObject.Instantiate (obj) as GameObject;
				if (gameObject != null) {
					T t = gameObject.GetComponent<T> ();
					if (t == null) {
						GameObject.Destroy (t);
					}

					return t;
				}
			}

			return null;
		}
	}
}
