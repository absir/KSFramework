using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Absir
{
	
	[CustomEditor (typeof(AB_Screen), true)] 
	public class AB_ScreenEditor : Editor
	{
		[HideInInspector]
		private bool calcing;

		public override void OnInspectorGUI ()
		{
			AB_Screen screen = (AB_Screen)target;
			if (EditorGUILayout.Toggle ("Screen Calc", calcing) && !calcing) {
				screen.CalcScreen ();
			}

			base.OnInspectorGUI ();
		}
	}
}
