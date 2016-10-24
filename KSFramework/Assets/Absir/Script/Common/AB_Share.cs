using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public class AB_Share : MonoBehaviour
	{
		private string _name;

		private static Dictionary<string, AB_Share> nameDictShare = new Dictionary<string, AB_Share> ();

		protected static bool AddShare (AB_Share share)
		{
			string name = share.name;
			if (!nameDictShare.ContainsKey (name)) {
				share._name = name;
				nameDictShare.Add (name, share);
				return true;
			}

			return false;
		}

		protected static bool RemoveShare (AB_Share share)
		{
			string name = share._name;
			if (nameDictShare.ContainsKey (name)) {
				nameDictShare.Remove (name);
				return true;
			}

			return false;
		}

		public AB_Share GetShare (string name)
		{
			AB_Share share = null;
			nameDictShare.TryGetValue (name, out share);
			return share;
		}

		void Awake ()
		{
			AddShare (this);
		}

		void OnDestory ()
		{
			RemoveShare (this);
		}
	}
}
