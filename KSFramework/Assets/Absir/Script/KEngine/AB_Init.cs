using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KEngine;
using KEngine.UI;
using KSFramework;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_Init : MonoBehaviour
	{
		public static bool inited { get; protected set; }

		public static bool started { get; protected set; }

		void Awake ()
		{
			if (!AB_Game.IsonLogicStarted ()) {
				gameObject.SetActive (false);
			}

			Init (true);
			AB_Game.AddLogicStartActions (InitStart);
			AB_Game.AddLogicStartActions (() => {
				if (!gameObject.activeSelf) {
					GameObject go = gameObject;
					AB_Game.AddLogicStartActions (() => {
						AB_Context.ME.StartCoroutine (SetInitActiveDelay (go));
					});
				}
					
				//Destroy (this);
			});
		}

		protected static IEnumerator SetInitActiveDelay (GameObject go)
		{
			while (!AB_Game.IsonLogicStarted ()) {
				yield return 0;
			}

			Debug.Log ("AB_Init Is not onLogicStarted SetActive");
			go.SetActive (true);
		}

		public static void Init (bool start)
		{
			if (!inited) {
				AB_Game game = AB_Resource.LoadModule<AB_Game> ("_AB_Game");
				if (game == null) {
					GameObject obj = new GameObject ();
					obj.name = "_AB_Game";
					game = obj.AddComponent<AB_Game> ();
				}

				DontDestroyOnLoad (game.gameObject);
				inited = true;
			}

			if (start && !started) {
				started = true;
				AB_Game.AddGameStartAction (() => {
					Transform startTransform = AB_Resource.LoadModule<Transform> ("_AB_Start");
					if (startTransform == null) {
						AB_Game.LogicComplete ("AB_Init");
					}
				});

				AB_Game.AddLogicStartActions (() => {
					SLua.LuaTable luaTable = null;
					if (AB_LUA.LoadLuaTable ("Main", out luaTable, true, null)) {
						AB_LUA.LuaTableCall (luaTable, "Start");
					}
				});
			}
		}

		protected virtual void InitStart ()
		{
			
		}

	}
}
