﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public class Brige
	{
		private static IBrige _ME = new BrigeKSEngine ();

		public static IBrige ME {
			get {
				return _ME;
			}
		}
	}

	public interface IBrige
	{
		void LogInfo (string message);

		void LogWarn (string message);

		void LogError (string message);

		void SetScreenScale (AB_Screen screen, Camera uiCamera, Vector2 scale);
	}
}

