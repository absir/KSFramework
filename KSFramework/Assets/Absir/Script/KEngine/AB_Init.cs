using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KEngine;
using KEngine.UI;
using KSFramework;

namespace Absir
{
	
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_Init : MonoBehaviour
	{
		public static bool inited { get; protected set; }

		public static bool started { get; protected set; }

		public static Action startAction;

		void Awake ()
		{
			Init (true);
		}

		public static void Init (bool start)
		{
			if (!inited) {
				KSGame game = AB_Resource.LoadModule<KSGame> ("_AB_Game");
				if (game == null) {
					GameObject obj = new GameObject ();
					obj.name = "_AB_Game";
					game = obj.AddComponent<AB_Game> ();
				}

				DontDestroyOnLoad (game.gameObject);
				string name = KResourceModule.Instance.name;
				Debug.Log (name);
				inited = true;
			}

			if (start && !started) {
				Transform startTransform = AB_Resource.LoadModule<Transform> ("_AB_Start");
				if (startTransform == null) {
					DoStart ();
				}
			}
		}

		public static void DoStart ()
		{
			if (startAction != null) {
				startAction ();
				startAction = null;
			}

			started = true;
		}

	}
}
