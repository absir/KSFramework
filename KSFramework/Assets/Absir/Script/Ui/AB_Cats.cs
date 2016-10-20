using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public interface ICats
	{
		void onTrigger (int componentIndex);
	}

	public class CatsTrigger : CatTriggerTarget<ICats>
	{
		public int componentIndex;

		public CatsTrigger (ICats cats, int componentIndex) : base (cats)
		{
			this.componentIndex = componentIndex;
		}

		override public void run ()
		{
			target.onTrigger (componentIndex);
		}
	}

	public abstract class AB_Cats<T> : AB_Can<T>, ICats where T : Component
	{
		override protected void initComponent ()
		{
			base.initComponent ();
			int count = componentSort.Count;
			for (int i = 0; i < count; i++) {
				T component = componentSort [i];
				addCatInvoker (component, i);
			}
		}

		protected void addCatInvoker (T component, int index)
		{
			AB_Cat cat = GameObjectUtils.getOrAddComponent<AB_Cat> (component.gameObject);
			cat.addCatTrigger (new CatsTrigger (this, index));
			bindCat (cat, component);
		}

		protected virtual void bindCat (AB_Cat cat, T component)
		{
		}

		protected void setCatInvoker (T component, int index)
		{
			AB_Cat cat = GameObjectUtils.getOrAddComponent<AB_Cat> (component.gameObject);
			CatsTrigger trigger = cat.getCatTrigger (this) as CatsTrigger;
			if (trigger == null) {
				cat.addCatTrigger (new CatsTrigger (this, index));
			
			} else {
				trigger.componentIndex = index;
			}
		}

		virtual public void onTrigger (int componentIndex)
		{
			setActiveComponentIndex (componentIndex);
		}

		override public void addCanComponent (T component)
		{
			base.addCanComponent (component);
			addCatInvoker (component, componentSort.Count - 1);
		}

		override public T removeCanComponentIndex (int index)
		{
			T component = base.removeCanComponentIndex (index);
			if (component != null) {
				int count = componentSort.Count;
				for (int i = index; i < count; i++) {
					setCatInvoker (componentSort [i], i);
				}
			}

			return component;
		}
	}
}
