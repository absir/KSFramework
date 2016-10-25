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

		protected static IEnumerator autoRelease (AbstractResourceLoader[] autoLoaders)
		{
			yield return 0;
			if (autoLoaders [0] == null) {
				yield return 0;
			}

			autoLoaders [0].Release ();
		}

		public static AssetFileLoader autoLoad (string path, AssetFileBridgeDelegate assetFileLoadedCallback, LoaderMode loaderMode)
		{
			// 添加扩展名
			path = path + AppEngine.GetConfig (KEngineDefaultConfigs.AssetBundleExt);
			AbstractResourceLoader[] autoLoaders = new AssetFileLoader[1];
			LoaderDelgate realcallback = (isOk, obj) => {
				if (assetFileLoadedCallback != null) {
					assetFileLoadedCallback (isOk, obj as Object);
				}

				AB_Context.ME.StartCoroutine (autoRelease (autoLoaders));
			};

			AssetFileLoader loader = AutoNew<AssetFileLoader> (path, realcallback, false, loaderMode);
			autoLoaders [0] = loader;
			return loader;
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

