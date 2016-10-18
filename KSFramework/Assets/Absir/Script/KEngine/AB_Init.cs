using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KEngine;
using KEngine.UI;
using KSFramework;

namespace Absir
{
	public class AB_Init : MonoBehaviour
	{
		public static bool inited { get; protected set; }

		public static bool started { get; protected set; }

		void Awake ()
		{
			Init (true);
			AB_Game.AddLogicStartActions (InitStart);
		}

		public static void Init (bool start)
		{
			if (!inited) {
				AB_Game game = AB_Resource.LoadModule<AB_Game> ("_AB_Game");
				if (game == null) {
					GameObject obj = new GameObject ();
					obj.name = "_AB_Game";
					game = obj.AddComponent<AB_Game> ();
				}

				DontDestroyOnLoad (game.gameObject);
				inited = true;
			}

			if (start && !started) {
				started = true;
				AB_Game.AddGameStartAction (() => {
					Transform startTransform = AB_Resource.LoadModule<Transform> ("_AB_Start");
					if (startTransform == null) {
						AB_Game.LogicComplete ("AB_Init");
					}
				});
			}
		}

		protected virtual void InitStart ()
		{
			
		}

	}
}
