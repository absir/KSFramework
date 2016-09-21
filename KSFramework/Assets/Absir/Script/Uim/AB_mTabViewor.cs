using UnityEngine;
using System.Collections;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	public class AB_mTabViewor : AB_mContainer
	{
		public AB_Bar tabBar;

		override protected void initComponent ()
		{
			base.initComponent ();
			if (tabBar == null) {
				tabBar = ComponentUtils.fetchChildrenComponent<AB_Bar> (gameObject);
			}

			if (tabBar != null && tabBar.canComponent == null && containerTrans != null) {
				tabBar.canComponent = ComponentUtils.getComponentObject<CanComponent> (containerTrans.gameObject);
			}
		}

		public override bool doAppear ()
		{
			if (base.doAppear ()) {
				if (tabBar != null) {
					(tabBar.canComponent.getActiveComponent () as AB_Viewor).doAppear ();
				}
			
				return true;
			}
		
			return false;
		}

		public override bool doDisappear ()
		{
			if (base.doDisappear ()) {
				if (tabBar != null) {
					(tabBar.canComponent.getActiveComponent () as AB_Viewor).doDisappear ();
				}
			
				return true;
			}
		
			return false;
		}
	}
}
