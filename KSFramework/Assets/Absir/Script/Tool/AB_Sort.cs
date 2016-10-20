using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	#if UNITY_EDITOR
	public class AB_Sort : AB_Tool
	{
		public double behindZ = 0;

		public override IEnumerator doTrigger ()
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

			yield break;
		}
	}
	#endif
}

