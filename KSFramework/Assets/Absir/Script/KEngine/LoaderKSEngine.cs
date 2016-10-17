using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KEngine;

namespace Absir
{
	public class AB_AssetLoader : AssetBundleLoader
	{
		public static AB_AssetLoader load (string url, LoaderDelgate callback)
		{
			return new AB_AssetLoader (url, callback, LoaderMode.Sync);
		}

		public AB_AssetLoader (string url, LoaderDelgate callback, LoaderMode mode)
		{
			if (string.IsNullOrEmpty (url)) {
				Log.Error ("[AB_SceneLoad:New]url为空");
			}

			IsForceNew = true;
			Init (url, mode);
			AddCallback (callback);
		}

		protected override void OnFinish (object resultObj)
		{
			base.OnFinish (resultObj);
			AssetFileLoader loader = resultObj as AssetFileLoader;
			if (loader != null) {
				loader.Release ();
			}
		}
	}

	public class AB_SceneLoader : SceneLoader
	{

		public AB_SceneLoader (string url, LoaderDelgate callback, LoaderMode mode)
		{
			if (string.IsNullOrEmpty (url)) {
				Log.Error ("[AB_SceneLoad:New]url为空");
			}

			IsForceNew = true;
			Init (url, mode);
			AddCallback (callback);
		}

		protected override void OnFinish (object resultObj)
		{
			base.OnFinish (resultObj);
			AssetFileLoader loader = resultObj as AssetFileLoader;
			if (loader != null) {
				loader.Release ();
			}
		}
	}
}

