using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal class ArrayMapping : TypeMapping
	{
		public Dictionary<string, Type> ElementTypes;

		public Type ItemType;

		public ArrayMapping(Type type)
			: base(type)
		{
			if (type.IsArray)
			{
				base.Name = base.Type.GetElementType().Name + "Array";
			}
			else if (type.IsGenericType)
			{
				Type[] genericArguments = type.GetGenericArguments();
				if (genericArguments != null && genericArguments.Length > 0)
				{
					base.Name = genericArguments[0].Name + "Collection";
				}
			}
		}
	}
}
