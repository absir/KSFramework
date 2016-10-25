using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KEngine;

namespace Absir
{
	public class BrigeKSEngine : IBrige
	{
		public void LogInfo (string message)
		{
			Log.Info (message);
		}

		public void LogWarn (string message)
		{
			Log.Warning (message);
		}

		public void LogError (string message)
		{
			Log.Error (message);
		}

		public string GetConfig (string section, string name)
		{
			return AppEngine.GetConfig (section, name, false);
		}

		public void Load (string path, bool sync, bool multi, Action<Object> callback)
		{
			if (callback == null) {
				return;
			}

			if (multi) {
				AB_AssetLoader.AutoLoad (path, (ok, obj) => {
					callback (obj);
				}, sync ? LoaderMode.Sync : LoaderMode.Async);

			} else {
				new AB_AssetLoader (path, (ok, obj) => {
					callback (obj);
				}, sync ? LoaderMode.Sync : LoaderMode.Async);
			}
		}
	}
}

