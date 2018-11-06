using AspNetCore.ReportingServices.RdlObjectModel.Serialization;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlObjectModel.RdlUpgrade
{
	internal abstract class SerializerHostBase : ISerializerHost
	{
		protected string m_extraStringData;

		protected bool m_serializing;

		public string ExtraStringData
		{
			get
			{
				return this.m_extraStringData;
			}
			set
			{
				this.m_extraStringData = value;
			}
		}

		internal SerializerHostBase(bool serializing)
		{
			this.m_serializing = serializing;
		}

		public abstract Type GetSubstituteType(Type type);

		public virtual void OnDeserialization(object value)
		{
		}

		public abstract IEnumerable<ExtensionNamespace> GetExtensionNamespaces();
	}
}
