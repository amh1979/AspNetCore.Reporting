using System;
using System.Reflection;

namespace AspNetCore.ReportingServices.Diagnostics
{
	internal sealed class AssemblyLocationResolver : MarshalByRefObject
	{
		private readonly bool m_fullLoad;

		public static AssemblyLocationResolver CreateResolver(AppDomain tempAppDomain)
		{
			if (tempAppDomain == null)
			{
				return new AssemblyLocationResolver(true);
			}
            
            return Activator.CreateInstance<AssemblyLocationResolver>();
        }

		public string LoadAssemblyAndResolveLocation(string name)
		{
			if (this.m_fullLoad)
			{
				return Assembly.Load(name).Location;
			}
			return Assembly.ReflectionOnlyLoad(name).Location;
		}

		public AssemblyLocationResolver()
			: this(false)
		{
		}

		private AssemblyLocationResolver(bool fullLoad)
		{
			this.m_fullLoad = fullLoad;
		}
	}
}
