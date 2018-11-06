using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using AspNetCore.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class OnDemandPageEvaluation : PageEvaluation
	{
		private OnDemandProcessingContext m_processingContext;

		private Dictionary<string, ReportSection> m_reportItemToReportSection = new Dictionary<string, ReportSection>();

		internal OnDemandPageEvaluation(Report report)
			: base(report)
		{
			this.InitializeEnvironment();
		}

		internal override void Add(string textboxName, object textboxValue)
		{
			if (textboxName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			if (this.m_processingContext.ReportItemsReferenced)
			{
				AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.TextBoxImpl textBoxImpl = (AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.TextBoxImpl)((ReportItems)this.m_processingContext.ReportObjectModel.ReportItemsImpl)[textboxName];
				if (textBoxImpl != null)
				{
					textBoxImpl.SetResult(new AspNetCore.ReportingServices.RdlExpressions.VariantResult(false, textboxValue));
				}
				ReportSection reportSection = default(ReportSection);
				AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.AggregatesImpl aggregatesImpl = default(AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.AggregatesImpl);
				if (this.m_reportItemToReportSection.TryGetValue(textboxName, out reportSection) && reportSection.PageAggregatesOverReportItems.TryGetValue(textboxName, out aggregatesImpl))
				{
					foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj @object in aggregatesImpl.Objects)
					{
						@object.Update();
					}
				}
			}
		}

		internal override void UpdatePageSections(ReportSection section)
		{
			if (section.Page.PageHeader == null && section.Page.PageFooter == null)
			{
				return;
			}
			AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl reportObjectModel = this.m_processingContext.ReportObjectModel;
			reportObjectModel.GlobalsImpl.SetPageName(base.m_pageName);
			if (section.PageAggregatesOverReportItems == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidPageSectionState, section.SectionIndex);
			}
			foreach (AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.AggregatesImpl value in section.PageAggregatesOverReportItems.Values)
			{
				foreach (AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj @object in value.Objects)
				{
					reportObjectModel.AggregatesImpl.Add(@object);
				}
			}
			section.PageAggregatesOverReportItems = null;
		}

		internal override void Reset(ReportSection section, int newPageNumber, int newTotalPages, int newOverallPageNumber, int newOverallTotalPages)
		{
			base.Reset(section, newPageNumber, newTotalPages, newOverallPageNumber, newOverallTotalPages);
			if (section.Page.PageHeader == null && section.Page.PageFooter == null)
			{
				return;
			}
			this.PageInit(section);
		}

		private void InitializeEnvironment()
		{
			this.m_processingContext = base.m_romReport.HeaderFooterRenderingContext.OdpContext;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Report reportDef = base.m_romReport.ReportDef;
			AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl reportObjectModel = this.m_processingContext.ReportObjectModel;
			if (reportDef.DataSetsNotOnlyUsedInParameters == 1)
			{
				this.m_processingContext.SetupFieldsForNewDataSetPageSection(reportDef.FirstDataSet);
			}
			else
			{
				this.m_processingContext.SetupEmptyTopLevelFields();
			}
			reportObjectModel.VariablesImpl = new VariablesImpl(false);
			if (reportDef.HasVariables)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance currentReportInstance = base.m_romReport.RenderingContext.OdpContext.CurrentReportInstance;
				this.m_processingContext.RuntimeInitializePageSectionVariables(reportDef, (currentReportInstance != null) ? currentReportInstance.VariableValues : null);
			}
			reportObjectModel.LookupsImpl = new LookupsImpl();
			if (reportDef.HasLookups)
			{
				this.m_processingContext.RuntimeInitializeLookups(reportDef);
			}
			AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ReportItemsImpl reportItemsImpl = new AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ReportItemsImpl(false);
			foreach (ReportSection reportSection in base.m_romReport.ReportSections)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection sectionDef = reportSection.SectionDef;
				reportSection.BodyItemsForHeadFoot = new AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ReportItemsImpl(false);
				reportSection.PageSectionItemsForHeadFoot = new AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ReportItemsImpl(false);
				reportObjectModel.ReportItemsImpl = reportSection.BodyItemsForHeadFoot;
				this.m_processingContext.RuntimeInitializeTextboxObjs(sectionDef.ReportItems, false);
				reportObjectModel.ReportItemsImpl = reportSection.PageSectionItemsForHeadFoot;
				AspNetCore.ReportingServices.ReportIntermediateFormat.Page page = sectionDef.Page;
				if (page.PageHeader != null)
				{
					if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
					{
						page.PageHeader.SetExprHost(this.m_processingContext.ReportRuntime.ReportExprHost, reportObjectModel);
					}
					this.m_processingContext.RuntimeInitializeReportItemObjs(page.PageHeader.ReportItems, false);
					this.m_processingContext.RuntimeInitializeTextboxObjs(page.PageHeader.ReportItems, true);
				}
				if (page.PageFooter != null)
				{
					if (this.m_processingContext.ReportRuntime.ReportExprHost != null)
					{
						page.PageFooter.SetExprHost(this.m_processingContext.ReportRuntime.ReportExprHost, reportObjectModel);
					}
					this.m_processingContext.RuntimeInitializeReportItemObjs(page.PageFooter.ReportItems, false);
					this.m_processingContext.RuntimeInitializeTextboxObjs(page.PageFooter.ReportItems, true);
				}
				reportItemsImpl.AddAll(reportSection.BodyItemsForHeadFoot);
				reportItemsImpl.AddAll(reportSection.PageSectionItemsForHeadFoot);
			}
			reportObjectModel.ReportItemsImpl = reportItemsImpl;
			reportObjectModel.AggregatesImpl = new AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.AggregatesImpl(this.m_processingContext);
		}

		private void PageInit(ReportSection section)
		{
			AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.ObjectModelImpl reportObjectModel = this.m_processingContext.ReportObjectModel;
			AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.AggregatesImpl aggregatesImpl = reportObjectModel.AggregatesImpl;
			Global.Tracer.Assert(section.BodyItemsForHeadFoot != null, "Missing cached BodyItemsForHeadFoot collection");
			Global.Tracer.Assert(section.PageSectionItemsForHeadFoot != null, "Missing cached PageSectionItemsForHeadFoot collection");
			section.BodyItemsForHeadFoot.ResetAll(default(AspNetCore.ReportingServices.RdlExpressions.VariantResult));
			section.PageSectionItemsForHeadFoot.ResetAll();
			reportObjectModel.GlobalsImpl.SetPageNumbers(base.m_currentPageNumber, base.m_totalPages, base.m_currentOverallPageNumber, base.m_overallTotalPages);
			reportObjectModel.GlobalsImpl.SetPageName(base.m_pageName);
			AspNetCore.ReportingServices.ReportIntermediateFormat.Report reportDef = base.m_romReport.ReportDef;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSection sectionDef = section.SectionDef;
			AspNetCore.ReportingServices.ReportIntermediateFormat.Page page = sectionDef.Page;
			section.PageAggregatesOverReportItems = new Dictionary<string, AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.AggregatesImpl>();
			this.m_processingContext.ReportObjectModel.ReportItemsImpl.SpecialMode = true;
			if (page.PageAggregates != null)
			{
				for (int i = 0; i < page.PageAggregates.Count; i++)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateInfo dataAggregateInfo = page.PageAggregates[i];
					aggregatesImpl.Remove(dataAggregateInfo);
					dataAggregateInfo.ExprHostInitialized = false;
					AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj dataAggregateObj = new AspNetCore.ReportingServices.ReportIntermediateFormat.DataAggregateObj(dataAggregateInfo, this.m_processingContext);
					object[] array = default(object[]);
					DataFieldStatus dataFieldStatus = default(DataFieldStatus);
					dataAggregateObj.EvaluateParameters(out array, out dataFieldStatus);
					string specialModeIndex = reportObjectModel.ReportItemsImpl.GetSpecialModeIndex();
					if (specialModeIndex == null)
					{
						aggregatesImpl.Add(dataAggregateObj);
					}
					else
					{
						AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.AggregatesImpl aggregatesImpl2 = default(AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.AggregatesImpl);
						if (!section.PageAggregatesOverReportItems.TryGetValue(specialModeIndex, out aggregatesImpl2))
						{
							aggregatesImpl2 = new AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel.AggregatesImpl(this.m_processingContext);
							section.PageAggregatesOverReportItems.Add(specialModeIndex, aggregatesImpl2);
						}
						aggregatesImpl2.Add(dataAggregateObj);
						this.m_reportItemToReportSection[specialModeIndex] = section;
					}
					dataAggregateObj.Init();
				}
			}
			reportObjectModel.ReportItemsImpl.SpecialMode = false;
			AspNetCore.ReportingServices.ReportIntermediateFormat.PageSection rifObject = null;
			IReportScopeInstance romInstance = null;
			if (sectionDef.Page.PageHeader != null)
			{
				rifObject = sectionDef.Page.PageHeader;
				romInstance = section.Page.PageHeader.Instance.ReportScopeInstance;
				section.Page.PageHeader.SetNewContext();
			}
			if (sectionDef.Page.PageFooter != null)
			{
				rifObject = sectionDef.Page.PageFooter;
				romInstance = section.Page.PageFooter.Instance.ReportScopeInstance;
				section.Page.PageFooter.SetNewContext();
			}
			if (sectionDef != null)
			{
				this.m_processingContext.SetupContext(rifObject, romInstance);
			}
		}
	}
}
