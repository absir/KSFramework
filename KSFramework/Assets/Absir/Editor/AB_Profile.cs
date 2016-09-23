using System.IO;
using UnityEditor;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Absir
{
	public class AB_ProfileCxt : MonoBehaviour
	{
		private LinkedList<Action> actions = new LinkedList<Action> ();

		public void AddAction (Action action)
		{
			lock (actions) {
				actions.AddLast (action);
			}
		}

		void Update ()
		{
			LinkedListNode<Action> first = null;
			lock (actions) {
				first = actions.First;
				if (first != null) {
					actions.RemoveFirst ();
					gameObject.name = "_profileCxt_" + actions.Count;
				}
			}

			if (first != null) {
				Action action = first.Value;
				if (action != null) {
					//Debug.Log ("_ProfileCxt => " + action);
					action ();	
				}
			}
		}
	}

	public class AB_Profile
	{
		private static AB_ProfileCxt _profileCxt;

		private static object _profileLock = new object ();

		private static bool _profiling;

		private static string _profileName;

		public static AB_ProfileCxt GetProfileCxt ()
		{
			if (_profileCxt == null) {
				_profileCxt = GameObject.FindObjectOfType<AB_ProfileCxt> ();
				if (_profileCxt == null) {
					_profileCxt = new GameObject ().AddComponent<AB_ProfileCxt> ();
				}

				_profileCxt.gameObject.name = "_profileCxt";
			}

			return _profileCxt;
		}

		private static void println (string log)
		{
			GetProfileCxt ().AddAction (() => {
				Debug.Log (log);
			});
		}

		private static void errorln (string error)
		{
			GetProfileCxt ().AddAction (() => {
				Debug.LogError (error);
			});
		}

		private static void startProfile (string name)
		{
			if (!Application.isPlaying) {
				throw new UnityException ("AB_Profile must in application playing");
			}

			lock (_profileLock) {
				if (_profiling) {
					throw new UnityException ("Absir is profiling");
				}

				_profiling = true;
			}

			GetProfileCxt ();
			_profiling = true;
			_profileName = name;
			println (_profileName + " profile is started");

		}

		private static void stopProfile ()
		{
			GetProfileCxt ().AddAction (() => {
				_profiling = false;
				_profileName = null;
				println (_profileName + " profile is stoped");
			});
		}

		[MenuItem ("AB_Edtior/Profile/Static Components")]
		public static void StaticComponents ()
		{
			startProfile ("StaticComponents");
			new Thread (ThreadStaticComponents).Start ();
		}

		private static void ThreadStaticComponents ()
		{
			try {
				List<object> staticObjects = new List<object> ();
				try {
					// export plugin-dll
					var assembly = Assembly.Load ("Assembly-CSharp-firstpass");
					var types = assembly.GetTypes ();
					foreach (Type t in types) {
						ThreadStaticComponents (t, staticObjects);
					}

				} catch (Exception) {
				}

				// export self-dll
				{
					var assembly = Assembly.Load ("Assembly-CSharp");
					var types = assembly.GetTypes ();
					foreach (Type t in types) {
						ThreadStaticComponents (t, staticObjects);
					}
				}

			} finally {
				stopProfile ();
			}
		}

		private static object GetFieldValue (FieldInfo field, object obj)
		{
			try {
				return field.GetValue (obj);

			} catch (Exception) {
			}

			return null;
		}

		private static void ThreadStaticComponents (System.Type type, List<object> staticObjects)
		{
			//println ("ThreadStaticComponents + " + type);
//			if (type.FullName.Contains ("StarsRewardManager")) {
//				println ("ThreadStaticComponents + " + type);
//			}

			string path = type.FullName;
			foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly)) {
				if (field.IsStatic) {
					var f = field;
					GetProfileCxt ().AddAction (() => {
						object value = GetFieldValue (f, type);
						//					if (type.FullName.EndsWith("StarsRewardManager")) {
						//						println (">>>>>> ThreadStaticComponents " + path + "." + field.Name + " = " + value);
						//					}

						if (value != null) {
							//						println (">>>>>> ThreadStaticComponents " + path + "." + f.Name + " = " + value);
							ThreadStaticComponents (path + "." + f.Name, value, staticObjects);
						}
					});
				}
			}
		}

		private static bool ThreadStaticComponents (string path, object obj, List<object> staticObjects)
		{
			if (obj == null) {
				return false;
			}

			foreach (var o in staticObjects) {
				if (o == obj) {
					return false;
				}
			}

			staticObjects.Add (obj);

//			println (">>>>>> ThreadStaticComponents " + path + " = " + obj);
			System.Type type = obj.GetType ();
			if (type.FullName.StartsWith ("System")) {
//				if (type.IsArray) {
//					Debug.Log ("type.IsArray === " + obj);
//				}

				if (obj is object[]) {
					foreach (var v in (object[])obj) {
						if (ThreadStaticComponents (path + "[]", v, staticObjects)) {
							return true;
						}
					}
				
				} else if (obj is ICollection) {
//					println (">>>>>> ThreadStaticComponents.ICollection " + path + " = " + obj);
					foreach (var v in (ICollection)obj) {
						if (ThreadStaticComponents (path + "[]", v, staticObjects)) {
							return true;
						}
					}
				
				} else if (obj is IDictionary) {
					IDictionary dict = (IDictionary)obj;
					//ICollection keys = dict.Keys;
					foreach (var v in dict.Values) {
						if (ThreadStaticComponents (path + "[]", v, staticObjects)) {
							return true;
						}
					}
				}


				return false;
			}
				
//			println (">>>>>> ThreadStaticComponents " + path + " = " + obj);
			//&& !((Component)obj).gameObject
			if (obj is Component) {
				try {
					if (((Component)obj).gameObject) {
						return false;
					}
				} catch (Exception) {
				}

				println (">>>>>> ThreadStaticComponents " + path + " = " + obj.GetType ());
				return true;
			}

			bool component = false;
			foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic)) {
				if (!(field.IsStatic)) {
					if (ThreadStaticComponents (path + "." + field.Name, GetFieldValue (field, obj), staticObjects)) {
						component = true;
					}
				}
			}

			return component;
		}
	}
}
