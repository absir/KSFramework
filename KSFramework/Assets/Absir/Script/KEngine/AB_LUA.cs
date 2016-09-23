using UnityEngine;
using System.Collections;
using SLua;
using KSFramework;
using KEngine;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	[RequireComponent (typeof(UILuaOutlet))]
	public class AB_LUA : MonoBehaviour
	{
		public static bool OnDestoryReload;

		public string lua;

		protected LuaTable _luaTable;

		public void Awake ()
		{
			LoadLuaBehaviour (lua, out _luaTable, true, this);
			LuaCall ("Awake");
		}

		protected void Start ()
		{
			LuaCall ("Start");
		}

		protected void Update ()
		{
			LuaCall ("Update");
		}

		protected void LateUpdate ()
		{
			LuaCall ("LateUpdate");
		}

		public LuaTable LuaTable ()
		{
			return _luaTable;
		}

		public void LuaCall (string call, params object[] args)
		{
			LuaTableCall (_luaTable, call, args);
		}

		#if UNITY_EDITOR
		public void OnDestory ()
		{
			if (!string.IsNullOrEmpty (lua) && OnDestoryReload) {
				ReloadLuaBehaviour (lua);
			}
		}
		#endif

		public static string LuaBehaviourPath (string name)
		{
			return name.Contains ("/") ? name : ("Behaviour/" + name);
		}

		public static bool LoadLuaBehaviour (string name, out LuaTable luaTable, bool showWarn, Component component)
		{
			luaTable = null;
			if (string.IsNullOrEmpty (name)) {
				return false;
			}

			var relPath = LuaBehaviourPath (name);
			var luaModule = KSGame.Instance.LuaModule;
			object scriptResult;
			if (!luaModule.TryImport (relPath, out scriptResult)) {
				if (showWarn)
					Log.LogWarning ("Import UI Lua Script failed: {0}", relPath);
				return false;
			}

			scriptResult = KSGame.Instance.LuaModule.CallScript (relPath);
			KEngine.Debuger.Assert (scriptResult is LuaTable, "{0} Script Must Return Lua Table with functions!", name);

			luaTable = scriptResult as LuaTable;

			var newFuncObj = luaTable ["New"]; // if a New function exist, new a table!
			if (newFuncObj != null) {
				var newTableObj = (newFuncObj as LuaFunction).call (component);
				luaTable = newTableObj as LuaTable;
			}

			var outlet = component.GetComponent<UILuaOutlet> ();
			if (outlet != null) {
				for (var i = 0; i < outlet.OutletInfos.Count; i++) {
					var outletInfo = outlet.OutletInfos [i];

					var gameObj = outletInfo.Object as GameObject;

					if (gameObj != null)
						luaTable [outletInfo.Name] = gameObj.GetComponent (outletInfo.ComponentType);
					else
						luaTable [outletInfo.Name] = outletInfo.Object;
				}
			}

			var luaInitObj = luaTable ["OnInit"];
			Debuger.Assert (luaInitObj is LuaFunction, "Must have OnInit function - {0}", name);

			(luaInitObj as LuaFunction).call (luaTable, component);

			return true;
		}

		public static void ReloadLuaBehaviour (string name)
		{
			var relPath = LuaBehaviourPath (name);
			var luaModule = KSFramework.KSGame.Instance.LuaModule;
			luaModule.ClearCache (relPath);
			Brige.ME.LogWarn ("Reload Lua: " + relPath);
		}

		public object LuaTableCall (LuaTable luaTable, string call, params object[] args)
		{
			if (luaTable != null) {
				var luaCallObj = luaTable [call];
				if (luaCallObj != null) {
					(luaCallObj as LuaFunction).call (args);
				}
			}

			return null;
		}
	}
}

