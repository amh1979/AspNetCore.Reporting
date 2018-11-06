using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportParameter
	{
		private AspNetCore.ReportingServices.ReportProcessing.ParameterDef m_renderParam;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef m_paramDef;

		private ReportParameterInstance m_paramInstance;

		private ReportStringProperty m_prompt;

		private bool m_validInstance;

		private OnDemandProcessingContext m_odpContext;

		public string Name
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return this.m_renderParam.Name;
				}
				return this.m_paramDef.Name;
			}
		}

		public TypeCode DataType
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return (TypeCode)this.m_renderParam.DataType;
				}
				return (TypeCode)this.m_paramDef.DataType;
			}
		}

		public bool Nullable
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return this.m_renderParam.Nullable;
				}
				return this.m_paramDef.Nullable;
			}
		}

		public bool MultiValue
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return this.m_renderParam.MultiValue;
				}
				return this.m_paramDef.MultiValue;
			}
		}

		public bool AllowBlank
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return this.m_renderParam.AllowBlank;
				}
				return this.m_paramDef.AllowBlank;
			}
		}

		public ReportStringProperty Prompt
		{
			get
			{
				if (this.m_prompt == null)
				{
					if (this.IsOldSnapshot)
					{
						this.m_prompt = new ReportStringProperty(false, this.m_renderParam.Prompt, this.m_renderParam.Prompt);
					}
					else
					{
						this.m_prompt = new ReportStringProperty(this.m_paramDef.PromptExpression);
					}
				}
				return this.m_prompt;
			}
		}

		public bool UsedInQuery
		{
			get
			{
				if (this.IsOldSnapshot)
				{
					return this.m_renderParam.UsedInQuery;
				}
				return this.m_paramDef.UsedInQuery;
			}
		}

		public ReportParameterInstance Instance
		{
			get
			{
				if (!this.m_validInstance)
				{
					return null;
				}
				if (this.IsOldSnapshot)
				{
					return this.m_paramInstance;
				}
				if (this.m_paramInstance == null)
				{
					this.m_paramInstance = new ReportParameterInstance(this);
				}
				return this.m_paramInstance;
			}
		}

		internal bool IsOldSnapshot
		{
			get
			{
				return this.m_renderParam != null;
			}
		}

		internal OnDemandProcessingContext OdpContext
		{
			get
			{
				return this.m_odpContext;
			}
		}

		internal ReportParameter(AspNetCore.ReportingServices.ReportProcessing.ParameterDef renderParam)
		{
			this.m_renderParam = renderParam;
		}

		internal ReportParameter(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef paramDef)
		{
			this.m_paramDef = paramDef;
			this.m_odpContext = odpContext;
		}

		internal void SetNewContext(bool validInstance)
		{
			this.m_validInstance = validInstance;
			if (this.m_paramInstance != null)
			{
				this.m_paramInstance.SetNewContext();
			}
		}

		internal void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportParameter paramValue)
		{
			if (paramValue == null)
			{
				this.m_validInstance = false;
			}
			else
			{
				this.m_validInstance = true;
				if (this.m_paramInstance == null)
				{
					this.m_paramInstance = new ReportParameterInstance(this, paramValue);
				}
				else
				{
					this.m_paramInstance.UpdateRenderReportItem(paramValue);
				}
			}
		}
	}
}
