using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class LangUtils
	{
		public static int parseInt (string str)
		{
			if (str == null) {
				return 0;
			}

			int i = 0;
			int.TryParse (str, out i);
			return i;
		}

		public static float parseFloat (string str)
		{
			if (str == null) {
				return 0;
			}

			float f = 0;
			float.TryParse (str, out f);
			return f;
		}
	}
}
