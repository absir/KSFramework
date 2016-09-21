using UnityEngine;
using System.Collections;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	public class AB_Animator : MonoBehaviour
	{
		public int index;

		private Action<int> callback;

		public void SetCallback (Action<int> callback)
		{
			DoReset ();
			this.callback = callback;
		}

		public void CompleteCallback (Action<int> callback)
		{
			if (this.callback == callback) {
				DoReset ();
			}
		}

		void OnDisable ()
		{
			DoReset ();
		}

		public void DoReset ()
		{
			index = 0;
			if (callback != null) {
				callback (-1);
				callback = null;
			}
		}

		public void DoCallback ()
		{
			if (callback != null) {
				callback (index);
			}

			index++;
		}

		[SLua.StaticExport]
		public static void PayAnimatorName (Animator animator, string name, float delay, Action<int> callback)
		{
			animator.Play (name);
			AddAnimatorCallback (animator, delay, callback);
		}

		[SLua.StaticExport]
		public static void AddAnimatorCallback (Animator animator, float delay, Action<int> callback)
		{
			if (callback == null) {
				return;
			}
			
			AB_Animator _animator = animator.GetComponent<AB_Animator> ();
			if (_animator != null) {
				_animator.SetCallback (callback);
			}

			AB_Context.ME.StartCoroutine (EnumeratorAnimatorCallback (animator, delay, callback, _animator));
		}

		protected static IEnumerator EnumeratorAnimatorCallback (Animator animator, float delay, Action<int> callback, AB_Animator _animator)
		{
			if (callback == null) {
				yield break;
			}

			yield return 0;
			AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo (0);
			delay = info.length + delay;
			if (delay > 0) {
				yield return new WaitForSeconds (delay);
			}

			if (_animator) {
				_animator.CompleteCallback (callback);

			} else {
				callback (-1);
			}
		}
	}
}
