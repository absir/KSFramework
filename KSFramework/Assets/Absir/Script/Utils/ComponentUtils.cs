using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public class ComponentUtils
	{
		public static void AddComponentSort<T> (List<T> componentSort, T component) where T : Component
		{
			string name = component.gameObject.name;
			int count = componentSort.Count;
			for (int i = 0; i < count; i++) {
				if (name.CompareTo (componentSort [i].gameObject.name) < 0) {
					componentSort.Insert (i, component);
					return;
				}
			}
		
			componentSort.Add (component);
		}

		public static List<T> GetComponentSort<T> (List<T> componentList) where T : Component
		{
			List<T> componentSort = new List<T> (componentList.Count);
			foreach (T component in componentList) {
				AddComponentSort (componentSort, component);
			}
		
			return componentSort;
		}

		public static List<T> GetChildrenComponentList<T> (GameObject gameObject) where T : Component
		{
			List<T> componentList = new List<T> ();
			Transform transform = gameObject.transform;
			foreach (Transform child in transform) {
				T component = child.gameObject.GetComponent<T> ();
				if (component != null) {
					componentList.Add (component);
				}
			}
		
			return componentList;
		}

		public static T GetComponentObject<T> (GameObject gameObject) where T : Object
		{
			foreach (Component component in gameObject.GetComponents<Component>()) {
				if (component is T) {
					return (T)(Object)component;
				}
			}
		
			return null;
		}

		public static List<T> GetComponentObjects<T> (GameObject gameObject) where T : Object
		{
			List<T> componentList = new List<T> ();
			foreach (Component component in gameObject.GetComponents<Component>()) {
				if (component is T) {
					componentList.Add ((T)(Object)component);
				}
			}
		
			return componentList;
		}

		public static List<T> GetComponentObjectSort<T> (GameObject gameObject) where T : Component
		{
			List<T> componentSort = new List<T> ();
			foreach (Component component in gameObject.GetComponents<Component>()) {
				if (component is T) {
					AddComponentSort<T> (componentSort, (T)component);
				}
			}
		
			return componentSort;
		}

		public static List<T> GetChildrenComponentSort<T> (GameObject gameObject) where T : Component
		{
			List<T> componentSort = new List<T> ();
			foreach (Transform child in gameObject.transform) {
				T component = child.gameObject.GetComponent<T> ();
				if (component != null) {
					AddComponentSort (componentSort, component);
				}
			}
		
			return componentSort;
		}

		public static T FetchParentComponent<T> (GameObject gameObject) where T : Component
		{
			T component;
			while ((component = gameObject.GetComponent<T> ()) == null) {
				Transform transform = gameObject.transform.parent;
				if (transform == null) {
					break;
				}
			
				gameObject = transform.gameObject;
			}
		
			return component;
		}

		public static T FetchChildrenComponent<T> (GameObject gameObject) where T : Component
		{
			return FetchChildrenComponent<T> (gameObject, 0);
		}

		public static T FetchChildrenComponent<T> (GameObject gameObject, int componentIndex) where T : Component
		{
			foreach (Transform child in gameObject.transform) {
				T component = child.gameObject.GetComponent<T> ();
				if (component != null && componentIndex-- <= 0) {
					return component;
				}
			}
		
			return null;
		}

		public static T FetchAllChildrenComponent<T> (GameObject gameObject) where T : Component
		{
			T component = gameObject.GetComponent<T> ();
			if (component != null) {
				return component;
			}
		
			foreach (Transform child in gameObject.transform) {
				component = FetchAllChildrenComponent<T> (child.gameObject);
				if (component != null) {
					return component;
				} 
			}
		
			return null;
		}

		public static T fetchCurrentComponent<T> (GameObject gameObject) where T : Component
		{
			Transform transform = gameObject.transform;
			while (transform != null) {
				gameObject = transform.gameObject;
				T component = gameObject.GetComponent<T> ();
				if (component != null) {
					return component;
				}
			
				transform = transform.parent;
			}
		
			return FetchAllChildrenComponent<T> (gameObject);
		}
	}
}
