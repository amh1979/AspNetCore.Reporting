using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.DataExtensions;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal sealed class LegacyProcessReportParameters : ProcessReportParameters
	{
		private Report m_report;

		private AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.RuntimeDataSourceNode m_runtimeDataSourceNode;

		internal LegacyProcessReportParameters(Report aReport, AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext aContext)
			: base(aContext)
		{
			this.m_report = aReport;
		}

		internal AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext GetLegacyContext()
		{
			return (AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext)base.ProcessingContext;
		}

		internal override IParameterDef GetParameterDef(int aParamIndex)
		{
			return this.m_report.Parameters[aParamIndex];
		}

		internal override void InitParametersContext(ParameterInfoCollection parameters)
		{
			int dataSourceCount = this.m_report.DataSourceCount;
			AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext legacyContext = this.GetLegacyContext();
			Global.Tracer.Assert(null == legacyContext.ReportObjectModel, "(null == processingContext.ReportObjectModel)");
			legacyContext.ReportObjectModel = new ObjectModelImpl(legacyContext);
			legacyContext.ReportObjectModel.ParametersImpl = new ParametersImpl(parameters.Count);
			legacyContext.ReportObjectModel.FieldsImpl = new FieldsImpl();
			legacyContext.ReportObjectModel.ReportItemsImpl = new ReportItemsImpl();
			legacyContext.ReportObjectModel.AggregatesImpl = new AggregatesImpl(null);
			legacyContext.ReportObjectModel.GlobalsImpl = new GlobalsImpl(legacyContext.ReportContext.ItemName, legacyContext.ExecutionTime, legacyContext.ReportContext.HostRootUri, legacyContext.ReportContext.ParentPath);
			legacyContext.ReportObjectModel.UserImpl = new UserImpl(legacyContext.RequestUserName, legacyContext.UserLanguage.Name, legacyContext.AllowUserProfileState);
			legacyContext.ReportObjectModel.DataSetsImpl = new DataSetsImpl();
			legacyContext.ReportObjectModel.DataSourcesImpl = new DataSourcesImpl(dataSourceCount);
			if (legacyContext.ReportRuntime == null)
			{
				legacyContext.ReportRuntime = new ReportRuntime(legacyContext.ReportObjectModel, legacyContext.ErrorContext);
				legacyContext.ReportRuntime.LoadCompiledCode(this.m_report, true, legacyContext.ReportObjectModel, legacyContext.ReportRuntimeSetup);
			}
		}

		internal override void Cleanup()
		{
			AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext legacyContext = this.GetLegacyContext();
			if (legacyContext.ReportRuntime != null)
			{
				legacyContext.ReportRuntime.Close();
			}
		}

		internal override void AddToRuntime(ParameterInfo aParamInfo)
		{
			ParameterImpl parameter = new ParameterImpl(aParamInfo.Values, aParamInfo.Labels, aParamInfo.MultiValue);
			this.GetLegacyContext().ReportObjectModel.ParametersImpl.Add(aParamInfo.Name, parameter);
		}

		internal override void SetupExprHost(IParameterDef aParamDef)
		{
			AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext legacyContext = this.GetLegacyContext();
			if (legacyContext.ReportRuntime.ReportExprHost != null)
			{
				((ParameterDef)aParamDef).SetExprHost(legacyContext.ReportRuntime.ReportExprHost, legacyContext.ReportObjectModel);
			}
		}

		internal override string EvaluatePromptExpr(ParameterInfo aParamInfo, IParameterDef aParamDef)
		{
			Global.Tracer.Assert(false);
			return null;
		}

		internal override object EvaluateDefaultValueExpr(IParameterDef aParamDef, int aIndex)
		{
			VariantResult variantResult = this.GetLegacyContext().ReportRuntime.EvaluateParamDefaultValue((ParameterDef)aParamDef, aIndex);
			if (variantResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, aParamDef.Name);
			}
			return variantResult.Value;
		}

		internal override object EvaluateValidValueExpr(IParameterDef aParamDef, int aIndex)
		{
			VariantResult variantResult = this.GetLegacyContext().ReportRuntime.EvaluateParamValidValue((ParameterDef)aParamDef, aIndex);
			if (variantResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, aParamDef.Name);
			}
			return variantResult.Value;
		}

		internal override object EvaluateValidValueLabelExpr(IParameterDef aParamDef, int aIndex)
		{
			VariantResult variantResult = this.GetLegacyContext().ReportRuntime.EvaluateParamValidValueLabel((ParameterDef)aParamDef, aIndex);
			if (variantResult.ErrorOccurred)
			{
				throw new ReportProcessingException(ErrorCode.rsReportParameterProcessingError, aParamDef.Name);
			}
			return variantResult.Value;
		}

		internal override bool NeedPrompt(IParameterDataSource paramDS)
		{
			bool result = false;
			AspNetCore.ReportingServices.ReportProcessing.DataSource dataSource = this.m_report.DataSources[paramDS.DataSourceIndex];
			if (this.GetLegacyContext().DataSourceInfos != null)
			{
				DataSourceInfo byID = this.GetLegacyContext().DataSourceInfos.GetByID(dataSource.ID);
				if (byID != null)
				{
					result = byID.NeedPrompt;
				}
			}
			return result;
		}

		internal override void ThrowExceptionForQueryBackedParameter(ReportProcessingException_FieldError aError, string aParamName, int aDataSourceIndex, int aDataSetIndex, int aFieldIndex, string propertyName)
		{
			AspNetCore.ReportingServices.ReportProcessing.DataSet dataSet = this.m_report.DataSources[aDataSourceIndex].DataSets[aDataSetIndex];
			throw new ReportProcessingException(ErrorCode.rsReportParameterQueryProcessingError, aParamName.MarkAsPrivate(), propertyName, dataSet.Fields[aFieldIndex].Name.MarkAsModelInfo(), dataSet.Name.MarkAsPrivate(), ReportRuntime.GetErrorName(aError.Status, aError.Message));
		}

		internal override ReportParameterDataSetCache ProcessReportParameterDataSet(ParameterInfo aParam, IParameterDef aParamDef, IParameterDataSource paramDS, bool aRetrieveValidValues, bool aRetrievalDefaultValues)
		{
			EventHandler eventHandler = null;
			LegacyReportParameterDataSetCache legacyReportParameterDataSetCache = new LegacyReportParameterDataSetCache(this, aParam, (ParameterDef)aParamDef, aRetrieveValidValues, aRetrievalDefaultValues);
			AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportProcessingContext legacyContext = this.GetLegacyContext();
			try
			{
				this.m_runtimeDataSourceNode = new AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ReportRuntimeDataSourceNode(this.m_report, this.m_report.DataSources[paramDS.DataSourceIndex], paramDS.DataSetIndex, legacyContext, legacyReportParameterDataSetCache);
				eventHandler = this.AbortHandler;
				legacyContext.AbortInfo.ProcessingAbortEvent += eventHandler;
				if (Global.Tracer.TraceVerbose)
				{
					Global.Tracer.Trace(TraceLevel.Verbose, "Abort handler registered.");
				}
				this.m_runtimeDataSourceNode.InitProcessingParams(false, true);
				this.m_runtimeDataSourceNode.ProcessConcurrent(null);
				legacyContext.CheckAndThrowIfAborted();
				AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.RuntimeDataSetNode runtimeDataSetNode = this.m_runtimeDataSourceNode.RuntimeDataSetNodes[0];
				return legacyReportParameterDataSetCache;
			}
			finally
			{
				if (eventHandler != null)
				{
					legacyContext.AbortInfo.ProcessingAbortEvent -= eventHandler;
				}
				if (this.m_runtimeDataSourceNode != null)
				{
					this.m_runtimeDataSourceNode.Cleanup();
				}
			}
		}

		private void AbortHandler(object sender, EventArgs e)
		{
			if (Global.Tracer.TraceInfo)
			{
				Global.Tracer.Trace(TraceLevel.Info, "Merge abort handler called. Aborting data sources ...");
			}
			this.m_runtimeDataSourceNode.Abort();
		}

		protected override string ApplySandboxStringRestriction(string value, string paramName, string propertyName)
		{
			return value;
		}
	}
}
