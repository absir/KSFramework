using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public class VieworUtils
	{
		public static AB_mNavViewor getNavViewor (GameObject gameObject)
		{
			return AB_Viewor.GetCurrentVieworOrder<AB_mNavViewor> (0);
		}

		public static AB_mTabViewor getTabViewor (GameObject gameObject)
		{
			return AB_Viewor.GetCurrentVieworOrder<AB_mTabViewor> (0);
		}
	}
}
