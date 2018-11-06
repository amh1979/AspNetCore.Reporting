using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlObjectModel.Serialization
{
	internal class StructMapping : TypeMapping
	{
		private struct UseTypeInfo
		{
			public string Name;

			public string Namespace;
		}

		public NameTable<MemberMapping> Elements;

		public NameTable<MemberMapping> Attributes;

		private List<MemberMapping> m_members;

		private List<UseTypeInfo> m_useTypes;

		public List<MemberMapping> Members
		{
			get
			{
				return this.m_members;
			}
		}

		public StructMapping(Type type)
			: base(type)
		{
			this.Elements = new NameTable<MemberMapping>();
			this.Attributes = new NameTable<MemberMapping>();
			this.m_members = new List<MemberMapping>();
		}

		public MemberMapping GetAttribute(string name, string ns)
		{
			return this.Attributes[name, ns];
		}

		public MemberMapping GetElement(string name, string ns)
		{
			return this.Elements[name, ns];
		}

		public void AddUseTypeInfo(string name, string ns)
		{
			UseTypeInfo item = default(UseTypeInfo);
			item.Name = name;
			item.Namespace = ns;
			if (this.m_useTypes == null)
			{
				this.m_useTypes = new List<UseTypeInfo>();
			}
			this.m_useTypes.Add(item);
		}

		public List<MemberMapping> GetTypeNameElements()
		{
			if (this.m_useTypes != null)
			{
				List<MemberMapping> list = new List<MemberMapping>();
				{
					foreach (UseTypeInfo useType in this.m_useTypes)
					{
						UseTypeInfo current = useType;
						list.Add(this.Elements[current.Name, current.Namespace]);
					}
					return list;
				}
			}
			return null;
		}

		internal void ResolveChildAttributes()
		{
			if (base.ChildAttributes != null)
			{
				for (int i = 0; i < base.ChildAttributes.Count; i++)
				{
					MemberMapping memberMapping = base.ChildAttributes[i];
					string elementName = ((XmlChildAttributeAttribute)memberMapping.XmlAttributes.XmlAttribute).ElementName;
					MemberMapping element = this.GetElement(elementName, "");
					element.AddChildAttribute(memberMapping);
				}
				base.ChildAttributes = null;
			}
		}
	}
}
