using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimPageEvaluation : PageEvaluation
	{
		private AspNetCore.ReportingServices.ReportProcessing.Report m_report;

		private CultureInfo m_reportCulture;

		private Hashtable m_aggregatesOverReportItems;

		private AggregatesImpl m_aggregates;

		private AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext m_processingContext;

		internal ShimPageEvaluation(Report report)
			: base(report)
		{
			this.InitializeEnvironment();
			this.PageInit();
		}

		internal override void Reset(ReportSection section, int newPageNumber, int newTotalPages, int newOverallPageNumber, int newOverallTotalPages)
		{
			base.Reset(section, newPageNumber, newTotalPages, newOverallPageNumber, newOverallTotalPages);
			this.PageInit();
		}

		internal override void Add(string textboxName, object textboxValue)
		{
			if (textboxName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			foreach (DataAggregateObj @object in this.m_aggregates.Objects)
			{
				this.m_processingContext.ReportObjectModel.AggregatesImpl.Add(@object);
			}
			if (this.m_processingContext.ReportItemsReferenced)
			{
				TextBoxImpl textBoxImpl = (TextBoxImpl)((ReportItems)this.m_processingContext.ReportObjectModel.ReportItemsImpl)[textboxName];
				if (textBoxImpl != null)
				{
					textBoxImpl.SetResult(new VariantResult(false, textboxValue));
				}
				AggregatesImpl aggregatesImpl = (AggregatesImpl)this.m_aggregatesOverReportItems[textboxName];
				if (aggregatesImpl != null)
				{
					foreach (DataAggregateObj object2 in aggregatesImpl.Objects)
					{
						object2.Update();
					}
				}
			}
		}

		internal override void UpdatePageSections(ReportSection section)
		{
			AspNetCore.ReportingServices.ReportRendering.PageSection header = null;
			AspNetCore.ReportingServices.ReportRendering.PageSection footer = null;
			foreach (AggregatesImpl value in this.m_aggregatesOverReportItems.Values)
			{
				foreach (DataAggregateObj @object in value.Objects)
				{
					this.m_processingContext.ReportObjectModel.AggregatesImpl.Add(@object);
				}
			}
			if (this.m_report.PageHeaderEvaluation)
			{
				header = this.GenerateRenderPageSection(this.m_report.PageHeader, "ph");
			}
			if (this.m_report.PageFooterEvaluation)
			{
				footer = this.GenerateRenderPageSection(this.m_report.PageFooter, "pf");
			}
			this.m_aggregates = null;
			this.m_aggregatesOverReportItems = null;
			section.Page.UpdateWithCurrentPageSections(header, footer);
		}

		private AspNetCore.ReportingServices.ReportRendering.PageSection GenerateRenderPageSection(AspNetCore.ReportingServices.ReportProcessing.PageSection pageSection, string uniqueNamePrefix)
		{
			AspNetCore.ReportingServices.ReportProcessing.PageSectionInstance pageSectionInstance = new AspNetCore.ReportingServices.ReportProcessing.PageSectionInstance(this.m_processingContext, base.m_currentPageNumber, pageSection);
			AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.PageMerge.CreateInstances(this.m_processingContext, pageSectionInstance.ReportItemColInstance, pageSection.ReportItems);
			string text = base.m_currentPageNumber.ToString(CultureInfo.InvariantCulture) + uniqueNamePrefix;
			AspNetCore.ReportingServices.ReportRendering.RenderingContext renderingContext = new AspNetCore.ReportingServices.ReportRendering.RenderingContext(base.m_romReport.RenderReport.RenderingContext, text);
			return new AspNetCore.ReportingServices.ReportRendering.PageSection(text, pageSection, pageSectionInstance, base.m_romReport.RenderReport, renderingContext, false);
		}

		private void InitializeEnvironment()
		{
			this.m_report = base.m_romReport.RenderReport.ReportDef;
			AspNetCore.ReportingServices.ReportProcessing.ReportInstance reportInstance = base.m_romReport.RenderReport.ReportInstance;
			AspNetCore.ReportingServices.ReportRendering.RenderingContext renderingContext = base.m_romReport.RenderReport.RenderingContext;
			ReportSnapshot reportSnapshot = renderingContext.ReportSnapshot;
			ReportInstanceInfo reportInstanceInfo = (ReportInstanceInfo)reportInstance.GetInstanceInfo(renderingContext.ChunkManager);
			this.m_processingContext = new AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.ProcessingContext(renderingContext.TopLevelReportContext, this.m_report.ShowHideType, renderingContext.GetResourceCallback, this.m_report.EmbeddedImages, this.m_report.ImageStreamNames, new ProcessingErrorContext(), !this.m_report.PageMergeOnePass, renderingContext.AllowUserProfileState, renderingContext.ReportRuntimeSetup, renderingContext.DataProtection);
			this.m_reportCulture = Localization.DefaultReportServerSpecificCulture;
			if (this.m_report.Language != null)
			{
				string text = null;
				text = ((this.m_report.Language.Type != ExpressionInfo.Types.Constant) ? reportInstance.Language : this.m_report.Language.Value);
				if (text != null)
				{
					try
					{
						this.m_reportCulture = new CultureInfo(text, false);
						if (this.m_reportCulture.IsNeutralCulture)
						{
							this.m_reportCulture = CultureInfo.CreateSpecificCulture(text);
							this.m_reportCulture = new CultureInfo(this.m_reportCulture.Name, false);
						}
					}
					catch (Exception e)
					{
						if (!AsynchronousExceptionDetection.IsStoppingException(e))
						{
							goto end_IL_0140;
						}
						throw;
						end_IL_0140:;
					}
				}
			}
			this.m_processingContext.ReportObjectModel = new ObjectModelImpl(this.m_processingContext);
			Global.Tracer.Assert(this.m_processingContext.ReportRuntime == null, "(m_processingContext.ReportRuntime == null)");
			this.m_processingContext.ReportRuntime = new ReportRuntime(this.m_processingContext.ReportObjectModel, this.m_processingContext.ErrorContext);
			this.m_processingContext.ReportObjectModel.FieldsImpl = new FieldsImpl();
			this.m_processingContext.ReportObjectModel.ParametersImpl = new ParametersImpl(reportInstanceInfo.Parameters.Count);
			this.m_processingContext.ReportObjectModel.GlobalsImpl = new GlobalsImpl(reportInstanceInfo.ReportName, base.m_currentPageNumber, base.m_totalPages, reportSnapshot.ExecutionTime, reportSnapshot.ReportServerUrl, reportSnapshot.ReportFolder);
			this.m_processingContext.ReportObjectModel.UserImpl = new UserImpl(reportSnapshot.RequestUserName, reportSnapshot.Language, this.m_processingContext.AllowUserProfileState);
			this.m_processingContext.ReportObjectModel.DataSetsImpl = new DataSetsImpl();
			this.m_processingContext.ReportObjectModel.DataSourcesImpl = new DataSourcesImpl(this.m_report.DataSourceCount);
			for (int i = 0; i < reportInstanceInfo.Parameters.Count; i++)
			{
				this.m_processingContext.ReportObjectModel.ParametersImpl.Add(reportInstanceInfo.Parameters[i].Name, new ParameterImpl(reportInstanceInfo.Parameters[i].Values, reportInstanceInfo.Parameters[i].Labels, reportInstanceInfo.Parameters[i].MultiValue));
			}
			this.m_processingContext.ReportRuntime.LoadCompiledCode(this.m_report, false, this.m_processingContext.ReportObjectModel, this.m_processingContext.ReportRuntimeSetup);
		}

		private void PageInit()
		{
			this.m_processingContext.ReportObjectModel.GlobalsImpl.SetPageNumbers(base.m_currentPageNumber, base.m_totalPages);
			this.m_processingContext.ReportObjectModel.ReportItemsImpl = new ReportItemsImpl();
			this.m_processingContext.ReportObjectModel.AggregatesImpl = new AggregatesImpl(this.m_processingContext.ReportRuntime);
			if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
			{
				this.m_processingContext.RuntimeInitializeReportItemObjs(this.m_report.ReportItems, true, true);
				if (this.m_report.PageHeader != null)
				{
					if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
					{
						this.m_report.PageHeader.SetExprHost(this.m_processingContext.ReportRuntime.ReportExprHost, this.m_processingContext.ReportObjectModel);
					}
					this.m_processingContext.RuntimeInitializeReportItemObjs(this.m_report.PageHeader.ReportItems, false, false);
				}
				if (this.m_report.PageFooter != null)
				{
					if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
					{
						this.m_report.PageFooter.SetExprHost(this.m_processingContext.ReportRuntime.ReportExprHost, this.m_processingContext.ReportObjectModel);
					}
					this.m_processingContext.RuntimeInitializeReportItemObjs(this.m_report.PageFooter.ReportItems, false, false);
				}
			}
			this.m_aggregates = new AggregatesImpl(this.m_processingContext.ReportRuntime);
			this.m_aggregatesOverReportItems = new Hashtable();
			this.m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = true;
			if (this.m_report.PageAggregates != null)
			{
				for (int i = 0; i < this.m_report.PageAggregates.Count; i++)
				{
					DataAggregateInfo dataAggregateInfo = this.m_report.PageAggregates[i];
					dataAggregateInfo.ExprHostInitialized = false;
					DataAggregateObj dataAggregateObj = new DataAggregateObj(dataAggregateInfo, this.m_processingContext);
					object[] array = default(object[]);
					DataFieldStatus dataFieldStatus = default(DataFieldStatus);
					dataAggregateObj.EvaluateParameters(out array, out dataFieldStatus);
					string specialModeIndex = this.m_processingContext.ReportObjectModel.ReportItemsImpl.GetSpecialModeIndex();
					if (specialModeIndex == null)
					{
						this.m_aggregates.Add(dataAggregateObj);
					}
					else
					{
						AggregatesImpl aggregatesImpl = (AggregatesImpl)this.m_aggregatesOverReportItems[specialModeIndex];
						if (aggregatesImpl == null)
						{
							aggregatesImpl = new AggregatesImpl(this.m_processingContext.ReportRuntime);
							this.m_aggregatesOverReportItems.Add(specialModeIndex, aggregatesImpl);
						}
						aggregatesImpl.Add(dataAggregateObj);
					}
					dataAggregateObj.Init();
				}
			}
			this.m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = false;
		}
	}
}
