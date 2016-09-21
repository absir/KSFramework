using UnityEngine;
using System.Collections;

namespace Absir
{
	public class TransformUtils : MonoBehaviour
	{
		public static string getVector3String (Vector3 vector)
		{
			return "{" + vector.x + "," + vector.y + "," + vector.z + "}";
		}

		public static Vector3 getLocalPostionParent (Transform transform, int parent)
		{
			Vector3 localPosition = transform.localPosition;
			while (parent-- > 0 && (transform = transform.parent) != null) {
				localPosition += transform.localPosition;
			}
		
			return localPosition;
		}

		public static Vector3 getLocalPostionParent (Transform transform, Transform parent)
		{
			Vector3 localPosition = transform.localPosition;
			while ((transform = transform.parent) != null && transform != parent) {
				localPosition += transform.localPosition;
			}
		
			return localPosition;
		}
	}
}
