using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	public class AB_Seg : AB_Cats<BoxCollider>
	{
		protected int activeComponentIndex;

		public int getActiveComponentIndex ()
		{
			return activeComponentIndex;
		}

		override protected void initComponent ()
		{
			base.initComponent ();
			if (activeComponent != null) {
				activeComponentIndex = componentSort.IndexOf (activeComponent);
			}
		}

		override public bool setActiveComponentIndex (int componentIndex)
		{
			if (base.setActiveComponentIndex (componentIndex)) {
				activeComponentIndex = componentIndex;
				return true;
			}
		
			return false;
		}

		override protected bool isComponentActive (BoxCollider component)
		{
			return !component.enabled;
		}

		override protected void setComponentActive (BoxCollider component, bool active)
		{
			if (active) {
				component.gameObject.SendMessage ("OnPress", true);
				component.enabled = false;
			
			} else {
				component.enabled = true;
				component.gameObject.SendMessage ("OnPress", false);
			}
		}
	}
}
