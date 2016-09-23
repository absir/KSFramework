using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_CUI : MonoBehaviour
	{
		public string uiTemplateName;

		void Awake ()
		{
			if (!string.IsNullOrEmpty (uiTemplateName)) {
				OpenWindow (transform, uiTemplateName);
			}
		}

		public static void AddWindow (Transform window, Transform parent)
		{
			if (window && parent != null) {
				window.parent = parent;
			}
		}

		public static void OpenWindow (Transform parent, string uiTemplateName, params object[] args)
		{
			KEngine.UI.UIModule.Instance.OpenWindow (uiTemplateName, args);
			CallAddWindow (parent, false, uiTemplateName);
		}

		public static void OpenDynamicWindow (Transform parent, string uiTemplateName, string instanceName, params object[] args)
		{
			KEngine.UI.UIModule.Instance.OpenDynamicWindow (uiTemplateName, instanceName, args);
			CallAddWindow (parent, true, instanceName);
		}

		public static void OpenDialogWindow (string uiTemplateName, string name, params object[] args)
		{
			OpenDialogNameWindow (uiTemplateName, null, name, args);
		}

		public static void OpenDialogNameWindow (string uiTemplateName, string instanceName, string name, params object[] args)
		{
			if (string.IsNullOrEmpty (instanceName)) {
				instanceName = uiTemplateName;
			}
				
			KEngine.UI.UIModule.Instance.OpenDynamicWindow (uiTemplateName, instanceName, args);
			CallDialogWindow (instanceName, name);
		}

		public static void CallAddWindow (Transform parent, bool dynamicUI, string uiName)
		{
			if (parent != null) {
				System.Action<KEngine.UI.UIController, object[]> action = (controller, args) => {
					AddWindow (controller.transform, parent);
				};

				if (dynamicUI) {
					KEngine.UI.UIModule.Instance.CallDynamicUI (uiName, action);

				} else {
					KEngine.UI.UIModule.Instance.CallUI (uiName, action);
				}
			}
		}

		public static void CallDialogWindow (string uiName)
		{
			KEngine.UI.UIModule.Instance.CallDynamicUI (uiName, (controller, args) => {
				AB_UI.ME.openDialog (controller.gameObject);
			});
		}

		public static void CallDialogWindow (string uiName, string name)
		{
			KEngine.UI.UIModule.Instance.CallDynamicUI (uiName, (controller, args) => {
				AB_UI.ME.openDialogName (controller.gameObject, name);
			});
		}
	}
}

