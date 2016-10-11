using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
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

		private Dictionary<object, LinkedList<ActionObj<object, Action<object>, Component>>> nameDictTargetActions = new Dictionary<object, LinkedList<ActionObj<object, Action<object>, Component>>> ();

		private Dictionary<object, List<object>> targetDictNames = new Dictionary<object, List<object>> ();

		private AB_Notification ()
		{
		}

		public void Post (object name, object obj)
		{
			LinkedList<ActionObj<object, Action<object>, Component>> targetActions = null;
			nameDictTargetActions.TryGetValue (name, out targetActions);
			if (targetActions != null) {
				List<object> removes = null;
				foreach (var actionObj in targetActions) {
					if (actionObj.t3 == null || actionObj.t3.gameObject) {
						actionObj.t2 (obj);

					} else {
						if (removes == null) {
							removes = new List<object> ();
						}

						removes.Add (actionObj.t1);
					}
				}

				if (removes != null) {
					foreach (object remove in removes) {
						RemoveObserve (remove);
					}
				}
			}
		}

		public void AddObserve (object target, object name, Action<object> action)
		{
			List<object> names = null;
			targetDictNames.TryGetValue (target, out names);
			if (names == null) {
				names = new List<object> ();
				targetDictNames.Add (target, names);
			
			} else {
				if (names.Contains (name)) {
					Brige.ME.LogError ("AB_Notification AddObserve has [" + name + "] =>" + target + " =>" + action);
					return;
				}
			}

			names.Add (name);
			LinkedList<ActionObj<object, Action<object>, Component>> targetActions = null;
			nameDictTargetActions.TryGetValue (name, out targetActions);
			if (targetActions == null) {
				targetActions = new LinkedList<ActionObj<object, Action<object>, Component>> ();
				nameDictTargetActions.Add (name, targetActions);
			}

			targetActions.AddLast (ActionObj<object, Action<object>, Component>.newObj (target, action, target as Component));
		}

		public void RemoveObserve (object target)
		{
			List<object> names = null;
			targetDictNames.TryGetValue (target, out names);
			if (names != null) {
				targetDictNames.Remove (target);
				foreach (var name in names) {
					RemoveObserveNameCheck (target, name, false);
				}
			}
		}

		public void RemoveObserveName (object target, object name)
		{
			RemoveObserveNameCheck (target, name, true);
		}

		private void RemoveObserveNameCheck (object target, object name, bool checkClear)
		{
			LinkedList<ActionObj<object, Action<object>, Component>> targetActions = null;
			nameDictTargetActions.TryGetValue (name, out targetActions);
			if (targetActions != null) {
				LinkedListNode<ActionObj<object, Action<object>, Component>> node = targetActions.First;
				while (node != null && node.Value != null) {
					ActionObj<object, Action<object>, Component> actionObj = node.Value;
					if (actionObj.t1 == target) {
						targetActions.Remove (node);
						if (checkClear) {
							List<object> names = null;
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

		public void CheckObserveGC ()
		{
			List<object> removes = null;
			foreach (var pair in targetDictNames) {
				Component component = pair.Key as Component;
				if (component != null && !component.gameObject) {
					if (removes == null) {
						removes = new List<object> ();
					}

					removes.Add (component);
				}
			}

			if (removes != null) {
				foreach (object remove in removes) {
					RemoveObserve (remove);
				}
			}
		}
	}
}

