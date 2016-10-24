using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_TabContainer : AB_Can<AB_Viewor>
	{
		override protected List<AB_Viewor> getComponentSort ()
		{
			return GameObjectUtils.getChildrenGameObjectComponentSort<AB_Viewor> (gameObject);
		}

		override protected void initComponent (AB_Viewor viewor)
		{
			GameObjectUtils.getOrAddComponent<AB_Retain> (viewor.gameObject).retain ();
		}

		override protected bool isComponentActive (AB_Viewor component)
		{
			if (component.gameObject.transform.parent != null) {
				if (component.gameObject.activeSelf) {
					return true;
				}
			
				component.gameObject.SetActive (true);
				setComponentActive (component, false);
			}

			return false;
		}

		override protected void setComponentActive (AB_Viewor component, bool status)
		{
			if (status) {
				component.doAppearTransform (gameObject.transform);
			
			} else {
				component.doDisappearTransform ();
			}
		}
	}
}
