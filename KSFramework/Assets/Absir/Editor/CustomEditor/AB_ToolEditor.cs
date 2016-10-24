using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[CustomEditor (typeof(AB_Tool), true)] 
	public class AB_ToolEditor : Editor
	{
		[HideInInspector]
		private bool triggering;

		private static List<IEnumerator> addCoroutines = new List<IEnumerator> ();

		private static List<AB_Coroutine> runingCoroutines = new List<AB_Coroutine> ();

		private static bool runned = true;

		private static long contextTime;

		static AB_ToolEditor ()
		{
			EditorApplication.update += () => {
				UpdateCoroutine ();
				runned = false;
			};
		}

		public static void StartCoroutine (IEnumerator coroutine)
		{
			if (coroutine != null) {
				lock (addCoroutines) {
					addCoroutines.Add (coroutine);
				}
			}
		}

		protected static void UpdateCoroutine ()
		{
			if (!runned) {
				runned = true;
				long time = System.DateTime.Now.Ticks / 10000;
				if (contextTime != 0) {
					AB_Coroutine.deltaTime = (time - contextTime) / 1000.0f;
				}

				contextTime = time;

				if (addCoroutines.Count > 0) {
					lock (addCoroutines) {
						foreach (IEnumerator coroutine in addCoroutines) {
							runingCoroutines.Add (new AB_Coroutine (coroutine));
						}

						addCoroutines.Clear ();
					}
				}

				if (runingCoroutines.Count > 0) {
					runingCoroutines.RemoveAll (
						coroutine => {
							return !coroutine.MoveNextSafe ();
						}  
					);  
				}
			}
		}

		public override void OnInspectorGUI ()
		{
			AB_Tool tool = (AB_Tool)target;
			bool trigger = EditorGUILayout.Toggle ("Do Trigger", triggering);
			if (trigger != triggering) {
				if (trigger) {
					triggering = true;
					StartCoroutine (doTrigger (tool));

				} else {
					Debug.Log (tool + " Do Trigger Intecept");
					tool.Intecepted = true;
				}
			}

			base.OnInspectorGUI ();
		}

		public IEnumerator doTrigger (AB_Tool tool)
		{
			triggering = true;
			Debug.Log (tool + " Do Trigger Start");
			try {
				tool.Intecepted = false;
				yield return tool.DoTrigger ();

			} finally {
				Debug.Log (tool + " Do Trigger Complete");
			}

			triggering = false;
		}

	}
}
