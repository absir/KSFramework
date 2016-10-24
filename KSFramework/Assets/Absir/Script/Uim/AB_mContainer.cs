using UnityEngine;
using System.Collections;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_mContainer : AB_Viewor
	{
		public Transform containerTrans;

		override protected void initComponent ()
		{
			if (containerTrans == null) {
				containerTrans = gameObject.transform;
			}
		}
	}
}
