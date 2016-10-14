using UnityEngine;
using System.Collections;
using KEngine;
using KEngine.UI;

public class AB_Load : MonoBehaviour {

	public string url;

	// Use this for initialization
	void Awake () {
		Load ();
	}

	protected void Load() {
		SceneLoader loader = SceneLoader.Load (url);
	}
}
