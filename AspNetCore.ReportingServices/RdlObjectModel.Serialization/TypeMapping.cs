using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal class TypeMapping
	{
		public Type Type;

		public string Name;

		public string Namespace;

		public List<MemberMapping> ChildAttributes;

		public TypeMapping(Type type)
		{
			this.Type = type;
			this.Namespace = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";
			this.Name = type.Name;
		}

		internal void AddChildAttribute(MemberMapping mapping)
		{
			if (this.ChildAttributes == null)
			{
				this.ChildAttributes = new List<MemberMapping>();
			}
			this.ChildAttributes.Add(mapping);
		}
	}
}
