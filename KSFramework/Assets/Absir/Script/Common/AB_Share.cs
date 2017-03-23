using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public class AB_Share : AB_Retain
	{
		private static Dictionary<string, AB_Share> nameDictShare = new Dictionary<string, AB_Share> ();

		private string _name;

		public AB_Call call;

		public bool _noUnactive;

		public static AB_Share GetShare (string name)
		{
			AB_Share share = null;
			nameDictShare.TryGetValue (name, out share);
			return share;
		}

		public static bool AddShare (AB_Share share, string name = null)
		{
			if (string.IsNullOrEmpty (name)) {
				name = share._name;
				if (string.IsNullOrEmpty (name)) {
					name = share.name;
				}

			} else {
				RemoveShare (share);
			}

			if (!nameDictShare.ContainsKey (name)) {
				nameDictShare.Add (name, share);
				share._name = name;
				return true;
			}

			return false;
		}

		public static bool RemoveShare (AB_Share share)
		{
			string _name = share._name;
			if (!string.IsNullOrEmpty (_name)) {
				share._name = null;
				AB_Share value = null;
				nameDictShare.TryGetValue (_name, out value);
				if (value == share && nameDictShare.Remove (_name)) {
					return true;
				}
			}

			return false;
		}

		void Awake ()
		{
			if (call == null) {
				call = AB_Call.Find (gameObject);
			}
			
			if (AddShare (this)) {
				if (!_noUnactive) {
					GameObjectUtils.GetOrAddComponent<AB_Retain> (gameObject).Retain ();
					AB_UI.ME.UnActiveView (transform);
				}
			}
		}

		void OnDestory ()
		{
			RemoveShare (this);
		}

		public const string _SetShareTag = "SetShareTag";

		public void SetShareTag (string shareTag)
		{
			AB_Call.DoCall (call, _SetShareTag, tag);
		}

	}
}
