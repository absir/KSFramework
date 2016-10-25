using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_Uri
	{
		private static Stack<AB_Uri> uriStack = new Stack<AB_Uri> ();

		public static AB_Uri current { get; protected set; }

		public static void Load (string uri, bool sync, long lId, string sId, Action<Object> callback)
		{
			if (current != null) {
				uriStack.Push (current);
			}

			string path = "ui/" + uri;
			current = new AB_Uri (uri, sync, lId, sId);
			Brige.Load (path, sync, false, callback);
		}

		public static void GoBack (int number, Action<Object> callback)
		{
			int count = uriStack.Count;
			if (count <= 0) {
				return;
			}

			if (number <= 0) {
				number = count + number;
			}

			if (number > count) {
				number = count;
			}

			while (number > 1) {
				uriStack.Pop ();
			}

			current = uriStack.Pop ();
			Brige.Load (current.path, current.sync, false, callback);
		}

		public static void ClearUri ()
		{
			current = null;
			uriStack.Clear ();
		}

		public string path{ get; protected set; }

		public bool sync { get; protected set; }

		public long lId { get; protected set; }

		public string sId { get; protected set; }

		private Dictionary<string, object> paramDict;

		public AB_Uri (string path, bool sync, long lId, string sId)
		{
			this.path = path;
			this.sync = sync;
			this.lId = lId;
			this.sId = sId;
		}

		public object GetParam (string name)
		{
			if (paramDict == null || name == null) {
				return null;
			}

			object value = null;
			paramDict.TryGetValue (name, out value);
			return value;
		}

		public void SetParam (string name, object value)
		{
			if (name == null) {
				return;
			}

			if (paramDict == null) {
				if (value == null) {
					return;
				}

				paramDict = new Dictionary<string, object> ();

			} else {
				paramDict.Remove (name);
			}

			if (value != null) {
				paramDict.Add (name, value);
			}
		}
	}
}
