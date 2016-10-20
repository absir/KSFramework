﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KEngine;
using KSFramework;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_Game : KSGame
	{
		private static List<Action> onBeforeActions = new List<Action> ();

		private static List<Action> onGameStartActions = new List<Action> ();

		private static int onLogicWaite;

		private static List<Action> onLogicStartActions = new List<Action> ();

		public static void RunActions (List<Action> actions, Action complete)
		{
			KResourceModule.Instance.StartCoroutine (RunActionsEnumerator (actions, complete));
		}

		public static IEnumerator RunActionsEnumerator (List<Action> actions, Action complete)
		{
			while (true) {
				int count = actions.Count;
				if (count <= 0) {
					break;
				}

				for (int i = 0; i < count; i++) {
					try {
						actions [i] ();

					} catch (System.Exception e) {
						Log.Error ("RunAction Exception " + e);
					}
				}

				actions.RemoveRange (0, count);
				yield return 0;
			}

			if (complete != null) {
				complete ();
			}
		}

		public static void AddBeforeAction (Action action)
		{
			if (action != null) {
				if (onBeforeActions == null) {
					action ();

				} else {
					onBeforeActions.Add (action);
				}
			}
		}

		public static void AddGameStartAction (Action action)
		{
			if (action != null) {
				if (onGameStartActions == null) {
					action ();

				} else {
					onGameStartActions.Add (action);
				}
			}
		}

		public static void AddLogicStartActions (Action action)
		{
			if (action != null) {
				if (onLogicStartActions == null) {
					action ();

				} else {
					onLogicStartActions.Add (action);
				}
			}
		}

		public static void LoginWaite (string name)
		{
			Log.Info ("LoginWaite " + name);
			onLogicWaite++;
			if (onLogicStartActions == null) {
				onLogicStartActions = new List<Action> ();
			}
		}

		public static void LogicComplete (string name)
		{
			Log.Info ("LogicComplete " + name);
			if (onLogicWaite > 0) {
				onLogicWaite--;

			} else {
				if (onLogicStartActions != null) {
					RunActions (onLogicStartActions, () => {
						onLogicStartActions = null;
					});
				}
			}
		}

		protected override void Awake ()
		{
			base.Awake ();
		}

		public override IEnumerator OnBeforeInit ()
		{
			yield return 0;
			if (onBeforeActions != null) {
				yield return RunActionsEnumerator (onBeforeActions, null);
				onBeforeActions = null;
			}
		}

		public override IEnumerator OnGameStart ()
		{
			yield return 0;
			if (onGameStartActions != null) {
				yield return RunActionsEnumerator (onGameStartActions, null);
				onGameStartActions = null;
			}
		}
	}
}
