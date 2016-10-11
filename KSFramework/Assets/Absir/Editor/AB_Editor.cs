using System.IO;
using KEngine.UI;
using KUnityEditorTools;
using UnityEditor;

#if UNITY_5
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using KEngine;
using KEngine.Editor;

namespace Absir
{
	[InitializeOnLoad]
	public class AB_Edtior
	{
		protected static AB_Screen GetScreen ()
		{
			AB_Screen screen = GameObject.FindObjectOfType<AB_Screen> ();
			if (screen == null) {
				screen = new GameObject ().AddComponent<AB_Screen> ();
				screen.gameObject.name = "AB_Screen";
			}

			screen.Awake ();
			return screen;
		}

		[MenuItem ("AB_Edtior/UI(UGUI)/Create UI(UGUI)")]
		public static void CreateNewUI ()
		{
			KUGUIBuilder.CreateNewUI ();
			UIWindowAsset[] assets = GameObject.FindObjectsOfType<UIWindowAsset> ();
			AB_Screen screen = null;
			foreach (var asset in assets) {
				if (asset.transform.parent == null) {
					if (screen == null) {
						screen = GetScreen ();
					}

					Transform assetTransform = asset.transform;
					//Vector3 localScale = assetTransform.localScale;

					string assetName = asset.name;
					GameObject newAsset = new GameObject ();
					newAsset.AddComponent<UIWindowAsset> ();
					Transform newAssetTransform = newAsset.transform;
					newAssetTransform.parent = screen.transform;
					newAssetTransform.localPosition = Vector3.zero;
					newAssetTransform.localScale = Vector3.one;

					foreach(Transform child in asset.transform) {
						child.transform.parent = newAssetTransform;
					}
						
					GameObject.DestroyImmediate (asset.gameObject);
					newAsset.name = assetName;
				}
			}
		}

	}
}

