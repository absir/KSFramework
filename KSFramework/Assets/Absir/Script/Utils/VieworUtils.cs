using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class VieworUtils
	{
		public static AB_Viewor getViewor (GameObject gameObject)
		{
			return ComponentUtils.fetchCurrentComponent<AB_Viewor> (gameObject);
		}

		public static AB_mNavViewor getNavViewor (GameObject gameObject)
		{
			return ComponentUtils.fetchCurrentComponent<AB_mNavViewor> (gameObject);
		}

		public static AB_mTabViewor getTabViewor (GameObject gameObject)
		{
			return ComponentUtils.fetchCurrentComponent<AB_mTabViewor> (gameObject);
		}
	}
}
