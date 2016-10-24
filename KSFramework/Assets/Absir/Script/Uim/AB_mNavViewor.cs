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

		override protected void initComponent ()
		{
			base.initComponent ();
			listViewors = GameObjectUtils.getChildrenGameObjectComponentSort<AB_Viewor> (containerTrans.gameObject);
		
			int count = listViewors.Count;
			if (count > 0) {
				foreach (AB_Viewor viewor in listViewors) {
					GameObjectUtils.getOrAddComponent<AB_Retain> (viewor.gameObject).retain ();
					AB_UI.ME.removeView (viewor.gameObject.transform);
				}
			
				listViewors [count - 1].doAppearTransform (containerTrans);
				display ();
			}
		}

		public void display ()
		{
			if (naviationBar != null) {
				naviationBar.display (listViewors);
			}
		}

		public void pushViewor (AB_Viewor viewor)
		{
			int count = listViewors.Count;
			if (count > 0) {
				listViewors [count - 1].doDisappearTransform ();
			}
		
			GameObjectUtils.getOrAddComponent<AB_Retain> (viewor.gameObject).retain ();
			listViewors.Add (viewor);
			viewor.doAppearTransform (containerTrans);
			display ();
		}

		public void popViewor ()
		{
			int count = listViewors.Count;
			if (count > 1) {
				count = count - 1;
				AB_Viewor viewor = listViewors [count];
				GameObjectUtils.getOrAddComponent<AB_Retain> (viewor.gameObject).release ();
				viewor.doDisappearTransform ();
				listViewors.RemoveAt (count);
			
				listViewors [count - 1].doAppearTransform (containerTrans);
				display ();
			}
		}

		public void pushGameObject (GameObject go)
		{
			pushViewor (GameObjectUtils.getOrAddComponent<AB_Viewor> (go));
		}

		public void popVieworRoot ()
		{		
			int count = listViewors.Count;
			if (count > 1) {
				for (--count; count > 0; count--) {
					AB_Viewor viewor = listViewors [count];
					GameObjectUtils.getOrAddComponent<AB_Retain> (viewor.gameObject).release ();
					viewor.doDisappearTransform ();
				}
			
				listViewors.RemoveRange (1, listViewors.Count - 1);
			
				listViewors [0].doAppearTransform (containerTrans);
				display ();
			}
		}

		public override bool doAppear ()
		{
			if (base.doAppear ()) {
				if (listViewors != null) {
					listViewors [listViewors.Count - 1].doAppear ();
				}

				return true;
			}

			return false;
		}

		public override bool doDisappear ()
		{
			if (base.doDisappear ()) {
				if (listViewors != null) {
					listViewors [listViewors.Count - 1].doDisappear ();
				}
			
				return true;
			}
		
			return false;
		}

		public void doDisappearTransform ()
		{
			if (base.doDisappearTransform ()) {
				if (listViewors != null) {
					foreach (AB_Viewor viewor in listViewors) {
						GameObjectUtils.getOrAddComponent<AB_Retain> (viewor.gameObject).release ();
						viewor.doDisappearTransform ();
					}

					listViewors.Clear ();
				}
			}
		}
	}
}
