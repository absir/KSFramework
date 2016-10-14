using UnityEngine;
using System.Collections;

namespace Absir
{
	public class Editor_Brige
	{
		private static IE_Brige _ME;

		static Editor_Brige() {
			
		}

		public static IE_Brige ME {
			get {
				if (_ME == null) {
					try {
						System.Type type = System.Type.GetType ("Absir.AB_Brige");
						if(type == null) {
							var assembly = System.Reflection.Assembly.Load ("Assembly-CSharp-Editor");
							type = assembly.GetType("Absir.AB_Brige");
						}

						if(type != null) {
							_ME = (IE_Brige)type.Assembly.CreateInstance (type.FullName);
						}

					} catch(System.Exception) {
					}
				}

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
