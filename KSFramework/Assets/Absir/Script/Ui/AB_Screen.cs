using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[ExecuteInEditMode]
	public class AB_Screen : MonoBehaviour
	{
		public enum Style
		{
			KeepingRatio,
			BasedOnWidth,
			FillKeepingRatio,
			FillScreen,
			None,
		}

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

		public bool NoConfig;

		public Vector2 size;
		public Style style;
		public float ratioMax;
		public float ratioMin;

		public Vector2 scale;

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
		void Awake ()
		{
			if (uiCamera == null) {
				uiCamera = ComponentUtils.fetchParentComponent<Camera> (gameObject);
				if (uiCamera == null) {
					uiCamera = ComponentUtils.fetchAllChildrenComponent<Camera> (gameObject);
					if (uiCamera == null) {
						uiCamera = GameObject.FindObjectOfType<Camera> ();
						if (uiCamera == null) {
							throw new UnityException ("AB_Screen could not found uiCamera");
						}
					}
				}
			}

			_ME = this;
			if (!NoConfig || size.x == 0) {
				size = AB_Config.ScreenSize;
				style = AB_Config.ScreenStyle;
				ratioMax = AB_Config.ScreenRatioMax;
				ratioMin = AB_Config.ScreenRatioMin;
			}

			scale = calcScreenRationScale (size, style, ratioMax, ratioMin);
			Brige.ME.SetScreenScale (this, uiCamera, scale);
		}

		private static bool _calcRatio;

		private static Vector2 _calcSize;
		private static Style _calcStyle;
		private static float _calcRationMax;
		private static float _calcRatioMin;

		private static Vector2 _calcRatioScale;

		protected Vector2 calcScreenRationScale (Vector2 size, Style style, float ratioMax, float ratioMin)
		{
			if (_calcRatio) {
				if (size != _calcSize || style != _calcStyle) {
					if (style == Style.None) {
						return Vector2.one;
					}

					_calcRatio = false;

				} else if ((ratioMax == 0 || ratioMax >= _calcRationMax) && (ratioMin == 0 || ratioMin <= _calcRatioMin)) {
					return _calcRatioScale;
				}
			}

			float aspectRatio = Screen.height / Screen.width;
			if (_calcRatio) {
				if ((ratioMax == 0 || ratioMax >= aspectRatio) && (ratioMin == 0 || ratioMin <= aspectRatio)) {
					return _calcRatioScale;
				}
			}

			_calcRatio = true;
			_calcSize = size;
			_calcStyle = style;
			_calcRationMax = ratioMax;
			_calcRatioMin = ratioMin;

			float screenWith = Screen.width;				
			if (style != Style.BasedOnWidth) {
				if (ratioMax != 0 && ratioMax < aspectRatio) {
					// 太宽屏幕处理
					screenWith = Screen.height / ratioMax;

				} else if (ratioMin != 0 && ratioMin > aspectRatio) {
					// 太窄屏幕处理
					screenWith = Screen.height / ratioMin;

				} else {
					if (style == Style.FillKeepingRatio) {
						float sizeRatio = size.y / size.x;
						if (aspectRatio < sizeRatio) {
							screenWith = Screen.height / sizeRatio;
						}
					}  
				}
			} 

			float scaleX = screenWith / size.y;
			if (style == Style.FillScreen) {
				_calcRatioScale = new Vector2 (scaleX, Screen.height / size.y);
			
			} else {
				_calcRatioScale = new Vector2 (scaleX, scaleX);
			}

			return _calcRatioScale;
		}
	}
}
