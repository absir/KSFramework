using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public abstract class CanComponent : AB_Bas
	{
		abstract public Component getActiveComponent ();

		abstract public bool setActiveComponentIndex (int componentIndex);
	}

	public abstract class AB_Can<T> : CanComponent where T : Component
	{
		public GameObject targetChildren;

		protected List<T> componentSort;

		protected T activeComponent;

		override protected void initComponent ()
		{
			if (targetChildren == null) {
				targetChildren = gameObject;
			}
		
			componentSort = getComponentSort ();
			foreach (T component in componentSort) {
				initComponent (component);
				if (isComponentActive (component)) {
					if (activeComponent == null) {
						activeComponent = component;
					
					} else {
						setComponentActive (component, false);
					}
				}
			}
		
			if (activeComponent == null && componentSort.Count > 0) {
				setActiveComponent (componentSort [0]);
			}
		}

		virtual public void addCanComponent (T component)
		{
			AB_UI.ME.addView (component.transform, targetChildren.transform);
			componentSort.Add (component);
			setComponentActive (component, false);
		}

		virtual public T removeCanComponentIndex (int index)
		{
			if (index >= 0 && index < componentSort.Count) {
				T component = componentSort [index];
				if (component == activeComponent) {
					setActiveComponentIndex (0);
				}

				componentSort.RemoveAt (index);
				AB_UI.ME.removeView (component.transform);
				return component;
			}

			return null;
		}

		virtual protected void initComponent (T component)
		{
		}

		virtual protected List<T> getComponentSort ()
		{
			return ComponentUtils.getChildrenComponentSort<T> (targetChildren);
		}

		override public Component getActiveComponent ()
		{
			return activeComponent;
		}

		override public bool setActiveComponentIndex (int componentIndex)
		{
			if (componentSort == null || componentIndex < 0 || componentIndex >= componentSort.Count) {
				return false;
			}
		
			return setActiveComponent (componentSort [componentIndex]);
		}

		virtual public bool setActiveComponent (T component)
		{
			if (component == activeComponent) {
				return false;
			}
		
			if (activeComponent != null) {
				setComponentActive (activeComponent, false);
			}
		
			setComponentActive (component, true);
			activeComponent = component;
			return true;
		}

		abstract protected bool isComponentActive (T component);

		abstract protected void setComponentActive (T component, bool active);
	}
}
