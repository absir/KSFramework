using UnityEngine;
using System.Collections;

namespace Absir
{
	public class Editor_Brige
	{
		private static IE_Brige _ME;

		static Editor_Brige() {
			try {
				System.Type type = System.Type.GetType ("Absir.AB_Brige");
				_ME = (IE_Brige)type.Assembly.CreateInstance (type.FullName);

			} catch(System.Exception) {
			}
		}

		public static IE_Brige ME {
			get {
				return _ME;
			}
		}
	}

	public interface IE_Brige
	{
		void ReloadLua();

		void BeforeReloadUI();

		void AfterReloadUI();

		void BeforeReloadUILua();

		void AfterReloadUILua();
	}

}
