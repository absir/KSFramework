using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KEngine;
using KEngine.UI;
using KSFramework;
using UnityEngine.SceneManagement;

namespace Absir
{
	//[System.Obsolete ("Use SceneManager to determine what scenes have been loaded")]
	public class AB_Load : MonoBehaviour
	{
		public string loadUrl = "prefab/splash.prefab.k";

		public string sceneUrl = "Scene/Main.unity";

		public float unloadDelay = 3.0f;

		private bool unloadWaite;

		private Action unloadAction;

		void Awake ()
		{
			AB_Init.Init (false);
			AB_Game.AddGameStartAction (Load);
		}

		protected void Load ()
		{
			if (!string.IsNullOrEmpty (loadUrl)) {
				bool contain = KResourceModule.ContainsResourceUrl (loadUrl);
				Debug.Log ("AB_Load loadUrl " + loadUrl + " contain " + contain);
				if (contain) {
					new AB_AssetLoader (loadUrl, (ok, result) => {
						Debug.Log ("AB_Load load result " + ok + " : " + result);
						if (!ok || result == null) {
							unloadDelay = 0;

						} else {
							Instantiate (result);
							if (unloadDelay > 0) {
								StartCoroutine (UnloadDelay ());
							}
						}

						LoadScene ();

					}, LoaderMode.Sync);

					return;
				}
			} 

			unloadDelay = 0;
			LoadScene ();
		}

		protected void LoadScene ()
		{
			if (string.IsNullOrEmpty (sceneUrl)) {
				return;
			}

			//string loadedName = Application.loadedLevelName;
			Scene activeScene = SceneManager.GetActiveScene ();
			string loadedName = activeScene.name;
			//string loadedName = Application.loadedLevelName;
			Debug.Log ("AB_Load load at : " + loadedName + " to " + sceneUrl);
			AB_Game.LoginWaite ("AB_Load");
			unloadAction = () => {
				Debug.Log ("AB_Load unload " + loadedName);
				SceneManager.UnloadScene (activeScene);
				//Application.UnloadLevel (loadedName);
			};

			int sceneCount = SceneManager.sceneCount;
			new AB_SceneLoader (sceneUrl, (ok, result) => {
				if (result != null) {
					SceneManager.SetActiveScene (SceneManager.GetSceneAt (sceneCount));
				}

				AB_Game.LogicComplete ("AB_Load");
				if (!unloadWaite) {
					UnloadAction ();
				}

			}, LoaderMode.Async);
		}

		public void UnloadWaite ()
		{
			Log.Info ("UnloadWaite");
			unloadWaite = true;
		}

		public void UnloadAction ()
		{
			if (unloadAction != null) {
				StartCoroutine (UnloadActionDelay (unloadAction));
				unloadAction = null;
			}
		}

		protected IEnumerator UnloadDelay ()
		{
			if (unloadDelay > 0) {
				yield return new WaitForSeconds (unloadDelay);
				//Debug.Log ("UnloadDelay Done");
				unloadDelay = 0;
			}
		}

		protected IEnumerator UnloadActionDelay (Action unloadAction)
		{
			yield return 0;
			//Debug.Log ("UnloadActionDelay " + unloadDelay);
			while (unloadDelay > 0 || !AB_Game.IsonLogicStarted ()) {
				yield return 0;
			}

			//Debug.Log ("UnloadActionDelay Done");
			unloadAction ();
		}
	}

}