using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ReportParameterCollection : NameObjectCollectionBase
	{
		private NameValueCollection m_asNameValueCollection;

		private bool m_isValid;

		public ReportParameter this[string name]
		{
			get
			{
				return (ReportParameter)base.BaseGet(name);
			}
		}

		public ReportParameter this[int index]
		{
			get
			{
				return (ReportParameter)base.BaseGet(index);
			}
		}

		public NameValueCollection AsNameValueCollection
		{
			get
			{
				if (this.m_asNameValueCollection == null)
				{
					int count = this.Count;
					this.m_asNameValueCollection = new NameValueCollection(count, StringComparer.Ordinal);
					for (int i = 0; i < count; i++)
					{
						ReportParameter reportParameter = this[i];
						this.m_asNameValueCollection.Add(reportParameter.Name, reportParameter.StringValues);
					}
				}
				return this.m_asNameValueCollection;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.m_isValid;
			}
		}

		internal ReportParameterCollection(ParameterInfoCollection parameters)
		{
			this.Init(parameters, true);
		}

		internal ReportParameterCollection(ParameterInfoCollection parameters, bool isValid)
		{
			this.Init(parameters, isValid);
		}

		private void Init(ParameterInfoCollection parameters, bool isValid)
		{
			this.m_isValid = isValid;
			int count = parameters.Count;
			for (int i = 0; i < count; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				if (parameterInfo.PromptUser)
				{
					base.BaseAdd(parameterInfo.Name, new ReportParameter(parameterInfo));
				}
			}
		}
	}
}
