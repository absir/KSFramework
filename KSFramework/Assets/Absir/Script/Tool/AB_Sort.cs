using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	#if UNITY_EDITOR
	[ExecuteInEditMode]
	public class AB_Sort : MonoBehaviour
	{
		public double behindZ = 0;

		// Use this for initialization
		void Start ()
		{
			List<GameObject> gameObjectSort = GameObjectUtils.getChildrenGameObjectSort (gameObject);
			float behind = (float)behindZ;
			int lastIndex = gameObjectSort.Count - 1;
			for (; lastIndex >= 0; lastIndex--) {
				Transform trans = gameObjectSort [lastIndex].transform;
				Vector3 localPosition = trans.localPosition;
				localPosition.z = behind;
				trans.localPosition = localPosition;
				behind += AB_UI.BEHIND_STEP;
			}
		}
	}
	#endif
}

