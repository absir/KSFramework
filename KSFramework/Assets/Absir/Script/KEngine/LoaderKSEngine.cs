using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KEngine;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_AssetLoader : AssetFileLoader
	{
		public static AB_AssetLoader load (string url, AssetFileBridgeDelegate callback)
		{
			return new AB_AssetLoader (url, callback, LoaderMode.Sync);
		}

		public AB_AssetLoader (string url, AssetFileBridgeDelegate callback, LoaderMode mode)
		{
			if (string.IsNullOrEmpty (url)) {
				Log.Error ("[AB_SceneLoad:New]url为空");
			}

			Init (url, mode);
			LoaderDelgate newCallback = null;
			if (callback != null) {
				newCallback = (isOk, obj) => callback (isOk, obj as Object);
				AddCallback (newCallback);
			}
		}

		protected override void OnFinish (object resultObj)
		{
			base.OnFinish (resultObj);
			OnReadyDisposed ();
		}
	}

	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_SceneLoader : SceneLoader
	{
		public AB_SceneLoader (string url, LoaderDelgate callback, LoaderMode mode)
		{
			if (string.IsNullOrEmpty (url)) {
				Log.Error ("[AB_SceneLoad:New]url为空");
			}
				
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

