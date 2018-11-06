using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections.ObjectModel;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportParameterInstance
	{
		private ReportParameter m_paramDef;

		private AspNetCore.ReportingServices.ReportRendering.ReportParameter m_renderParamValue;

		private AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ParameterImpl m_paramValue;

		public string Prompt
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return this.m_renderParamValue.Prompt;
				}
				return this.ReportOMParam.Prompt;
			}
		}

		public object Value
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return this.m_renderParamValue.Value;
				}
				object[] values = this.ReportOMParam.GetValues();
				if (values != null && values.Length != 0)
				{
					return values[0];
				}
				return null;
			}
		}

		public ReadOnlyCollection<object> Values
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return Array.AsReadOnly(this.m_renderParamValue.Values);
				}
				object[] values = this.ReportOMParam.GetValues();
				if (values != null && values.Length != 0)
				{
					return Array.AsReadOnly(values);
				}
				return null;
			}
		}

		public string Label
		{
			get
			{
				string[] array = (!this.IsOldSnapshot) ? this.ReportOMParam.GetLabels() : this.m_renderParamValue.UnderlyingParam.Labels;
				if (array != null && array.Length != 0)
				{
					return array[0];
				}
				return null;
			}
		}

		public ReadOnlyCollection<string> Labels
		{
			get
			{
				string[] array = (!this.IsOldSnapshot) ? this.ReportOMParam.GetLabels() : this.m_renderParamValue.UnderlyingParam.Labels;
				if (array != null && array.Length != 0)
				{
					return Array.AsReadOnly(array);
				}
				return null;
			}
		}

		internal bool IsOldSnapshot
		{
			get
			{
				return this.m_renderParamValue != null;
			}
		}

		internal AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ParameterImpl ReportOMParam
		{
			get
			{
				if (this.m_paramValue == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ParametersImpl parametersImpl = this.m_paramDef.OdpContext.ReportObjectModel.ParametersImpl;
					this.m_paramValue = (AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ParameterImpl)((Parameters)parametersImpl)[this.m_paramDef.Name];
				}
				return this.m_paramValue;
			}
		}

		internal ReportParameterInstance(ReportParameter paramDef)
		{
			this.m_paramDef = paramDef;
		}

		internal ReportParameterInstance(ReportParameter paramDef, AspNetCore.ReportingServices.ReportRendering.ReportParameter paramValue)
		{
			this.m_paramDef = paramDef;
			this.m_renderParamValue = paramValue;
		}

		internal void SetNewContext()
		{
			this.m_paramValue = null;
		}

		internal void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportParameter paramValue)
		{
			this.m_renderParamValue = paramValue;
		}
	}
}
