using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KEngine;

namespace Absir
{
	public class BrigeKSEngine : IBrige
	{
		public void LogInfo (string message)
		{
			Log.Info (message);
		}

		public void LogWarn (string message)
		{
			Log.Warning (message);
		}

		public void LogError (string message)
		{
			Log.Error (message);
		}

		public void SetScreenScale (AB_Screen screen, Camera uiCamera, Vector2 scale)
		{
		}
	}
}

