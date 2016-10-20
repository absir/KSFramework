using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using System.Reflection;
#endif

namespace Absir
{
	public class AB_Coroutine : IEnumerator
	{
		private Stack<IEnumerator> executionStack;

		#if UNITY_EDITOR
		public static float deltaTime;

		private float waitForSeconds;

		private FieldInfo m_SecondsFieldInfo = typeof(WaitForSeconds).GetField ("m_Seconds", BindingFlags.Instance | BindingFlags.Default | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
		#endif

		public AB_Coroutine (IEnumerator iterator)
		{
			this.executionStack = new Stack<IEnumerator> ();
			this.executionStack.Push (iterator);
		}

		public bool MoveNext ()
		{
			#if UNITY_EDITOR
			if (waitForSeconds > 0) {
				if (deltaTime > 0) {
					if ((waitForSeconds -= deltaTime) >= 0) {
						return true;		
					}

				} else {
					waitForSeconds = 0;
				}
			}
			#endif

			IEnumerator i = this.executionStack.Peek ();
			if (i.MoveNext ()) {
				object result = i.Current;
				if (result != null) {
					if (result is IEnumerator) {
						this.executionStack.Push ((IEnumerator)result);
						return true;
					} 

					#if UNITY_EDITOR
					if (m_SecondsFieldInfo != null && result is WaitForSeconds) {
						waitForSeconds = (float)m_SecondsFieldInfo.GetValue (result);
						return true;
					}
					#endif
				}

				return true;
			} else {
				if (this.executionStack.Count > 1) {
					this.executionStack.Pop ();
					return true;
				}
			}

			return false;
		}

		public void Reset ()
		{
			throw new System.NotSupportedException ("This Operation Is Not Supported.");
		}

		public object Current {
			get { return this.executionStack.Peek ().Current; }
		}

		public bool Find (IEnumerator iterator)
		{
			return this.executionStack.Contains (iterator);
		}

		public bool MoveNextSafe ()
		{
			try {
				return MoveNext ();

			} catch (System.Exception e) {
				Debug.LogError (e);
			}

			return false;
		}
	}
}
