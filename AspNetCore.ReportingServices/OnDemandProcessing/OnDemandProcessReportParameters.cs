using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class OnDemandProcessReportParameters : ProcessReportParameters
	{
		private AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_report;

		internal OnDemandProcessReportParameters(AspNetCore.ReportingServices.ReportIntermediateFormat.Report aReport, OnDemandProcessingContext aContext)
			: base(aContext)
		{
			this.m_report = aReport;
			if (aContext.IsRdlSandboxingEnabled())
			{
				IRdlSandboxConfig rdlSandboxing = aContext.Configuration.RdlSandboxing;
				base.m_maxStringResultLength = rdlSandboxing.MaxStringResultLength;
			}
		}

		internal OnDemandProcessingContext GetOnDemandContext()
		{
			return (OnDemandProcessingContext)base.ProcessingContext;
		}

		internal override IParameterDef GetParameterDef(int aParamIndex)
		{
			Global.Tracer.Assert(aParamIndex < this.m_report.Parameters.Count, "Invalid Parameter Index.  Found: {0}.  Count: {1}", aParamIndex, this.m_report.Parameters.Count);
			return this.m_report.Parameters[aParamIndex];
		}

		internal override void InitParametersContext(ParameterInfoCollection parameters)
		{
		}

		internal override void Cleanup()
		{
		}

		internal override void AddToRuntime(ParameterInfo aParamInfo)
		{
			ParameterImpl parameter = new ParameterImpl(aParamInfo);
			this.GetOnDemandContext().ReportObjectModel.ParametersImpl.Add(aParamInfo.Name, parameter);
		}

		internal override void SetupExprHost(IParameterDef aParamDef)
		{
			OnDemandProcessingContext onDemandContext = this.GetOnDemandContext();
			if (onDemandContext.ReportRuntime.ReportExprHost != null)
			{
				((AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef)aParamDef).SetExprHost(onDemandContext.ReportRuntime.ReportExprHost, onDemandContext.ReportObjectModel);
			}
		}

		internal override object EvaluateDefaultValueExpr(IParameterDef aParamDef, int aIndex)
		{
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = this.GetOnDemandContext().ReportRuntime.EvaluateParamDefaultValue((AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef)aParamDef, aIndex);
			if (variantResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, aParamDef.Name);
			}
			return variantResult.Value;
		}

		internal override object EvaluateValidValueExpr(IParameterDef aParamDef, int aIndex)
		{
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = this.GetOnDemandContext().ReportRuntime.EvaluateParamValidValue((AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef)aParamDef, aIndex);
			if (variantResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, aParamDef.Name);
			}
			return variantResult.Value;
		}

		internal override object EvaluateValidValueLabelExpr(IParameterDef aParamDef, int aIndex)
		{
			AspNetCore.ReportingServices.RdlExpressions.VariantResult variantResult = this.GetOnDemandContext().ReportRuntime.EvaluateParamValidValueLabel((AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef)aParamDef, aIndex);
			if (variantResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, aParamDef.Name);
			}
			return variantResult.Value;
		}

		internal override string EvaluatePromptExpr(ParameterInfo aParamInfo, IParameterDef aParamDef)
		{
			return this.GetOnDemandContext().ReportRuntime.EvaluateParamPrompt((AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef)aParamDef);
		}

		internal override bool NeedPrompt(IParameterDataSource paramDS)
		{
			bool result = false;
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSource dataSource = this.m_report.DataSources[paramDS.DataSourceIndex];
			if (this.GetOnDemandContext().DataSourceInfos != null)
			{
				DataSourceInfo byID = this.GetOnDemandContext().DataSourceInfos.GetByID(dataSource.ID);
				if (byID != null)
				{
					result = byID.NeedPrompt;
				}
			}
			return result;
		}

		internal override void ThrowExceptionForQueryBackedParameter(ReportProcessingException_FieldError aError, string aParamName, int aDataSourceIndex, int aDataSetIndex, int aFieldIndex, string propertyName)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = this.m_report.DataSources[aDataSourceIndex].DataSets[aDataSetIndex];
			throw new ReportProcessingException(ErrorCode.rsReportParameterQueryProcessingError, aParamName.MarkAsPrivate(), propertyName, dataSet.Fields[aFieldIndex].Name.MarkAsModelInfo(), dataSet.Name.MarkAsPrivate(), AspNetCore.ReportingServices.ReportProcessing.ReportRuntime.GetErrorName(aError.Status, aError.Message));
		}

		internal override ReportParameterDataSetCache ProcessReportParameterDataSet(ParameterInfo aParam, IParameterDef aParamDef, IParameterDataSource paramDS, bool aRetrieveValidValues, bool aRetrievalDefaultValues)
		{
			ReportParameterDataSetCache reportParameterDataSetCache = new OnDemandReportParameterDataSetCache(this, aParam, (AspNetCore.ReportingServices.ReportIntermediateFormat.ParameterDef)aParamDef, aRetrieveValidValues, aRetrievalDefaultValues);
			RetrievalManager retrievalManager = new RetrievalManager(this.m_report, this.GetOnDemandContext());
			retrievalManager.FetchParameterData(reportParameterDataSetCache, paramDS.DataSourceIndex, paramDS.DataSetIndex);
			return reportParameterDataSetCache;
		}

		protected override string ApplySandboxStringRestriction(string value, string paramName, string propertyName)
		{
			return ProcessReportParameters.ApplySandboxRestriction(ref value, paramName, propertyName, this.GetOnDemandContext(), base.m_maxStringResultLength);
		}
	}
}
