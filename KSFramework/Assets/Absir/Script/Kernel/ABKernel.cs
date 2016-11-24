using UnityEngine;
using System;
using System.Collections;

namespace Absir
{
	public static class ABConfig
	{
		public static readonly BeanConfigImpl CONFIG = new BeanConfigImpl (null, Application.streamingAssetsPath);
	}

	[SLua.CustomLuaClassAttribute]
	[SLua.GenLuaName]
	public static class ABKernel
	{
		public static object Config (string name)
		{
			return ABConfig.CONFIG.getConfigValue (name);
		}
	}
}
