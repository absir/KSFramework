using UnityEngine;
using System.Collections;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_mNavTabBar : AB_Bar
	{
		override protected void setTabComponentIndex ()
		{
			if (canComponent != null) {
				Component component = canComponent.getActiveComponent ();
				if (canComponent.setActiveComponentIndex (activeComponentIndex)) {
					if (component != null) {
						AB_mNavViewor navigation = ComponentUtils.fetchAllChildrenComponent<AB_mNavViewor> (component.gameObject);
						if (navigation != null) {
							navigation.popVieworRoot ();
						}
					}
				}
			}
		}
	}
}
