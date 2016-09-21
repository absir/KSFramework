using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public class GameObjectUtils
	{
		public static void addGameObjectSort (List<GameObject> gameObjectSort, GameObject gameObject)
		{
			string name = gameObject.name;
			int count = gameObjectSort.Count;
			for (int i = 0; i < count; i++) {
				if (name.CompareTo (gameObjectSort [i].name) < 0) {
					gameObjectSort.Insert (i, gameObject);
					return;
				}
			}
		
			gameObjectSort.Add (gameObject);
		}

		public static List<GameObject> getGameObjectSort (List<GameObject> gameObjectList)
		{
			List<GameObject> gameObjectSort = new List<GameObject> (gameObjectList.Count);
			foreach (GameObject gameObject in gameObjectList) {
				addGameObjectSort (gameObjectSort, gameObject);
			}
			return gameObjectSort;
		}

		public static List<GameObject> getChildrenGameObjectSort (GameObject gameObject)
		{
			List<GameObject> gameObjectSort = new List<GameObject> ();
			foreach (Transform child in gameObject.transform) {
				addGameObjectSort (gameObjectSort, child.gameObject);
			}
		
			return gameObjectSort;
		}

		public static T getGameObjectComponent<T> (GameObject gameObject) where T : Component
		{
			return getGameObjectComponent<T, T> (gameObject);
		}

		public static T getGameObjectComponent<T, K> (GameObject gameObject) where T : Component where K : T
		{
			T component = gameObject.GetComponent<T> ();
			if (component == null) {
				component = gameObject.AddComponent<K> ();
			}

			return component;
		}

		public static List<T> getChildrenGameObjectComponentSort<T> (GameObject gameObject) where T : Component
		{
			return getChildrenGameObjectComponentSort<T, T> (gameObject);
		}

		public static List<T> getChildrenGameObjectComponentSort<T, K> (GameObject gameObject) where T : Component where K : T
		{
			List<T> componentSort = new List<T> ();
			foreach (Transform child in gameObject.transform) {
				ComponentUtils.addComponentSort (componentSort, getGameObjectComponent<T, K> (child.gameObject));
			}
		
			return componentSort;
		}

		public static GameObject fetchParentGameObject (GameObject gameObject, string name)
		{
			Transform transform = gameObject.transform;
			while (transform != null) {
				if (gameObject.name.Equals (name)) {
					return gameObject;
				}
			
				transform = transform.parent;
			}
		
			return null;
		}

		public static GameObject fetchChildrenGameObject (GameObject gameObject, string name)
		{
			if (gameObject.name.Equals (name)) {
				return gameObject;
			}
		
			foreach (Transform child in gameObject.transform) {
				gameObject = fetchChildrenGameObject (child.gameObject, name);
				if (gameObject != null) {
					return gameObject;
				}
			}
		
			return null;
		}
	}
}

