using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class Brige
	{
		private static IBrige _ME = new BrigeKSEngine ();

		public static IBrige ME {
			get {
				return _ME;
			}
		}

		public static void Load (string uri, bool sync, Action<Object> callback)
		{
			ME.Load (uri, sync, callback);
		}
	}

	public interface IBrige
	{
		void LogInfo (string message);

		void LogWarn (string message);

		void LogError (string message);

		string GetConfig (string section, string name);

		void Load (string uri, bool sync, Action<Object> callback);
	}
}

