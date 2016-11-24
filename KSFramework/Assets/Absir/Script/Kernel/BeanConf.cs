using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Absir
{
	public class BeanConf
	{
		private bool _inited;

		public BeanConf ()
		{
	
		}

		public void InitConf ()
		{
			if (!_inited) {
				_inited = true;
				SetConf (this, GetConfStream ());
			}
		}

		protected virtual System.IO.Stream GetConfStream ()
		{
			string nameConf = Application.streamingAssetsPath + "/" + GetType ().FullName + ".txt";
			//Debug.Log ("finding nameConf = " + nameConf);
			if (!System.IO.File.Exists (nameConf)) {
				nameConf = Application.streamingAssetsPath + "/" + GetType ().Name + ".txt";
				//Debug.Log ("finding nameConf = " + nameConf);
				if (!System.IO.File.Exists (nameConf)) {
					return null;
				}
			}
				
			return System.IO.File.OpenRead (nameConf);
		}

		public static void SetConf (object bean, System.IO.Stream confStream)
		{
			if (confStream != null) {
				IDictionary<string, object> confMap = new Dictionary<string, object> ();
				BeanConfigImpl.readProperties (ABConfig.CONFIG, confMap, confStream, null);
				foreach (var field in bean.GetType().GetFields(BindingFlags.Default  |  BindingFlags.Public | BindingFlags.NonPublic| BindingFlags.Instance | BindingFlags.CreateInstance)) {
					if (field.Name [0] != '_') {
						object value = BeanConfigImpl.getMapObject (confMap, field.Name, field.FieldType);
						if (value != null) {
							field.SetValue (bean, value);
						}
					}
				}
			}
		}

	}

	public class BeanConfRes : BeanConf
	{
		protected override System.IO.Stream GetConfStream ()
		{
			string nameConf = GetType ().FullName;
			Debug.Log ("finding nameConf = " + nameConf);
			TextAsset text = Resources.Load<TextAsset> (nameConf);
			if (text == null) {
				nameConf = GetType ().Name;
				Debug.Log ("finding nameConf = " + nameConf);
				text = Resources.Load<TextAsset> (nameConf);
				if (text == null) {
					return null;
				}
			}

			return new System.IO.MemoryStream (text.bytes);
		}
	}
}
