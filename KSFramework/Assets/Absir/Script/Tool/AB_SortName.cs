using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	#if UNITY_EDITOR
	public class AB_SortName : AB_Tool
	{
		public string sortName;

		public int sortLength = 2;

		public override IEnumerator doTrigger ()
		{
			if (sortName != null) {
				if (sortName.Trim ().Length == 0) {
					sortName = null;
				}
			}

			string sortFormate = "{0:D" + sortLength.ToString () + "}";
			Dictionary<string, int> nameSorts = new Dictionary<string, int> ();
			Dictionary<string, GameObject> nameGameObjects = new Dictionary<string, GameObject> ();
			int sortIndex = 0;
			foreach (GameObject go in GameObjectUtils.getChildrenGameObjectSort(gameObject)) {
				string goname = go.name;
				if (sortName == null) {
					if (nameSorts.ContainsKey (goname)) {
						int index = nameSorts [goname];
						if (index == 0) {
							index = 1;
							nameGameObjects [goname].name = goname + string.Format (sortFormate, 0);
							nameGameObjects.Remove (goname);
						}
					
						go.name = goname + string.Format (sortFormate, index);
						nameSorts.Add (goname, ++index);
					
					} else {
						nameSorts.Add (goname, 0);
						nameGameObjects.Add (goname, go);
					}
				
				} else {
					go.name = sortName + string.Format (sortFormate, sortIndex++);
				}
			}


			yield break;
		}
	}
	#endif
}
