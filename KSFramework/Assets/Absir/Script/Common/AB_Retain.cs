using UnityEngine;
using System.Collections;

namespace Absir
{
	public class AB_Retain : MonoBehaviour
	{
		private int retainCount;

		public int RetainCount {
			get {
				return retainCount;
			}
		}

		public void Retain ()
		{
			++retainCount;
		}

		public void Release ()
		{
			--retainCount;

		}

		public void ReleaseCleanUp ()
		{
			if (--retainCount <= 0) {
				Destroy (gameObject);
			}
		}
	}
}
