using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KEngine;
using KEngine.UI;
using KSFramework;

namespace Absir
{

	[System.Obsolete ("Use SceneManager to determine what scenes have been loaded")]
	public class AB_Load : MonoBehaviour
	{
		public string loadUrl;

		public string sceneUrl;

		public bool unloadWaite;

		private Action unloadAction;

		void Awake ()
		{
			AB_Init.Init (false);
			Load ();
		}

		protected void Load ()
		{
			if (string.IsNullOrEmpty (sceneUrl)) {
				return;
			}

			if (!string.IsNullOrEmpty (loadUrl)) {
				bool contain = KResourceModule.ContainsResourceUrl (loadUrl);
				Debug.Log ("AB_Load loadUrl " + loadUrl + " contain " + contain);
				if (contain) {
					new AB_AssetLoader (loadUrl, (ok, result) => {
						LoadScene ();

					}, LoaderMode.Sync);

					return;
				}
			} 

			LoadScene ();
		}

		protected void LoadScene ()
		{
			string loadedName = Application.loadedLevelName;
			Debug.Log ("AB_Load load at : " + loadedName + " to " + sceneUrl);
			unloadAction = () => {
				Debug.Log ("AB_Load unload " + loadedName);
				Application.UnloadLevel (loadedName);
			};

			new AB_SceneLoader (sceneUrl, (ok, result) => {
				if (!unloadWaite) {
					UnloadAction ();
				}

			}, LoaderMode.Async);
		}

		public void UnloadWaite ()
		{
			unloadWaite = true;
		}

		public void UnloadAction ()
		{
			if (unloadAction != null) {
				StartCoroutine (UnloadActionDelay (unloadAction));
				unloadAction = null;
			}
		}

		protected IEnumerator UnloadActionDelay (Action unloadAction)
		{
			yield return 0;
			unloadAction ();
		}
	}

}