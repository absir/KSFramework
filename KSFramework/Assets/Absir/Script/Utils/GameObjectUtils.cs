using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Absir
{
	public class GameObjectUtils
	{
		public static bool S_UNITY_HAS_CHILDREN = false;

		public static T GetOrAddComponent<T> (GameObject gameObject) where T : Component
		{
			return GetOrAddComponent<T, T> (gameObject);
		}

		public static T GetOrAddComponent<T, K> (GameObject gameObject) where T : Component where K : T
		{
			T component = gameObject.GetComponent<T> ();
			if (component == null) {
				component = gameObject.AddComponent<K> ();
			}

			return component;
		}

		public static void AddGameObjectSort (List<GameObject> gameObjectSort, GameObject gameObject)
		{
			if (S_UNITY_HAS_CHILDREN) {
				string name = gameObject.name;
				int count = gameObjectSort.Count;
				for (int i = 0; i < count; i++) {
					if (name.CompareTo (gameObjectSort [i].name) < 0) {
						gameObjectSort.Insert (i, gameObject);
						return;
					}
				}
			}
		
			gameObjectSort.Add (gameObject);
		}

		public static List<GameObject> GetGameObjectSort (List<GameObject> gameObjectList)
		{
			if (S_UNITY_HAS_CHILDREN) {
				List<GameObject> gameObjectSort = new List<GameObject> (gameObjectList.Count);
				foreach (GameObject gameObject in gameObjectList) {
					AddGameObjectSort (gameObjectSort, gameObject);
				}
				return gameObjectSort;

			} else {
				return gameObjectList;
			}
		}

		public static List<GameObject> GetChildrenGameObjectSort (GameObject gameObject)
		{
			List<GameObject> gameObjectSort = new List<GameObject> ();
			foreach (Transform child in gameObject.transform) {
				AddGameObjectSort (gameObjectSort, child.gameObject);
			}
		
			return gameObjectSort;
		}

		public static List<T> GetChildrenGameObjectComponentSort<T> (GameObject gameObject) where T : Component
		{
			return GetChildrenGameObjectComponentSort<T, T> (gameObject);
		}

		public static List<T> GetChildrenGameObjectComponentSort<T, K> (GameObject gameObject) where T : Component where K : T
		{
			List<T> componentSort = new List<T> ();
			foreach (Transform child in gameObject.transform) {
				ComponentUtils.AddComponentSort (componentSort, GetOrAddComponent<T, K> (child.gameObject));
			}
		
			return componentSort;
		}

		public static GameObject FetchParentGameObject (GameObject gameObject, string name)
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

		public static GameObject FetchChildrenGameObject (GameObject gameObject, string name)
		{
			if (gameObject.name.Equals (name)) {
				return gameObject;
			}
		
			foreach (Transform child in gameObject.transform) {
				gameObject = FetchChildrenGameObject (child.gameObject, name);
				if (gameObject != null) {
					return gameObject;
				}
			}
		
			return null;
		}
	}
}

