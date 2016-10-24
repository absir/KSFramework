using UnityEngine;
using System.Collections;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_Bar : AB_Seg
	{
		public CanComponent canComponent;

		override protected void InitComponent ()
		{
			base.InitComponent ();
			SetTabComponentIndex ();
		}

		override public bool SetActiveComponentIndex (int componentIndex)
		{
			if (base.SetActiveComponentIndex (componentIndex)) {
				SetTabComponentIndex ();
				return true;
			}
		
			return false;
		}

		virtual protected void SetTabComponentIndex ()
		{
			if (canComponent != null) {
				canComponent.SetActiveComponentIndex (activeComponentIndex);
			}
		}
	}
}
