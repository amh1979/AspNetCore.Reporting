using System;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal abstract class MemberMapping : TypeMapping
	{
		public bool IsReadOnly;

		public XmlAttributes XmlAttributes;

		public MemberMapping(Type type, string name, string ns, bool isReadOnly)
			: base(type)
		{
			base.Name = name;
			base.Namespace = ns;
			this.IsReadOnly = isReadOnly;
		}

		public abstract void SetValue(object obj, object value);

		public abstract object GetValue(object obj);

		public abstract bool HasValue(object obj);
	}
}
