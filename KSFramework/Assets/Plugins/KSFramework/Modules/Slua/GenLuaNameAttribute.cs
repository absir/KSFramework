using System.Collections;
using System;
using LuaInterface;
using System.Reflection;

namespace SLua
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct)]
	public class GenLuaNameAttribute : System.Attribute
	{
		private string name;

		public GenLuaNameAttribute()
		{
			this.name = "";
		}

		public GenLuaNameAttribute(string name)
		{
			this.name = name;
		}

		public string getName() {
			return name;
		}
	}
}

