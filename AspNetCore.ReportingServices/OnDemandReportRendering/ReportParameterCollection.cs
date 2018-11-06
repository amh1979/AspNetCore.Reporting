using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportParameterCollection : ReportElementCollectionBase<ReportParameter>
	{
		private List<ReportParameter> m_parameters;

		private Dictionary<string, ReportParameter> m_parametersByName;

		private NameValueCollection m_reportParameters;

		public ReportParameter this[string name]
		{
			get
			{
				if (this.m_parametersByName == null)
				{
					this.m_parametersByName = new Dictionary<string, ReportParameter>(this.m_parameters.Count);
					for (int i = 0; i < this.m_parameters.Count; i++)
					{
						ReportParameter reportParameter = this.m_parameters[i];
						this.m_parametersByName.Add(reportParameter.Name, reportParameter);
					}
				}
				return this.m_parametersByName[name];
			}
		}

		public override ReportParameter this[int index]
		{
			get
			{
				return this.m_parameters[index];
			}
		}

		public override int Count
		{
			get
			{
				return this.m_parameters.Count;
			}
		}

		internal NameValueCollection ToNameValueCollection
		{
			get
			{
				if (this.m_reportParameters == null && this.m_parameters != null)
				{
					int count = this.m_parameters.Count;
					this.m_reportParameters = new NameValueCollection(count);
					for (int i = 0; i < count; i++)
					{
						ReportParameter reportParameter = this.m_parameters[i];
						ReportParameterInstance instance = reportParameter.Instance;
						if (instance != null && instance.Values != null)
						{
							int count2 = instance.Values.Count;
							for (int j = 0; j < count2; j++)
							{
								this.m_reportParameters.Add(reportParameter.Name, Formatter.FormatWithInvariantCulture(instance.Values[j]));
							}
						}
					}
					if (count > 0)
					{
						this.m_reportParameters.Add("rs:ParameterLanguage", "");
					}
				}
				return this.m_reportParameters;
			}
		}

		internal ReportParameterCollection(ParameterDefList parameterDefs, AspNetCore.ReportingServices.ReportRendering.ReportParameterCollection paramValues)
		{
			this.m_parameters = new List<ReportParameter>(parameterDefs.Count);
			for (int i = 0; i < parameterDefs.Count; i++)
			{
				if (parameterDefs[i].PromptUser)
				{
					this.m_parameters.Add(new ReportParameter(parameterDefs[i]));
				}
			}
			this.UpdateRenderReportItem(paramValues);
		}

		internal ReportParameterCollection(OnDemandProcessingContext odpContext, List<AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef> parameterDefs, bool validInstance)
		{
			this.m_parameters = new List<ReportParameter>(parameterDefs.Count);
			for (int i = 0; i < parameterDefs.Count; i++)
			{
				if (parameterDefs[i].PromptUser)
				{
					this.m_parameters.Add(new ReportParameter(odpContext, parameterDefs[i]));
				}
			}
			this.SetNewContext(validInstance);
		}

		internal void SetNewContext(bool validInstance)
		{
			for (int i = 0; i < this.m_parameters.Count; i++)
			{
				this.m_parameters[i].SetNewContext(validInstance);
			}
		}

		internal void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportParameterCollection paramValues)
		{
			int count = this.m_parameters.Count;
			if (paramValues != null && paramValues.Count != count)
			{
				paramValues = null;
			}
			for (int i = 0; i < count; i++)
			{
				if (paramValues == null)
				{
					this.m_parameters[i].UpdateRenderReportItem(null);
				}
				else
				{
					this.m_parameters[i].UpdateRenderReportItem(paramValues[i]);
				}
			}
		}
	}
}
