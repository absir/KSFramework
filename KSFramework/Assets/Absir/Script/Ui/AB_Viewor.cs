using UnityEngine;
using System.Collections;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	public class AB_Viewor : AB_Bas
	{
		public MonoBehaviour holder;

		private AB_Viewor parent;

		private AB_Viewor child;

		public virtual bool doAppear ()
		{
			if (child == null) {
				viewDidAppear ();
				return true;
			}

			child.doAppear ();
			return false;
		}

		public virtual bool doDisappear ()
		{
			if (child == null) {
				viewDidDisappear ();
				return true;
			}
		
			child.doDisappear ();
			return false;
		}

		private void viewDidAppear ()
		{
		
		}

		private void viewDidDisappear ()
		{
		
		}

		public AB_Viewor getParent ()
		{
			return parent;
		}

		public void presentViewor (AB_Viewor viewor)
		{
			child = viewor;
			viewor.parent = this;

			Transform container = gameObject.transform.parent;
			GameObjectUtils.getGameObjectComponent<AB_Retain> (gameObject).retain ();

			doDisappearTransform ();
			viewor.doAppearTransform (container);
		}

		public void presentGameObject (GameObject go)
		{
			presentViewor (GameObjectUtils.getGameObjectComponent<AB_Viewor> (go));
		}

		public AB_Viewor dismissViewor ()
		{
			if (parent != null) {
				Transform container = gameObject.transform.parent;
				doDisappearTransform ();

				GameObjectUtils.getGameObjectComponent<AB_Retain> (parent.gameObject).release ();

				AB_Viewor viewor = parent;
				parent.child = null;
				parent = null;

				viewor.doAppearTransform (container);
				return viewor;
			}

			return null;
		}

		public void doAppearTransform (Transform containerTrans)
		{
			AB_UI.ME.addViewAuto (transform, containerTrans, true);
			doAppear ();
		}

		public void doDisappearTransform ()
		{
			AB_UI.ME.removeView (transform);
			doDisappear ();
		}

		public static void doAppearGameObject (GameObject gameObject, Transform containerTrans)
		{
			GameObjectUtils.getGameObjectComponent<AB_Viewor> (gameObject).doAppearTransform (containerTrans);
		}
	}
}
