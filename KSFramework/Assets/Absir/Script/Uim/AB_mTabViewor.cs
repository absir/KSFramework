using UnityEngine;
using System.Collections;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_mTabViewor : AB_mContainer
	{
		public AB_Bar tabBar;

		override protected void InitComponent ()
		{
			base.InitComponent ();
			if (tabBar == null) {
				tabBar = ComponentUtils.FetchChildrenComponent<AB_Bar> (gameObject);
			}

			if (tabBar != null && tabBar.canComponent == null && containerTrans != null) {
				tabBar.canComponent = ComponentUtils.GetComponentObject<CanComponent> (containerTrans.gameObject);
			}
		}

		public override bool DoAppear ()
		{
			if (base.DoAppear ()) {
				if (tabBar != null) {
					(tabBar.canComponent.GetActiveComponent () as AB_Viewor).DoAppear ();
				}
			
				return true;
			}
		
			return false;
		}

		public override bool DoDisappear ()
		{
			if (base.DoDisappear ()) {
				if (tabBar != null) {
					(tabBar.canComponent.GetActiveComponent () as AB_Viewor).DoDisappear ();
				}
			
				return true;
			}
		
			return false;
		}

		override public bool DoDisappearTransform ()
		{
			if (base.DoDisappearTransform ()) {
				if (tabBar != null) {
					CanComponent canComponent = tabBar.canComponent;
					if (canComponent != null) {
						int count = canComponent.GetComponentCount ();
						for (int i = 0; i < count; i++) {
							AB_Viewor viewor = (AB_Viewor)canComponent.GetComponentAt (i);
							GameObjectUtils.GetOrAddComponent<AB_Retain> (viewor.gameObject).Release ();
							viewor.DoDisappearTransform ();
						}
					}
				}

				return true;
			}

			return false;
		}
	}
}
