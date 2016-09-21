using UnityEngine;
using System.Collections;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	public class AB_Bar : AB_Seg
	{
		public CanComponent canComponent;

		override protected void initComponent ()
		{
			base.initComponent ();
			setTabComponentIndex ();
		}

		override public bool setActiveComponentIndex (int componentIndex)
		{
			if (base.setActiveComponentIndex (componentIndex)) {
				setTabComponentIndex ();
				return true;
			}
		
			return false;
		}

		virtual protected void setTabComponentIndex ()
		{
			if (canComponent != null) {
				canComponent.setActiveComponentIndex (activeComponentIndex);
			}
		}
	}
}
