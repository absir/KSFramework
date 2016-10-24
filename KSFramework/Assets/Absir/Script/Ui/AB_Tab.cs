using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_Tab : AB_Can<Transform>
	{
		override protected bool IsComponentActive (Transform component)
		{
			return component.gameObject.activeSelf;
		}

		override protected void SetComponentActive (Transform component, bool status)
		{
			AB_UI.ME.SetViewActive (component, status);
		}
	}
}