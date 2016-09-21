#region Copyright (c) 2015 KEngine / Kelly <http://github.com/mr-kelly>, All rights reserved.

// KEngine - Toolset and framework for Unity3D
// ===================================
// 
// Filename: AppEngineInspector.cs
// Date:     2015/12/03
// Author:  Kelly
// Email: 23110388@qq.com
// Github: https://github.com/mr-kelly/KEngine
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library.

#endregion
//using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AppSettings;
using KEngine;
using KEngine.UI;
using KSFramework;

using Absir;

public class Game : KSGame
{
	/// <summary>
	/// Add Your Custom Initable(Coroutine) Modules Here...
	/// </summary>
	/// <returns></returns>
	protected override IList<IModuleInitable> CreateModules ()
	{
		var modules = base.CreateModules ();

		// TIP: Add Your Custom Module here
		//modules.Add(new Module());

		return modules;
	}

	/// <summary>
	/// Before Init Modules, coroutine
	/// </summary>
	/// <returns></returns>
	public override IEnumerator OnBeforeInit ()
	{
		// Do Nothing
		yield break;
	}

	/// <summary>
	/// After Init Modules, coroutine
	/// </summary>
	/// <returns></returns>
	public override IEnumerator OnGameStart ()
	{

		// Print AppConfigs
		Log.Info ("======================================= Read Settings from C# =================================");
		foreach (GameConfigSetting setting in GameConfigSettings.GetAll()) {
			Debug.Log (string.Format ("C# Read Setting, Key: {0}, Value: {1}", setting.Id, setting.Value));
		}

		yield return null;

		Log.Info ("======================================= Open Window 'Login' =================================");

		//UIModule.Instance.OpenWindow ("Login", 888);

		UIModule.Instance.OpenWindow ("Demo", 888);

//		UIModule.Instance.CallUI("Demo", (controller, args)=>{
//			GameObject image = controller.FindGameObject("Image");
//			Animator animator = image.GetComponent<Animator>();
//			float startTime = Time.time;
//			AB_Animator.PayAnimatorName(animator, "DemoAni", 0, (index)=>{
//				Debug.Log("ani callback[" + index + "] => use " + (Time.time - startTime) + "s");
//			});
//		});

		AB_Notification.ME.AddObserve (this, AB_Event.TEST, test);
		AB_Notification.ME.AddObserve (this, AB_Event.TEST2, test2);
		AB_Notification.ME.Post (AB_Event.TEST, "1aaaa");
		AB_Notification.ME.Post (AB_Event.TEST2, "1bbbb");

		AB_Notification.ME.RemoveObserveName (this, AB_Event.TEST);
		AB_Notification.ME.Post (AB_Event.TEST, "2aaaa");
		AB_Notification.ME.Post (AB_Event.TEST2, "2bbbb");

		AB_Notification.ME.AddObserve (this, AB_Event.TEST, test);
		AB_Notification.ME.Post (AB_Event.TEST, "3aaaa");
		AB_Notification.ME.Post (AB_Event.TEST2, "3bbbb");

		AB_Notification.ME.RemoveObserve (this);
		AB_Notification.ME.Post (AB_Event.TEST, "4aaaa");
		AB_Notification.ME.Post (AB_Event.TEST2, "4bbbb");

		// Test Load a scene in asset bundle
		//SceneLoader.Load("Scene/TestScene/TestScene.unity");

		// 开始加载我们的公告界面！
		//UIModule.Instance.OpenWindow("Billboard");
	}


	protected void test (object obj)
	{
		Debug.Log ("test " + obj);
	}

	protected void test2 (object obj)
	{
		Debug.Log ("test2 " + obj);
	}

}
