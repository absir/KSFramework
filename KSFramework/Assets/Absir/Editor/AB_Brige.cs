using UnityEngine;
using System.Collections;

namespace Absir
{
	public class AB_Brige : IE_Brige
	{
		public void ReloadLua ()
		{
			foreach (AB_LUA lua in GameObject.FindObjectsOfType<AB_LUA> ()) {
				AB_LUA.ReloadLuaBehaviour (lua.lua);
			}

			Debug.Log ("AB_Brige.ReloadLua");
		}

		public void BeforeReloadUI ()
		{
		}

		public void AfterReloadUI ()
		{
		}

		public void BeforeReloadUILua ()
		{
		}

		public void AfterReloadUILua ()
		{
		}
	}
}
