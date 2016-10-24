using UnityEngine;
using System.Collections;

namespace Absir
{
	public class AB_Retain : MonoBehaviour
	{
		private int _retainCount;

		public int retainCount {
			get {
				return _retainCount;
			}
		}

		public void retain ()
		{
			++_retainCount;
		}

		public void release ()
		{
			--_retainCount;

		}

		public void releaseCleanUp ()
		{
			if (--_retainCount <= 0) {
				Destroy (gameObject);
			}
		}
	}
}
