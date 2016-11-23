using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

		protected object[] _cacheParam;

		private LuaFunction _luaUpdateFunction;

		private LuaFunction _luaLastUpdateFunction;

		override public object Call (string call, params object[] args)
		{
			return LuaTableCall (_luaTable, call, args == null || args.Length == 0 ? _cacheParam : args);
		}

		override public void NameCall (string nameCall)
		{
			if (_luaTable != null) {
				int pos = nameCall.IndexOf (',');
				if (pos > 0) {
					Call (nameCall.Substring (0, pos), _luaTable, nameCall.Substring (pos + 1));

				} else {
					Call (nameCall, _cacheParam);
				}
			}
		}

		public LuaTable LuaTable ()
		{
			return _luaTable;
		}

		protected void Awake ()
		{
			if (string.IsNullOrEmpty (lua)) {
				AB_Game.AddLogicStartActions (AwakeLua);
			}
		}

		protected void AwakeLua ()
		{
			LoadAwakeLua (lua);
		}

		public void LoadAwakeLua (string lua)
		{
			if (_luaTable != null) {
				throw new UnityException (gameObject.name + " has load lua " + this.lua + ", could not load other lua " + lua);
			}

			this.lua = lua;
			LoadLuaTable (lua, out _luaTable, true, this);
			if (_luaTable != null) {
				_cacheParam = new object[]{ _luaTable };
				Call ("Awake", _cacheParam);
			}
		}

		protected void Start ()
		{
			if (_luaTable != null) {
				Call ("Start", _cacheParam);
				_luaUpdateFunction = (LuaFunction)_luaTable ["Update"];
				_luaLastUpdateFunction = (LuaFunction)_luaTable ["LateUpdate"];
			}
		}

		protected void Update ()
		{
			if (_luaUpdateFunction != null) {
				_luaUpdateFunction.call (_cacheParam);
			}
		}

		protected void LateUpdate ()
		{
			if (_luaLastUpdateFunction != null) {
				_luaLastUpdateFunction.call (_cacheParam);
			}
		}

		protected void OnEnable ()
		{
			Call ("OnEnable", _cacheParam);
		}

		protected void OnDisable ()
		{
			Call ("OnDisable", _cacheParam);
		}

		protected void OnDestory ()
		{
			Call ("OnDestory", _cacheParam);
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

		public static bool HasLuaPath (string lua)
		{
			var relPath = LuaRelPath (lua);
			return KSGame.Instance.LuaModule.HasScript (relPath);
		}

		protected static Dictionary<string, Func<object, GameObject>> typeDictFunc = new Dictionary<string, Func<object, GameObject>> ();

		static AB_LUA ()
		{
			typeDictFunc.Add ("UnityEngine.Transform", (go) => {
				return go.transform;
			});

			typeDictFunc.Add ("UnityEngine.RectTransform", (go) => {
				return go.transform;
			});
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

					object obj = outletInfo.Object;
					var gameObj = obj as GameObject;
					if (gameObj != null) {
						string type = outletInfo.ComponentType;
						Func<object, GameObject> func = null;
						typeDictFunc.TryGetValue (type, out func);
						if (func == null) {
							obj = gameObj.GetComponent (outletInfo.ComponentType);

						} else {
							obj = func (gameObj);
						}

						if (obj == null) {
							obj = outletInfo.Object;
						}
					} 

					luaTable [outletInfo.Name] = obj;
					//Debug.Log (outletInfo.Name + " = " + outletInfo.ComponentType + " : " + obj);
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
			return LuaTableCallSelf (luaTable, call, true, args);
		}

		public static object LuaTableCallSelf (LuaTable luaTable, string call, bool selfCheck, params object[] args)
		{
			if (luaTable != null) {
				var luaCallObj = luaTable [call];
				if (luaCallObj != null) {
					if (selfCheck) {
						int length = args == null ? 0 : args.Length;
						if (length > 0) {
							if (args [0] != luaTable) {
								object[] newArgs = new object[length + 1];
								newArgs [0] = luaTable;
								int i = 0;
								newArgs [i++] = luaCallObj;
								foreach (object arg in args) {
									newArgs [i++] = arg;
								}

								args = newArgs;
							}

						} else {
							object[] newArgs = new object[1];
							newArgs [0] = luaTable;
							args = newArgs;
						}
					}

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

