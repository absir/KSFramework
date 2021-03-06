using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	[ExecuteInEditMode]
	public class AB_Screen : MonoBehaviour
	{
		public static AB_Screen ME {
			get {
				if (_ME == null) {
					GameObject screen = new GameObject ();
					screen.name = "AB_Screen";
					_ME = screen.AddComponent<AB_Screen> ();
				}

				return _ME;
			}
		}

		private static AB_Screen _ME;

		public Camera uiCamera = null;
		public CanvasScaler canvasScaler;
		public Canvas canvas;

		public bool NoConfig;

		public Vector2 size;
		public CanvasScaler.ScreenMatchMode style;

		public RectTransform _container;

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

		public RectTransform getContainer ()
		{
			return _container;
		}

		// Use this for initialization
		public void Awake ()
		{
			CalcScreen ();
		}

		public void CalcScreen ()
		{
			if (uiCamera == null) {
				uiCamera = ComponentUtils.FetchParentComponent<Camera> (gameObject);
				if (uiCamera == null) {
					uiCamera = ComponentUtils.FetchAllChildrenComponent<Camera> (gameObject);
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
				canvasScaler = GameObjectUtils.GetOrAddComponent<CanvasScaler> (gameObject);
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

			RectTransform container = _container;
			if (container == null) {
				container = InitContainer ();
				_container = container;
			}

			CalcContainer (container);

			scaleOffset = CalcScreenRationScaleOffset (size, style);
			Debug.Log ("scaleOffset = " + TransformUtils.GetVector2String (scaleOffset));
		}

		protected RectTransform InitContainer ()
		{
			GameObject container = new GameObject ();
			container.name = "container";
			RectTransform trans = container.AddComponent<RectTransform> ();
			trans.parent = transform;
			trans.localPosition = Vector3.zero;
			trans.localScale = Vector3.one;
			trans.sizeDelta = Vector2.zero;
			trans.anchorMin = Vector2.zero;
			trans.anchorMax = Vector2.one;
			return trans;
		}

		protected void CalcContainer (RectTransform container)
		{
			
		}

		private static bool _calcRatio;
		private static Vector2 _calcSize;
		private static CanvasScaler.ScreenMatchMode _calcStyle;

		private static Vector2 _calcScaleOffset;

		public static float ScaleOffsetX;
		public static float ScaleOffsetY;

		protected Vector2 CalcScreenRationScaleOffset (Vector2 size, CanvasScaler.ScreenMatchMode style)
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

			ScaleOffsetX = _calcScaleOffset.x;
			ScaleOffsetY = _calcScaleOffset.y;

			DestroyImmediate (_lt);
			return _calcScaleOffset;
		}
	}
}
