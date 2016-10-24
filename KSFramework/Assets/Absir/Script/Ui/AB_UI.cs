using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_UI
	{
		public const float BEHIND_STEP = 0.000001f;

		public const float BEHIND_STEP_D = BEHIND_STEP * 100.0f;

		private static AB_UI _ME;

		public static AB_UI ME {
			get {
				if (_ME == null) {
					_ME = new AB_UI ();
				}

				return _ME;
			}
		}

		private GameObject _unactiveGameObject;

		private Dictionary<string, AB_Retain> _nameDictRetain = new Dictionary<string, AB_Retain> ();

		private Stack<GameObject> _dialogObjectStack;

		private AB_UI ()
		{
			_unactiveGameObject = GameObject.Find ("_UnactiveGameObject");
			if (_unactiveGameObject == null) {
				_unactiveGameObject = new GameObject ("_UnactiveGameObject");
			}

			GameObject.DontDestroyOnLoad (_unactiveGameObject);
			_unactiveGameObject.SetActive (false);
			Transform _unactiveGameObjectTransform = _unactiveGameObject.transform;
			_unactiveGameObjectTransform.localPosition = new Vector3 (Screen.width * 128, Screen.height * 128, 0);
			_unactiveGameObjectTransform.localScale = AB_Screen.ME.transform.lossyScale;

			foreach (var retain in GameObject.FindObjectsOfType<AB_Retain> ()) {
				if (retain.name.StartsWith ("_")) {
					GameObject.DontDestroyOnLoad (retain.gameObject);
					retain.retain ();
					_nameDictRetain.Add (retain.name, retain);
					addView (retain.transform, _unactiveGameObject.transform);
				}
			}
				
			_dialogObjectStack = new Stack<GameObject> ();
		}

		public GameObject getRetain (string name)
		{
			AB_Retain retain = null;
			_nameDictRetain.TryGetValue (name, out retain);
			if (retain == null) {
				return null;
			}

			GameObject gameObject = retain.gameObject;
			if (gameObject) {
				return gameObject;
			}

			retain.release ();
			_nameDictRetain.Remove (name);
			return null;
		}

		public void putRetain (string name, AB_Retain retain)
		{
			retain.retain ();
			removeRetain (name);
			_nameDictRetain.Add (name, retain);
			addView (retain.transform, _unactiveGameObject.transform);
		}

		public void removeRetain (string name)
		{
			AB_Retain retain = null;
			_nameDictRetain.TryGetValue (name, out retain);
			if (retain != null) {
				_nameDictRetain.Remove (name);
				retain.releaseCleanUp ();
			}
		}

		public GameObject getUnactiveGameObject ()
		{
			return _unactiveGameObject;
		}

		public void setViewActive (Transform transform, bool status)
		{
			transform.gameObject.SetActive (status);
		}

		public void addView (Transform viewTrans, Transform containerTrans)
		{
			Vector3 localScale = viewTrans.localScale;
			Vector3 localPosition = viewTrans.localPosition;
			viewTrans.parent = containerTrans;
			viewTrans.localScale = localScale;
			viewTrans.localPosition = localPosition;
		}

		public void addViewAuto (Transform viewTrans, Transform containerTrans, bool autoHeight)
		{
			addView (viewTrans, containerTrans);
		}

		public bool removeView (Transform viewTrans)
		{
			AB_Retain retain = viewTrans.gameObject.GetComponent<AB_Retain> ();
			if (retain == null || retain.retainCount <= 0) {
				GameObject.Destroy (viewTrans.gameObject);
				return true;

			} else {
				unActiveView (viewTrans);
			}

			return false;
		}

		public void unActiveView (Transform viewTrans)
		{
			addView (viewTrans, _unactiveGameObject.transform);
		}

		private GameObject _dialogBackGround = null;

		public void closeDialogBackGround ()
		{
			if (_dialogBackGround != null && _dialogBackGround) {
				removeView (_dialogBackGround.transform);
			}

			_dialogBackGround = null;
		}

		public void showDialogBackGround (GameObject dialogBackGround)
		{
			closeDialogBackGround ();
			if (dialogBackGround != null) {
				_dialogBackGround = dialogBackGround;
				addView (_dialogBackGround.transform, AB_Screen.ME.getContainer ());
			}
		}

		public void showDialogBackGroundName (string name)
		{
			showDialogBackGround (getRetain (name));
		}

		public void openDialog (GameObject gameObject)
		{
			openDialogName (gameObject, null);
		}

		public void openDialogName (GameObject gameObject, string name)
		{
			if (name == null) {
				name = "_dialogBackGround";
			}
			
			openDialogWithBackGround (gameObject, _dialogObjectStack.Count == 0 && name.Length != 0 ? getRetain (name) : null);
		}

		public void openDialogWithBackGround (GameObject gameObject, GameObject dialogBackGround)
		{
			Vector3 localePosition = gameObject.transform.localPosition;
			if (_dialogObjectStack.Count == 0) {
				float minZ = 0;
				foreach (Transform trans in AB_Screen.ME.getContainer ()) {
					float z = trans.localPosition.z;
					if (minZ > z) {
						minZ = z;
					}
				}

				localePosition.z = minZ - BEHIND_STEP_D;
				showDialogBackGround (dialogBackGround);

			} else {
				GameObject dialogObject = _dialogObjectStack.Peek ();
				localePosition.z = dialogObject.transform.localPosition.z - BEHIND_STEP_D;
			}

			if (_dialogBackGround != null) {
				Vector3 dialogPosition = _dialogBackGround.transform.localPosition;
				dialogPosition.z = localePosition.z;
				_dialogBackGround.transform.localPosition = dialogPosition;
			}

			localePosition.z -= BEHIND_STEP_D;
			gameObject.transform.localPosition = localePosition;

			_dialogObjectStack.Push (gameObject);
			addViewAuto (gameObject.transform, AB_Screen.ME.getContainer (), false);
		}

		public GameObject currentDialog ()
		{
			return _dialogObjectStack.Peek ();
		}

		public void closeDialog ()
		{
			if (_dialogObjectStack.Count > 0) {
				GameObject dialogObject = _dialogObjectStack.Pop ();
				if (dialogObject != null) {
					removeView (dialogObject.transform);
					if (_dialogObjectStack.Count == 0) {
						closeDialogBackGround ();
					}
				}
			}
		}

		public void closeDialogCount (int cnt)
		{
			int count = _dialogObjectStack.Count;
			if (count == 0) {
				return;
			}

			GameObject dialog;
			while (count-- > 0 && cnt-- > 0) {
				dialog = _dialogObjectStack.Pop ();
				removeView (dialog.transform);
			}

			if (_dialogObjectStack.Count == 0) {
				closeDialogBackGround ();
			}
		}

		public void closeDialogAll ()
		{
			int count = _dialogObjectStack.Count;
			if (count == 0) {
				return;
			}

			GameObject dialog;
			while (count-- > 0) {
				dialog = _dialogObjectStack.Pop ();
				removeView (dialog.transform);
			}

			closeDialogBackGround ();
		}
	}
}

