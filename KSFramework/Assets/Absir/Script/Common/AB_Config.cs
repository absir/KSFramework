using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_Config
	{
		private static AB_Config _ME;

		public static AB_Config ME {
			get {
				if (_ME == null) {
					_ME = new AB_Config ();
				}

				return _ME;
			}
		}

		public const string SECTION = "AB_Config";

		public Vector2 ScreenSize = new Vector2 (640, 960);

		public CanvasScaler.ScreenMatchMode ScreenStyle = CanvasScaler.ScreenMatchMode.Expand;

		public string Version = "0.0.1";

		private AB_Config ()
		{
			int width = LangUtils.ParseInt (Brige.ME.GetConfig (SECTION, "width"));
			int height = LangUtils.ParseInt (Brige.ME.GetConfig (SECTION, "height"));
			if (width > 0) {
				ScreenSize.x = width;
			}

			if (height > 0) {
				ScreenSize.y = height;
			}

			int style = LangUtils.ParseInt (Brige.ME.GetConfig (SECTION, "style"));
			if (style > 0) {
				ScreenStyle = (CanvasScaler.ScreenMatchMode)(style - 1);
			}

			string version = Brige.ME.GetConfig (SECTION, "version");
			if (version != null) {
				Version = version;
			}

		}
	}
}

