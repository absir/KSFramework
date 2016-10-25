using UnityEngine;
using System.Collections;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public abstract class AB_Call : MonoBehaviour
	{
		abstract public object Call (string name, params object[] args);

		//public const object NULL_CALL_OBJECT = new object ();

		public static AB_Call Find (GameObject go)
		{
			return ComponentUtils.GetComponentObject<AB_Call> (go);
		}

		public static object DoCall (AB_Call call, string name, params object[] args)
		{
			if (call != null) {
				return call.Call (name, args);
			}

			return null;
		}
	}
}
