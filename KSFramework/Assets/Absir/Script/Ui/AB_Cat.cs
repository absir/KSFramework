using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public interface CatTrigger
	{
		object GetTrigger ();

		void Run ();
	}

	public abstract class CatTriggerTarget<T> : CatTrigger
	{
		protected T target;

		public CatTriggerTarget (T target)
		{
			this.target = target;
		}

		public object GetTrigger ()
		{
			return target;
		}

		abstract public void Run ();
	}

	public class CatTriggerAction : CatTrigger
	{
		private Action action;

		public CatTriggerAction (Action action)
		{
			this.action = action;
		}

		public object GetTrigger ()
		{
			return action;
		}

		public void Run ()
		{
			action ();
		}
	}

	[SLua.CustomLuaClassAttribute]
	public class AB_Cat : MonoBehaviour
	{
		protected List<CatTrigger> catTriggers = new List<CatTrigger> (1);

		public void AddCatTrigger (CatTrigger catTrigger)
		{
			catTriggers.Add (catTrigger);
		}

		public CatTrigger GetCatTrigger (object trigger)
		{
			foreach (var catTrigger in catTriggers) {
				if (catTrigger.GetTrigger () == trigger) {
					return catTrigger;
				}
			}

			return null;
		}

		public void RemoveCatTrigger (object trigger)
		{
			int count = catTriggers.Count;
			for (int i = 0; i < count; i++) {
				if (catTriggers [i].GetTrigger () == trigger) {
					catTriggers.RemoveAt (i);
					break;
				}
			}
		}

		public void ClearCatTrigger (object trigger)
		{
			int count = catTriggers.Count;
			for (int i = 0; i < count; i++) {
				if (trigger == null || catTriggers [i].GetTrigger () == trigger) {
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
					catTrigger.Run ();
				
				} else {
					object t = catTrigger.GetTrigger ();
					if (t == null || t == trigger) {
						catTrigger.Run ();
					}
				}
			}
		}
	}
}
