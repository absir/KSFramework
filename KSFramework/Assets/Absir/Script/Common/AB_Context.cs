using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public class AB_Context : MonoBehaviour
	{
		public static System.DateTime ZERO_DATE = new System.DateTime (1970, 1, 1);

		// 定时时间
		public System.DateTime contextDate;

		public long contextTime;

		// 动作执行速度，调度
		public int actionRate;

		public int actionDeplete;

		// 主线程动作
		private List<Action> addActions = new List<Action> ();

		private LinkedList<Action> actionQueue = new LinkedList<Action> ();

		// 定时执行全部检测
		private List<ActionObj<long, Action, bool>> addDelayActions = new List<ActionObj<long, Action, bool>> ();

		private LinkedList<ActionObj<long, Action, bool>> delayActionQueue = new LinkedList<ActionObj<long, Action, bool>> ();

		// 定时执行提前排序
		private List<ActionObj<long, Action, bool>> addScheduleActions = new List<ActionObj<long, Action, bool>> ();

		private LinkedList<ActionObj<long, Action, bool>> scheduleActionQueue = new LinkedList<ActionObj<long, Action, bool>> ();

		public void AddAction (Action action, int deplete = 0)
		{
			if (action == null) {
				return;
			}

			Action newAction = null;
			if (actionRate > 0 && deplete > 0) {
				newAction = () => {
					actionDeplete += deplete;
					action ();
				};
			} 

			lock (addActions) {
				addActions.Add (newAction == null ? action : newAction);
			}

		}

		public void AddDelayAction (Action action, long delay, bool schedule = false, bool backgroud = false)
		{
			if (action == null) {
				return;
			}
			
			ActionObj<long, Action, bool> actionObj = ActionObj<long, Action, bool>.newObj (contextTime + delay, action, backgroud);
			if (schedule) {
				lock (addScheduleActions) {
					addScheduleActions.Add (actionObj);
				}

			} else {
				lock (addDelayActions) {
					addDelayActions.Add (actionObj);
				}
			}
		}

		public void AddScheduleAction (Action action, long schedule, bool backgroud = false)
		{
			if (action == null) {
				return;
			}

			ActionObj<long, Action, bool> actionObj = ActionObj<long, Action, bool>.newObj (schedule, action, backgroud);
			lock (addScheduleActions) {
				addScheduleActions.Add (actionObj);
			}
		}

		// Use this for initialization
		void Awake ()
		{
			initContext ();
			if (_ME != null) {
				throw new UnityException ("AB_Context has setted");
			}

			_ME = this;
		}

		void OnDestory ()
		{
			throw new UnityException ("AB_Context could not destory");
		}

		private void CalTime ()
		{
			contextDate = System.DateTime.Now;
			contextTime = contextDate.Ticks / 10;
		}

		protected void CalcMore ()
		{
			
		}
	
		// Update is called once per frame
		void Update ()
		{
			CalcMore ();
			if (addActions.Count > 0) {
				lock (addActions) {
					foreach (Action action in addActions) {
						actionQueue.AddLast (action);
					}

					addActions.Clear ();
				}
			}

			while (true) {
				LinkedListNode<Action> first = actionQueue.First;
				if (first == null || first.Value == null) {
					break;
				}

				actionDeplete++;
				actionQueue.Remove (first);
				first.Value ();
				if (actionDeplete >= actionRate) {
					break;
				}
			}
		}

		public static AB_Context ME {
			get {
				if (_ME == null) {
					_ME = newContext ();
				}

				return _ME;
			}
		}

		private static AB_Context _ME;

		private static AB_Context newContext ()
		{
			AB_Context context = new GameObject ().AddComponent<AB_Context> ();
			context.initContext ();
			return context;
		}

		private bool inited;

		private System.Threading.Thread _currentThread;

		public System.Threading.Thread CurrentThread {
			get {
				return _currentThread;
			}
		}

		public bool isMainThread ()
		{
			return System.Threading.Thread.CurrentThread == _currentThread;
		}

		private void initContext ()
		{
			if (inited) {
				return;
			}

			inited = true;
			_currentThread = System.Threading.Thread.CurrentThread;
			DontDestroyOnLoad (gameObject);
			gameObject.name = "_AB_Context";
			CalTime ();
			CalcMore ();
			new System.Threading.Thread (threadContext).Start ();
		}

		private void threadContext ()
		{
			while (true) {
				if (addDelayActions.Count > 0) {
					lock (addDelayActions) {
						foreach (var action in addDelayActions) {
							delayActionQueue.AddLast (action);
						}

						addDelayActions.Clear ();
					}
				}

				if (addScheduleActions.Count > 0) {
					ActionObj<long, Action, bool>[] actions;
					lock (addScheduleActions) {
						actions = addScheduleActions.ToArray ();
						addScheduleActions.Clear ();
					}

					foreach (var action in actions) {
						var node = scheduleActionQueue.First;
						while (node != null && node.Value != null) {
							if (node.Value.t1 >= action.t1) {
								break;
							}

							node = node.Next;
						}

						if (node == null) {
							scheduleActionQueue.AddLast (action);

						} else {
							scheduleActionQueue.AddBefore (node, action);
						}
					}

					CalTime ();
					long time = contextTime;
					{
						// 定时执行全部检测
						var node = delayActionQueue.First;
						while (node != null && node.Value != null) {
							var action = node.Value;
							node = node.Next;
							if (action.t1 <= time) {
								delayActionQueue.Remove (node);
								InvokeAction (action.t2, action.t3);
							}
						}
					}

					{
						// 定时执行提前排序
						var node = scheduleActionQueue.First;
						while (node != null && node.Value != null) {
							var action = node.Value;
							if (action.t1 <= time) {
								node = node.Next;
								scheduleActionQueue.Remove (node);
								InvokeAction (action.t2, action.t3);

							} else {
								break;
							}
						}
					}

					System.Threading.Thread.Sleep (1000);
				}
			}
		}

		public void InvokeAction (Action action, bool backgroud)
		{
			if (backgroud) {
				try {
					action ();

				} catch (System.Exception e) {
					KEngine.Log.Error ("AB_Context InvokeAction action error", action);
				}
			
			} else {
				AddAction (action);
			}
		}

		public static void EditorMainThread ()
		{
			#if UNITY_EDITOR
			if (!ME.isMainThread ()) {
				throw new UnityException ("editor is not main thread");
			}
			#endif
		}
	}
}
