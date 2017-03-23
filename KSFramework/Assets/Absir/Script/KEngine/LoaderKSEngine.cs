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
		public static void AutoRelease (AbstractResourceLoader loader)
		{
			loader.AddCallback ((ok, obj) => {
				AB_Context.ME.AddAction (() => {
					loader.Release ();
				});
			});
		}

		public static AssetFileLoader AutoLoad (string path, AssetFileBridgeDelegate assetFileLoadedCallback, LoaderMode loaderMode)
		{
			AssetFileLoader loader = AssetFileLoader.Load (path, assetFileLoadedCallback, loaderMode);
			AutoRelease (loader);
			return loader;
		}

		public static Object Load (string url)
		{
			AssetFileLoader loader = AssetFileLoader.Load (url, null, LoaderMode.Sync);
			AutoRelease (loader);
			return loader.IsCompleted ? loader.Asset : null;
		}

		public static GameObject Instantiate (string url)
		{
			Object asset = Load (url);
			return asset == null ? null : GameObject.Instantiate (asset) as GameObject;
		}

		public static AB_AssetLoader Open (string url, AssetFileBridgeDelegate callback)
		{
			return new AB_AssetLoader (url, callback, LoaderMode.Sync);
		}

		public AB_AssetLoader (string url, AssetFileBridgeDelegate callback, LoaderMode mode)
		{
			if (string.IsNullOrEmpty (url)) {
				Log.Error ("[AB_SceneLoad:New]url为空");
			}
				
			LoaderDelgate newCallback = null;
			if (callback != null) {
				newCallback = (isOk, obj) => callback (isOk, obj as Object);
				AddCallback (newCallback);
			}
				
			//url = url + AppEngine.GetConfig (KEngineDefaultConfigs.AssetBundleExt);
			Init (url, mode);
		}

		protected override void OnFinish (object resultObj)
		{
			base.OnFinish (resultObj);
			AB_Context.ME.AddAction (DoDispose);
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
				
			AddCallback (callback);
			Init (url, mode);
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

