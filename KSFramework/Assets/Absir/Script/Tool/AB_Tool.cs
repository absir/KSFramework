using UnityEngine;
using System.Collections;

namespace Absir
{
	#if UNITY_EDITOR
	[ExecuteInEditMode]
	public abstract class AB_Tool : MonoBehaviour
	{
		[HideInInspector]
		public bool Intecepted;

		public abstract IEnumerator doTrigger ();

	}
	#endif
}
