using UnityEngine;
using System.Collections;
using KEngine;
using KEngine.UI;

[System.Obsolete ("Use SceneManager to determine what scenes have been loaded")]
public class AB_Load : MonoBehaviour {

	public string url;

	// Use this for initialization
	void Awake () {
		Load ();
	}

	protected void Load() {
		string loadedName = Application.loadedLevelName;
		Debug.Log ("AB_Load loadedName : " + loadedName + " => " + url);
		SceneLoader.Load (url, (success)=> {
			if(success) {
				Application.UnloadLevel (loadedName);
			}
		}, LoaderMode.Async);
	}
}
