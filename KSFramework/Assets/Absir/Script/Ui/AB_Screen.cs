using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Absir
{
	[ExecuteInEditMode]
	public class AB_Screen : MonoBehaviour
	{
		public static AB_Screen ME {
			get {
				return _ME;
			}
		}

		private static AB_Screen _ME;

		public static GameObject getScreenObject ()
		{
			return _ME.gameObject;
		}

		public Camera uiCamera = null;
		public CanvasScaler canvasScaler;
		public Canvas canvas;

		public bool NoConfig;

		public Vector2 size;
		public CanvasScaler.ScreenMatchMode style;

		public Vector2 scaleOffset;

		void OnEnable ()
		{
			_ME = this;
		}

		void OnDisable ()
		{
			if (_ME == this) {
				_ME = null;
			}
		}

		// Use this for initialization
		public void Awake ()
		{
			if (uiCamera == null) {
				uiCamera = ComponentUtils.fetchParentComponent<Camera> (gameObject);
				if (uiCamera == null) {
					uiCamera = ComponentUtils.fetchAllChildrenComponent<Camera> (gameObject);
					if (uiCamera == null) {
						uiCamera = Camera.main;
						if (uiCamera == null) {
							uiCamera = GameObject.FindObjectOfType<Camera> ();
							if (uiCamera == null) {
								throw new UnityException ("AB_Screen could not found uiCamera");
							}
						}
					}
				}
			}

			if (canvasScaler == null) {
				canvasScaler = GameObjectUtils.getOrAddComponent<CanvasScaler> (gameObject);
			}

			canvas = canvasScaler.GetComponent<Canvas> ();

			_ME = this;
			if (!NoConfig || size.x == 0) {
				size = AB_Config.ME.ScreenSize;
				style = AB_Config.ME.ScreenStyle;

				uiCamera.farClipPlane = 20;
				Vector3 localPosition = uiCamera.transform.localPosition;
				localPosition.z = -10;
				uiCamera.transform.localPosition = localPosition;

				canvas.renderMode = RenderMode.ScreenSpaceCamera;
				canvas.worldCamera = uiCamera;
				canvas.planeDistance = 10;

				canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
				canvasScaler.referenceResolution = size;
				canvasScaler.screenMatchMode = style;
			}

			float localScaleX = canvasScaler.transform.localScale.x;
			if (localScaleX != 0 && localScaleX != 1) {
				uiCamera.orthographicSize /= localScaleX;
				uiCamera.fieldOfView = Mathf.Atan (Mathf.Tan (uiCamera.fieldOfView / 360.0f * Mathf.PI) / localScaleX) / Mathf.PI * 360.0f;
			}

			scaleOffset = calcScreenRationScaleOffset (size, style);
		}

		private static bool _calcRatio;
		private static Vector2 _calcSize;
		private static CanvasScaler.ScreenMatchMode _calcStyle;

		private static Vector2 _calcScaleOffset;

		public static float ScaleOffsetX;
		public static float ScaleOffsetY;

		protected Vector2 calcScreenRationScaleOffset (Vector2 size, CanvasScaler.ScreenMatchMode style)
		{
			if (size.x == Screen.width && size.y == Screen.height) {
				return Vector2.zero;
			}

			if (_calcRatio) {
				if (size == _calcSize && style == _calcStyle) {
					return _calcScaleOffset;
				}
			}

			_calcRatio = true;
			_calcSize = size;
			_calcStyle = style;

			GameObject _lt = new GameObject ();
			_lt.name = "_lt";
			_lt.transform.parent = _ME.transform;

			RectTransform _ltTransform = _lt.AddComponent<RectTransform> ();
			_ltTransform.anchorMin = Vector2.one;
			_ltTransform.anchorMax = Vector2.one;
			_ltTransform.anchoredPosition = Vector2.zero;

			Vector3 localPosition = _lt.transform.localPosition;
			//Debug.Log (TransformUtils.getVector3String(_lt.transform.localPosition));
			_calcScaleOffset = new Vector2 (localPosition.x - size.x / 2.0f, localPosition.y - size.y / 2.0f);

			Debug.Log ("_calcScaleOffset = " + TransformUtils.getVector2String (_calcScaleOffset));

			ScaleOffsetX = _calcScaleOffset.x;
			ScaleOffsetY = _calcScaleOffset.y;
			return _calcScaleOffset;
		}
	}
}
