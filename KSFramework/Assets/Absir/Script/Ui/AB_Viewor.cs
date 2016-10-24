using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_Viewor : AB_Bas
	{
		public MonoBehaviour holder;

		//public bool appeared { get; private set; }

		private AB_Viewor parent;

		private AB_Viewor child;

		public virtual bool doAppear ()
		{
//			if (appeared) {
//				return false;
//			}

			if (child == null) {
				//appeared = true;
				viewDidAppear ();
				return true;
			}
				
			child.doAppear ();
			return false;
		}

		public virtual bool doDisappear ()
		{
//			if (!appeared) {
//				return false;
//			}

			if (child == null) {
				//appeared = false;
				viewDidDisappear ();
				return true;
			}
		
			child.doDisappear ();
			return false;
		}

		protected virtual void viewDidAppear ()
		{
		
		}

		protected virtual void viewDidDisappear ()
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
			GameObjectUtils.getOrAddComponent<AB_Retain> (gameObject).retain ();

			doDisappearTransform ();
			viewor.doAppearTransform (container);
		}

		public void presentGameObject (GameObject go)
		{
			presentViewor (GameObjectUtils.getOrAddComponent<AB_Viewor> (go));
		}

		public AB_Viewor dismissViewor ()
		{
			if (parent != null) {
				Transform container = gameObject.transform.parent;
				doDisappearTransform ();

				GameObjectUtils.getOrAddComponent<AB_Retain> (parent.gameObject).release ();

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

		public bool doDisappearTransform ()
		{
			bool destory = AB_UI.ME.removeView (transform);
			doDisappear ();
			return destory;
		}

		void OnEnable ()
		{
			currentViewors.Add (this);
		}

		void OnDisable ()
		{
			int count = currentViewors.Count;
			for (count--; count >= 0; count--) {
				if (currentViewors [count] == this) {
					currentViewors.RemoveAt (count);
					break;
				}
			}
		}

		public static void doAppearGameObject (GameObject gameObject, Transform containerTrans)
		{
			GameObjectUtils.getOrAddComponent<AB_Viewor> (gameObject).doAppearTransform (containerTrans);
		}

		private static List<AB_Viewor> currentViewors = new List<AB_Viewor> ();

		public static int getCurrentVieworCount ()
		{
			return currentViewors.Count;
		}

		public static AB_Viewor getCurrentViewor (int index)
		{
			return index < 0 || index >= currentViewors.Count ? null : currentViewors [index];
		}

		public static AB_Viewor getCurrentViewor ()
		{
			int count = currentViewors.Count;
			return count > 0 ? currentViewors [count - 1] : null;
		}

		public static T getCurrentVieworOrder<T> (int order) where T : AB_Viewor
		{
			int count = currentViewors.Count;
			for (count--; count >= 0; count--) {
				AB_Viewor viewor = currentViewors [count];
				if (viewor is Time && order-- <= 0) {
					return (T)viewor;
				}
			}

			return null;
		}

		public static AB_Viewor getRootViewor ()
		{
			int count = currentViewors.Count;
			return count > 0 ? currentViewors [0] : null;
		}

		public static void clearViewor (AB_Viewor viewor)
		{
			if (viewor != null) {
				clearViewor (viewor.child);
				viewor.doDisappearTransform ();
			}
		}

		public static void setRootViewor (AB_Viewor viewor)
		{
			AB_Viewor rootViewor = getRootViewor ();
			Transform rootContainer;
			if (rootViewor == null) {
				rootContainer = AB_Screen.ME.getContainer ();
				clearViewor (rootViewor);
			
			} else {
				rootContainer = rootViewor.parent;
			}

			viewor.doAppearTransform (rootContainer);
		}

		public static void setRootGameObject (GameObject go)
		{
			setRootViewor (GameObjectUtils.getOrAddComponent<AB_Viewor> (go));
		}
	}
}
