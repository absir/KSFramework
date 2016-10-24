using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public class AB_mNavViewor : AB_mContainer
	{
		public AB_Nav naviationBar;

		protected List<AB_Viewor> listViewors;

		override protected void InitComponent ()
		{
			base.InitComponent ();
			listViewors = GameObjectUtils.GetChildrenGameObjectComponentSort<AB_Viewor> (containerTrans.gameObject);
		
			int count = listViewors.Count;
			if (count > 0) {
				foreach (AB_Viewor viewor in listViewors) {
					GameObjectUtils.GetOrAddComponent<AB_Retain> (viewor.gameObject).Retain ();
					AB_UI.ME.RemoveView (viewor.gameObject.transform);
				}
			
				listViewors [count - 1].DoAppearTransform (containerTrans);
				Display ();
			}
		}

		public void Display ()
		{
			if (naviationBar != null) {
				naviationBar.Display (listViewors);
			}
		}

		public void PushViewor (AB_Viewor viewor)
		{
			int count = listViewors.Count;
			if (count > 0) {
				listViewors [count - 1].DoDisappearTransform ();
			}
		
			GameObjectUtils.GetOrAddComponent<AB_Retain> (viewor.gameObject).Retain ();
			listViewors.Add (viewor);
			viewor.DoAppearTransform (containerTrans);
			Display ();
		}

		public void PopViewor ()
		{
			int count = listViewors.Count;
			if (count > 1) {
				count = count - 1;
				AB_Viewor viewor = listViewors [count];
				GameObjectUtils.GetOrAddComponent<AB_Retain> (viewor.gameObject).Release ();
				viewor.DoDisappearTransform ();
				listViewors.RemoveAt (count);
			
				listViewors [count - 1].DoAppearTransform (containerTrans);
				Display ();
			}
		}

		public void PushGameObject (GameObject go)
		{
			PushViewor (GameObjectUtils.GetOrAddComponent<AB_Viewor> (go));
		}

		public void PopVieworRoot ()
		{		
			int count = listViewors.Count;
			if (count > 1) {
				for (--count; count > 0; count--) {
					AB_Viewor viewor = listViewors [count];
					GameObjectUtils.GetOrAddComponent<AB_Retain> (viewor.gameObject).Release ();
					viewor.DoDisappearTransform ();
				}
			
				listViewors.RemoveRange (1, listViewors.Count - 1);
			
				listViewors [0].DoAppearTransform (containerTrans);
				Display ();
			}
		}

		public override bool DoAppear ()
		{
			if (base.DoAppear ()) {
				if (listViewors != null) {
					listViewors [listViewors.Count - 1].DoAppear ();
				}

				return true;
			}

			return false;
		}

		public override bool DoDisappear ()
		{
			if (base.DoDisappear ()) {
				if (listViewors != null) {
					listViewors [listViewors.Count - 1].DoDisappear ();
				}
			
				return true;
			}
		
			return false;
		}

		override public bool DoDisappearTransform ()
		{
			if (base.DoDisappearTransform ()) {
				if (listViewors != null) {
					foreach (AB_Viewor viewor in listViewors) {
						GameObjectUtils.GetOrAddComponent<AB_Retain> (viewor.gameObject).Release ();
						viewor.DoDisappearTransform ();
					}

					listViewors.Clear ();
				}

				return true;
			}

			return false;
		}
	}
}
