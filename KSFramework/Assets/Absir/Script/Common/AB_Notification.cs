using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public class AB_Notification
	{
		public static AB_Notification ME {
			get {
				AB_Context.EditorMainThread ();
				if (_ME == null) {
					_ME = new AB_Notification ();
				}

				return _ME;
			}
		}

		private static AB_Notification _ME;

		private Dictionary<AB_Event, LinkedList<ActionObj<object, Action<object>>>> nameDictTargetActions = new Dictionary<AB_Event, LinkedList<ActionObj<object, Action<object>>>> ();

		private Dictionary<object, List<AB_Event>> targetDictNames = new Dictionary<object, List<AB_Event>> ();

		private AB_Notification ()
		{
		}

		public void Post (AB_Event name, object obj)
		{
			LinkedList<ActionObj<object, Action<object>>> targetActions = null;
			nameDictTargetActions.TryGetValue (name, out targetActions);
			if (targetActions != null) {
				foreach (var actionObj in targetActions) {
					actionObj.t2 (obj);
				}
			}
		}

		public void AddObserve (object target, AB_Event name, Action<object> action)
		{
			List<AB_Event> names = null;
			targetDictNames.TryGetValue (target, out names);
			if (names == null) {
				names = new List<AB_Event> ();
				targetDictNames.Add (target, names);
			
			} else {
				if (names.Contains (name)) {
					KEngine.Log.Error ("AB_Notification AddObserve has [" + name + "]", target, action);
					return;
				}
			}

			names.Add (name);
			LinkedList<ActionObj<object, Action<object>>> targetActions = null;
			nameDictTargetActions.TryGetValue (name, out targetActions);
			if (targetActions == null) {
				targetActions = new LinkedList<ActionObj<object, Action<object>>> ();
				nameDictTargetActions.Add (name, targetActions);
			}

			targetActions.AddLast (ActionObj<object, Action<object>>.newObj (target, action));
		}

		public void RemoveObserve (object target)
		{
			List<AB_Event> names = null;
			targetDictNames.TryGetValue (target, out names);
			if (names != null) {
				targetDictNames.Remove (target);
				foreach (var name in names) {
					RemoveObserve (target, name, false);
				}
			}
		}

		public void RemoveObserve (object target, AB_Event name)
		{
			RemoveObserve (target, name, true);
		}

		private void RemoveObserve (object target, AB_Event name, bool checkClear)
		{
			LinkedList<ActionObj<object, Action<object>>> targetActions = null;
			nameDictTargetActions.TryGetValue (name, out targetActions);
			if (targetActions != null) {
				LinkedListNode<ActionObj<object, Action<object>>> node = targetActions.First;
				while (node != null && node.Value != null) {
					ActionObj<object, Action<object>> actionObj = node.Value;
					if (actionObj.t1 == target) {
						targetActions.Remove (node);
						if (checkClear) {
							List<AB_Event> names = null;
							targetDictNames.TryGetValue (target, out names);
							if (names != null) {
								if (names.Remove (name) && names.Count == 0) {
									targetDictNames.Remove (target);
								}
							}
						}

						break;
					}

					node = node.Next;
				}
			}
		}
	}
}

