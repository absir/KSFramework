using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_TabContainer : AB_Can<AB_Viewor>
	{
		override protected List<AB_Viewor> GetComponentSort ()
		{
			return GameObjectUtils.GetChildrenGameObjectComponentSort<AB_Viewor> (gameObject);
		}

		override protected void InitComponent (AB_Viewor viewor)
		{
			GameObjectUtils.GetOrAddComponent<AB_Retain> (viewor.gameObject).Retain ();
		}

		override protected bool IsComponentActive (AB_Viewor component)
		{
			if (component.gameObject.transform.parent != null) {
				if (component.gameObject.activeSelf) {
					return true;
				}
			
				component.gameObject.SetActive (true);
				SetComponentActive (component, false);
			}

			return false;
		}

		override protected void SetComponentActive (AB_Viewor component, bool status)
		{
			if (status) {
				component.DoAppearTransform (gameObject.transform);
			
			} else {
				component.DoDisappearTransform ();
			}
		}
	}
}
