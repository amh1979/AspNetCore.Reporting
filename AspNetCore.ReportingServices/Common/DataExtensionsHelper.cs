using System;
using System.Reflection;

namespace AspNetCore.ReportingServices.Common
{
	internal sealed class DataExtensionsHelper
	{
		internal static Type GetDataExtensionConnectionType(string extensionProvider, string getProviderConnectionTypeMethod)
		{
			try
			{
				Assembly assembly = Assembly.Load("AspNetCore.ReportingServices.DataExtensions.dll");
				Type type = assembly.GetType(extensionProvider);
				return (Type)type.InvokeMember(getProviderConnectionTypeMethod, BindingFlags.Static | BindingFlags.Public, null, null, null);
			}
			catch
			{
				return null;
			}
		}
	}
}
