using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public interface ICats
	{
		void OnTrigger (int componentIndex);
	}

	public class CatsTrigger : CatTriggerTarget<ICats>
	{
		public int componentIndex;

		public CatsTrigger (ICats cats, int componentIndex) : base (cats)
		{
			this.componentIndex = componentIndex;
		}

		override public void Run ()
		{
			target.OnTrigger (componentIndex);
		}
	}

	public abstract class AB_Cats<T> : AB_Can<T>, ICats where T : Component
	{
		override protected void InitComponent ()
		{
			base.InitComponent ();
			int count = componentSort.Count;
			for (int i = 0; i < count; i++) {
				T component = componentSort [i];
				AddCatInvoker (component, i);
			}
		}

		protected void AddCatInvoker (T component, int index)
		{
			AB_Cat cat = GameObjectUtils.GetOrAddComponent<AB_Cat> (component.gameObject);
			cat.AddCatTrigger (new CatsTrigger (this, index));
			BindCat (cat, component);
		}

		protected virtual void BindCat (AB_Cat cat, T component)
		{
		}

		protected void SetCatInvoker (T component, int index)
		{
			AB_Cat cat = GameObjectUtils.GetOrAddComponent<AB_Cat> (component.gameObject);
			CatsTrigger trigger = cat.GetCatTrigger (this) as CatsTrigger;
			if (trigger == null) {
				cat.AddCatTrigger (new CatsTrigger (this, index));
			
			} else {
				trigger.componentIndex = index;
			}
		}

		virtual public void OnTrigger (int componentIndex)
		{
			SetActiveComponentIndex (componentIndex);
		}

		override public void AddCanComponent (T component)
		{
			base.AddCanComponent (component);
			AddCatInvoker (component, componentSort.Count - 1);
		}

		override public T RemoveCanComponentIndex (int index)
		{
			T component = base.RemoveCanComponentIndex (index);
			if (component != null) {
				int count = componentSort.Count;
				for (int i = index; i < count; i++) {
					SetCatInvoker (componentSort [i], i);
				}
			}

			return component;
		}
	}
}
