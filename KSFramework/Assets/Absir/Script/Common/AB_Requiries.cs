using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public class AB_Requiries : MonoBehaviour
	{
		public string shareTag;

		public string[] requiries;

		private List<AB_Share> shareRequiries;

		protected void AddShare (AB_Share share)
		{
			share.Retain ();
			shareRequiries.Add (share);
			share.SetShareTag (shareTag);
			AB_UI.ME.AddView (share.transform, transform.parent);
		}

		void Start ()
		{
			shareRequiries = new List<AB_Share> ();
			if (requiries != null && requiries.Length > 0) {
				foreach (string req in requiries) {
					if (!string.IsNullOrEmpty (req)) {
						string name = req;
						AB_Share share = AB_Share.GetShare (name);
						if (share == null) {
							string path = "prefab/" + name;
							Brige.Load (path, false, false, (obj) => {
								if (obj != null) {
									GameObject go = Instantiate (obj) as GameObject;
									if (go != null) {
										share = GameObjectUtils.GetOrAddComponent<AB_Share> (go);
										AB_Share.AddShare (share, name);
										AddShare (share);
									}
								}
							});

						} else {
							AddShare (share);
						}
					}
				}
			}
		}

		void OnDestory ()
		{
			if (shareRequiries != null) {
				AB_Context.ME.AddAction (() => {
					List<AB_Share> shares = shareRequiries;
					foreach (AB_Share share in shares) {
						share.ReleaseCleanUp ();
					}
				});

				shareRequiries = null;
			}
		}

	}
}
