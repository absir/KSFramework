using UnityEngine;
using System.Collections;
using SLua;
using KSFramework;
using KEngine;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]

	public class AB_LUA : AB_Call
	{
		public static bool OnDestoryReload;

		public string lua;

		protected LuaTable _luaTable;

		private LuaFunction _luaUpdateFunction;

		private LuaFunction _luaLastUpdateFunction;

		override public object Call (string call, params object[] args)
		{
			return LuaTableCall (_luaTable, call, args);
		}

		public LuaTable LuaTable ()
		{
			return _luaTable;
		}

		protected void Awake ()
		{
			AB_Game.AddLogicStartActions (AwakeLua);
		}

		protected void AwakeLua ()
		{
			LoadLuaTable (lua, out _luaTable, true, this);
			if (_luaTable != null) {
				Call ("Awake");
			}
		}

		protected void Start ()
		{
			if (_luaTable != null) {
				Call ("Start");
				_luaUpdateFunction = (LuaFunction)_luaTable ["Update"];
				_luaLastUpdateFunction = (LuaFunction)_luaTable ["LateUpdate"];
			}
		}

		protected void Update ()
		{
			if (_luaUpdateFunction != null) {
				_luaUpdateFunction.call ();
			}
		}

		protected void LateUpdate ()
		{
			if (_luaLastUpdateFunction != null) {
				_luaLastUpdateFunction.call ();
			}
		}

		protected void OnEnable ()
		{
			Call ("OnEnable");
		}

		protected void OnDisable ()
		{
			Call ("OnDisable");
		}

		protected void OnDestory ()
		{
			Call ("OnDestory");
			#if UNITY_EDITOR
			if (!string.IsNullOrEmpty (lua) && OnDestoryReload) {
				ClearLuaCache (lua);
			}
			#endif
		}

		public static string LuaRelPath (string lua)
		{
			return lua;
		}

		public static bool LoadLuaTable (string lua, out LuaTable luaTable, bool showWarn, Component component)
		{
			luaTable = null;
			if (string.IsNullOrEmpty (lua)) {
				return false;
			}

			var relPath = LuaRelPath (lua);
			var luaModule = KSGame.Instance.LuaModule;
			object scriptResult;
			if (!luaModule.TryImport (relPath, out scriptResult)) {
				if (showWarn)
					Log.LogWarning ("Import UI Lua Script failed: {0}", relPath);
				return false;
			}

			scriptResult = KSGame.Instance.LuaModule.CallScript (relPath);
			KEngine.Debuger.Assert (scriptResult is LuaTable, "{0} Script Must Return Lua Table with functions!", lua);

			luaTable = scriptResult as LuaTable;

			var newFuncObj = luaTable ["New"]; // if a New function exist, new a table!
			if (newFuncObj != null) {
				var newTableObj = (newFuncObj as LuaFunction).call (component);
				luaTable = newTableObj as LuaTable;
			}

			var outlet = component == null ? null : component.GetComponent<UILuaOutlet> ();
			if (outlet != null) {
				for (var i = 0; i < outlet.OutletInfos.Count; i++) {
					var outletInfo = outlet.OutletInfos [i];

					var gameObj = outletInfo.Object as GameObject;

					if (gameObj != null)
						luaTable [outletInfo.Name] = gameObj.GetComponent (outletInfo.ComponentType);
					else
						luaTable [outletInfo.Name] = outletInfo.Object;
				}

				DestroyObject (outlet);
			}

			var luaInitObj = luaTable ["OnInit"];
			//Debuger.Assert (luaInitObj is LuaFunction, "Must have OnInit function - {0}", lua);
			if (luaInitObj != null) {
				(luaInitObj as LuaFunction).call (luaTable, component);
			}

			return true;
		}

		public static object LuaTableCall (LuaTable luaTable, string call, params object[] args)
		{
			if (luaTable != null) {
				var luaCallObj = luaTable [call];
				if (luaCallObj != null) {
					return (luaCallObj as LuaFunction).call (args);
				}
			}

			return null;
		}

		public static void ClearLuaCache (string lua)
		{
			var relPath = LuaRelPath (lua);
			var luaModule = KSFramework.KSGame.Instance.LuaModule;
			luaModule.ClearCache (relPath);
			Brige.ME.LogWarn ("Reload Lua: " + relPath);
		}
	}
}

