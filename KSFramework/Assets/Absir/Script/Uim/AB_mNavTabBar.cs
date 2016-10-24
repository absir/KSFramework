using UnityEngine;
using System.Collections;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_mNavTabBar : AB_Bar
	{
		override protected void SetTabComponentIndex ()
		{
			if (canComponent != null) {
				Component component = canComponent.GetActiveComponent ();
				if (canComponent.SetActiveComponentIndex (activeComponentIndex)) {
					if (component != null) {
						AB_mNavViewor navigation = ComponentUtils.FetchAllChildrenComponent<AB_mNavViewor> (component.gameObject);
						if (navigation != null) {
							navigation.PopVieworRoot ();
						}
					}
				}
			}
		}
	}
}
