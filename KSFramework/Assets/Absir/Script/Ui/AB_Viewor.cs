using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_Viewor : AB_Bas
	{
		public const string _ViewDidAppear = "ViewDidAppear";

		public const string _ViewDidDisappear = "ViewDidDisappear";

		public AB_Call call;

		//public bool appeared { get; private set; }

		private AB_Viewor parent;

		private AB_Viewor child;

		override protected void InitComponent ()
		{
			base.InitComponent ();
			if (call != null) {
				call = AB_Call.Find (gameObject);
			}
		}

		public virtual bool DoAppear ()
		{
//			if (appeared) {
//				return false;
//			}

			if (child == null) {
				//appeared = true;
				ViewDidAppear ();
				return true;
			}
				
			child.DoAppear ();
			return false;
		}

		public virtual bool DoDisappear ()
		{
//			if (!appeared) {
//				return false;
//			}

			if (child == null) {
				//appeared = false;
				ViewDidDisappear ();
				return true;
			}
		
			child.DoDisappear ();
			return false;
		}

		protected virtual void ViewDidAppear ()
		{
			AB_Call.DoCall (call, _ViewDidAppear);
		}

		protected virtual void ViewDidDisappear ()
		{
			AB_Call.DoCall (call, _ViewDidDisappear);
		}

		public AB_Viewor GetParent ()
		{
			return parent;
		}

		public void PresentViewor (AB_Viewor viewor)
		{
			child = viewor;
			viewor.parent = this;

			Transform container = gameObject.transform.parent;
			GameObjectUtils.GetOrAddComponent<AB_Retain> (gameObject).Retain ();

			DoDisappearTransform ();
			viewor.DoAppearTransform (container);
		}

		public void PresentGameObject (GameObject go)
		{
			PresentViewor (GameObjectUtils.GetOrAddComponent<AB_Viewor> (go));
		}

		public AB_Viewor DismissViewor ()
		{
			if (parent != null) {
				Transform container = gameObject.transform.parent;
				DoDisappearTransform ();

				GameObjectUtils.GetOrAddComponent<AB_Retain> (parent.gameObject).Release ();

				AB_Viewor viewor = parent;
				parent.child = null;
				parent = null;

				viewor.DoAppearTransform (container);
				return viewor;
			}

			return null;
		}

		public void DoAppearTransform (Transform containerTrans)
		{
			AB_UI.ME.AddViewAuto (transform, containerTrans, true);
			DoAppear ();
		}

		public virtual bool DoDisappearTransform ()
		{
			bool destory = AB_UI.ME.RemoveView (transform);
			DoDisappear ();
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

		public static void DoAppearGameObject (GameObject gameObject, Transform containerTrans)
		{
			GameObjectUtils.GetOrAddComponent<AB_Viewor> (gameObject).DoAppearTransform (containerTrans);
		}

		private static List<AB_Viewor> currentViewors = new List<AB_Viewor> ();

		public static int GetCurrentVieworCount ()
		{
			return currentViewors.Count;
		}

		public static AB_Viewor GetCurrentVieworAt (int index)
		{
			return index < 0 || index >= currentViewors.Count ? null : currentViewors [index];
		}

		public static AB_Viewor GetCurrentViewor ()
		{
			int count = currentViewors.Count;
			return count > 0 ? currentViewors [count - 1] : null;
		}

		public static T GetCurrentVieworOrder<T> (int order) where T : AB_Viewor
		{
			int count = currentViewors.Count;
			for (count--; count >= 0; count--) {
				AB_Viewor viewor = currentViewors [count];
				if (viewor is T && order-- <= 0) {
					return (T)viewor;
				}
			}

			return null;
		}

		public static AB_Viewor GetRootViewor ()
		{
			int count = currentViewors.Count;
			return count > 0 ? currentViewors [0] : null;
		}

		public static void ClearViewor (AB_Viewor viewor)
		{
			if (viewor != null) {
				ClearViewor (viewor.child);
				viewor.DoDisappearTransform ();
			}
		}

		public static void SetRootViewor (AB_Viewor viewor)
		{
			AB_Viewor rootViewor = GetRootViewor ();
			Transform rootContainer;
			if (rootViewor == null || rootViewor == viewor) {
				rootContainer = AB_Screen.ME.getContainer ();
			
			} else {
				rootContainer = rootViewor.transform.parent;
				ClearViewor (rootViewor);
			}

			viewor.DoAppearTransform (rootContainer);
		}

		public static void SetRootGameObject (GameObject go)
		{
			SetRootViewor (GameObjectUtils.GetOrAddComponent<AB_Viewor> (go));
		}
	}

}
