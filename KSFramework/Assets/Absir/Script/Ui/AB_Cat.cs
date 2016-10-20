using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public interface CatTrigger
	{
		object getTrigger ();

		void run ();
	}

	public abstract class CatTriggerTarget<T> : CatTrigger
	{
		protected T target;

		public CatTriggerTarget (T target)
		{
			this.target = target;
		}

		public object getTrigger ()
		{
			return target;
		}

		abstract public void run ();
	}

	public class CatTriggerAction : CatTrigger
	{
		private Action action;

		public CatTriggerAction (Action action)
		{
			this.action = action;
		}

		public object getTrigger ()
		{
			return action;
		}

		public void run ()
		{
			action ();
		}
	}

	[SLua.CustomLuaClassAttribute]
	public class AB_Cat : MonoBehaviour
	{
		protected List<CatTrigger> catTriggers = new List<CatTrigger> (1);

		public void addCatTrigger (CatTrigger catTrigger)
		{
			catTriggers.Add (catTrigger);
		}

		public CatTrigger getCatTrigger (object trigger)
		{
			foreach (var catTrigger in catTriggers) {
				if (catTrigger.getTrigger () == trigger) {
					return catTrigger;
				}
			}

			return null;
		}

		public void removeCatTrigger (object trigger)
		{
			int count = catTriggers.Count;
			for (int i = 0; i < count; i++) {
				if (catTriggers [i].getTrigger () == trigger) {
					catTriggers.RemoveAt (i);
					break;
				}
			}
		}

		public void clearCatTrigger (object trigger)
		{
			int count = catTriggers.Count;
			for (int i = 0; i < count; i++) {
				if (trigger == null || catTriggers [i].getTrigger () == trigger) {
					catTriggers.RemoveAt (i);
					i--;
					count--;
				}
			}
		}

		public void OnClick ()
		{
			OnTrigger (null);	
		}

		public void OnTrigger (object trigger)
		{
			foreach (CatTrigger catTrigger in catTriggers) {
				if (trigger == null) {
					catTrigger.run ();
				
				} else {
					object t = catTrigger.getTrigger ();
					if (t == null || t == trigger) {
						catTrigger.run ();
					}
				}
			}
		}
	}
}
