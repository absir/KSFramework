using UnityEngine;
using System.Collections;

namespace Absir
{
	public class AB_AniEvent : MonoBehaviour
	{
		public int index;

		public Action<int> callback;

		void OnEnable ()
		{
			DoReset ();
		}

		public void DoReset ()
		{
			index = 0;
		}

		public void DoEvent ()
		{
			if (callback != null) {
				callback (index);
			}

			index++;
		}

		public static void PayAnimatorName (Animator animator, string name, Action<int> callback)
		{
			animator.Play (name);
		}

		public static void AddAnimatorCallback (Animator animator, Action<int> callback)
		{
			AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo (0);

		}
	}
}
