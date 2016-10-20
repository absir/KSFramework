using UnityEngine;
using System.Collections;

public class AB_Grid : MonoBehaviour
{
	public Vector2 rectSize;

	public bool autoCellSize;

	public Vector2 cellSize;

	public Vector2 cellDirection;

	public bool horizontalCenter;

	public bool verticalCenter;

	public void Refresh ()
	{
		if (transform.GetType () == typeof(RectTransform)) {
			Vector2 size = ((RectTransform)transform).sizeDelta;
			if (rectSize.x == 0) {
				rectSize.x = size.x;
			}

			if (rectSize.y == 0) {
				rectSize.y = size.y;
			}
		}

		if (autoCellSize && transform.childCount > 0) {
			autoCellSize = false;
//			RectTransform rect = transform [0] as RectTransform;
//			if (rect != null) {
//				Vector2 size = rect.sizeDelta;
//				if (cellSize.x == 0) {
//					cellSize.x = size.x;
//				}
//
//				if (cellSize.y == 0) {
//					cellSize.y = size.y;
//				}
//			}


		}


	}
}
