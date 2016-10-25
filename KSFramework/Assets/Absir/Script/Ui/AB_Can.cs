using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public abstract class CanComponent : AB_Bas
	{
		abstract public int GetComponentCount ();

		abstract public Component GetComponentAt (int index);

		abstract public Component GetActiveComponent ();

		abstract public bool SetActiveComponentIndex (int componentIndex);
	}

	public abstract class AB_Can<T> : CanComponent where T : Component
	{
		public GameObject targetChildren;

		protected List<T> componentSort;

		protected T activeComponent;

		override protected void InitComponent ()
		{
			if (targetChildren == null) {
				targetChildren = gameObject;
			}
		
			componentSort = GetComponentSort ();
			foreach (T component in componentSort) {
				InitComponent (component);
				if (IsComponentActive (component)) {
					if (activeComponent == null) {
						activeComponent = component;
						SetComponentActive (component, true);
					
					} else {
						SetComponentActive (component, false);
					}
				}
			}
		
			if (activeComponent == null && componentSort.Count > 0) {
				setActiveComponent (componentSort [0]);
			}
		}

		virtual public void AddCanComponent (T component)
		{
			AB_UI.ME.AddView (component.transform, targetChildren.transform);
			componentSort.Add (component);
			SetComponentActive (component, false);
		}

		virtual public T RemoveCanComponentIndex (int index)
		{
			if (index >= 0 && index < componentSort.Count) {
				T component = componentSort [index];
				if (component == activeComponent) {
					SetActiveComponentIndex (0);
				}

				componentSort.RemoveAt (index);
				AB_UI.ME.RemoveView (component.transform);
				return component;
			}

			return null;
		}

		virtual protected void InitComponent (T component)
		{
		}

		virtual protected List<T> GetComponentSort ()
		{
			return ComponentUtils.GetChildrenComponentSort<T> (targetChildren);
		}

		override public int GetComponentCount ()
		{
			return componentSort.Count;
		}

		override public Component GetComponentAt (int index)
		{
			int count = componentSort.Count;
			return index < 0 || index >= count ? null : componentSort [index];
		}

		override public Component GetActiveComponent ()
		{
			return activeComponent;
		}

		override public bool SetActiveComponentIndex (int componentIndex)
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
				SetComponentActive (activeComponent, false);
			}
		
			SetComponentActive (component, true);
			activeComponent = component;
			return true;
		}

		abstract protected bool IsComponentActive (T component);

		abstract protected void SetComponentActive (T component, bool active);
	}
}
