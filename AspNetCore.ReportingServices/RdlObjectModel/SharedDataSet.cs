using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace AspNetCore.ReportingServices.RdlObjectModel
{
	internal class SharedDataSet : ReportObject
	{
		internal class Definition : DefinitionStore<SharedDataSet, Definition.Properties>
		{
			internal enum Properties
			{
				SharedDataSetReference,
				QueryParameters
			}
		}

		private string m_reportServerUrl;

		public string SharedDataSetReference
		{
			get
			{
				return (string)base.PropertyStore.GetObject(0);
			}
			set
			{
				base.PropertyStore.SetObject(0, value);
			}
		}

		[XmlElement(typeof(RdlCollection<QueryParameter>))]
		public IList<QueryParameter> QueryParameters
		{
			get
			{
				return (IList<QueryParameter>)base.PropertyStore.GetObject(1);
			}
			set
			{
				base.PropertyStore.SetObject(1, value);
			}
		}

		[DefaultValue("")]
		[XmlElement(Namespace = "http://schemas.microsoft.com/SQLServer/reporting/reportdesigner")]
		public string ReportServerUrl
		{
			get
			{
				return this.m_reportServerUrl;
			}
			set
			{
				this.m_reportServerUrl = value;
			}
		}

		public SharedDataSet()
		{
		}

		internal SharedDataSet(IPropertyStore propertyStore)
			: base(propertyStore)
		{
		}

		public override void Initialize()
		{
			base.Initialize();
			this.QueryParameters = new RdlCollection<QueryParameter>();
		}
	}
}
