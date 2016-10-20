using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	public class AB_Seg : AB_Cats<Button>
	{
		protected int activeComponentIndex;

		public Action<int> componentIndexAction;

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

		protected override void bindCat (AB_Cat cat, Button component)
		{
			component.onClick.AddListener (cat.OnClick);
		}

		override public bool setActiveComponentIndex (int componentIndex)
		{
			if (base.setActiveComponentIndex (componentIndex)) {
				activeComponentIndex = componentIndex;
				if (componentIndexAction != null) {
					componentIndexAction (componentIndex);
				}

				return true;
			}
		
			return false;
		}

		override protected bool isComponentActive (Button component)
		{
			return !component.interactable;
		}

		override protected void setComponentActive (Button component, bool active)
		{
			if (active) {
				//component.gameObject.SendMessage ("OnPress", true);
				component.interactable = false;
			
			} else {
				component.interactable = true;
				//component.gameObject.SendMessage ("OnPress", false);
			}
		}
	}
}
