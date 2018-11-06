using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class SubReport : ReportItem
	{
		private Report m_report;

		private ReportStringProperty m_noRowsMessage;

		private bool m_processedWithError;

		private SubReportErrorCodes m_errorCode;

		private string m_errorMessage;

		private bool m_noRows;

		private bool m_isNewContext = true;

		public string ReportName
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return ((AspNetCore.ReportingServices.ReportProcessing.SubReport)base.m_renderReportItem.ReportItemDef).ReportPath;
				}
				return ((AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport)base.m_reportItemDef).ReportName;
			}
		}

		public Report Report
		{
			get
			{
				this.RetrieveSubreport();
				return this.m_report;
			}
		}

		public ReportStringProperty NoRowsMessage
		{
			get
			{
				if (this.m_noRowsMessage == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_noRowsMessage = new ReportStringProperty(((AspNetCore.ReportingServices.ReportProcessing.SubReport)base.m_renderReportItem.ReportItemDef).NoRows);
					}
					else
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo noRowsMessage = ((AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport)base.m_reportItemDef).NoRowsMessage;
						if (noRowsMessage == null)
						{
							this.m_noRowsMessage = new ReportStringProperty(false, null, null);
						}
						else
						{
							this.m_noRowsMessage = new ReportStringProperty(noRowsMessage.IsExpression, noRowsMessage.OriginalText, noRowsMessage.StringValue);
						}
					}
				}
				return this.m_noRowsMessage;
			}
		}

		public bool OmitBorderOnPageBreak
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return false;
				}
				return ((AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport)base.m_reportItemDef).OmitBorderOnPageBreak;
			}
		}

		public bool KeepTogether
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return true;
				}
				return ((AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport)base.m_reportItemDef).KeepTogether;
			}
		}

		internal bool ProcessedWithError
		{
			get
			{
				this.RetrieveSubreport();
				return this.m_processedWithError;
			}
		}

		internal SubReportErrorCodes ErrorCode
		{
			get
			{
				this.RetrieveSubreport();
				return this.m_errorCode;
			}
		}

		internal string ErrorMessage
		{
			get
			{
				this.RetrieveSubreport();
				return this.m_errorMessage;
			}
		}

		internal bool NoRows
		{
			get
			{
				this.RetrieveSubreport();
				return this.m_noRows;
			}
		}

		internal SubReport(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal SubReport(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.SubReport renderSubReport, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderSubReport, renderingContext)
		{
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (base.m_instance == null)
			{
				base.m_instance = new SubReportInstance(this);
			}
			return base.m_instance;
		}

		internal void RetrieveSubreport()
		{
			if (this.m_isNewContext)
			{
				if (base.m_isOldSnapshot)
				{
					AspNetCore.ReportingServices.ReportRendering.SubReport subReport = (AspNetCore.ReportingServices.ReportRendering.SubReport)base.m_renderReportItem;
					if (subReport.Report != null)
					{
						if (this.m_report == null)
						{
							this.m_report = new Report(this, base.m_inSubtotal, subReport, base.m_renderingContext);
						}
						else
						{
							this.m_report.UpdateSubReportContents(this, subReport);
						}
					}
					this.m_noRows = subReport.NoRows;
					this.m_processedWithError = subReport.ProcessedWithError;
				}
				else
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport2 = (AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport)base.m_reportItemDef;
					RenderingContext renderingContext = null;
					try
					{
						if (subReport2.ExceededMaxLevel)
						{
							this.m_errorCode = SubReportErrorCodes.ExceededMaxRecursionLevel;
							this.m_errorMessage = RPRes.rsExceededMaxRecursionLevel(subReport2.Name);
							this.FinalizeErrorMessageAndThrow();
						}
						else
						{
							this.CheckRetrievalStatus(subReport2.RetrievalStatus);
						}
						if (base.m_renderingContext.InstanceAccessDisallowed)
						{
							renderingContext = this.GetOrCreateRenderingContext(subReport2, null);
							renderingContext.SubReportHasNoInstance = true;
						}
						else
						{
							base.m_renderingContext.OdpContext.SetupContext(subReport2, base.Instance.ReportScopeInstance);
							if (subReport2.CurrentSubReportInstance == null)
							{
								renderingContext = this.GetOrCreateRenderingContext(subReport2, null);
								renderingContext.SubReportHasNoInstance = true;
							}
							else
							{
								AspNetCore.ReportingServices.ReportIntermediateFormat.SubReportInstance subReportInstance = subReport2.CurrentSubReportInstance.Value();
								this.m_noRows = subReportInstance.NoRows;
								this.m_processedWithError = subReportInstance.ProcessedWithError;
								if (this.m_processedWithError)
								{
									this.CheckRetrievalStatus(subReportInstance.RetrievalStatus);
								}
								AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance = subReportInstance.ReportInstance.Value();
								renderingContext = this.GetOrCreateRenderingContext(subReport2, reportInstance);
								renderingContext.OdpContext.LoadExistingSubReportDataChunkNameModifier(subReportInstance);
								renderingContext.OdpContext.SetSubReportContext(subReportInstance, true);
								reportInstance.SetupEnvironment(renderingContext.OdpContext);
							}
						}
					}
					catch (Exception e)
					{
						this.m_processedWithError = true;
						ErrorContext subReportErrorContext = null;
						if (subReport2.OdpContext != null)
						{
							subReportErrorContext = subReport2.OdpContext.ErrorContext;
						}
						if (renderingContext == null && this.m_report != null)
						{
							renderingContext = this.m_report.RenderingContext;
						}
						AspNetCore.ReportingServices.ReportProcessing.ReportProcessing.HandleSubReportProcessingError(base.m_renderingContext.OdpContext.TopLevelContext.ErrorContext, subReport2, subReport2.UniqueName, subReportErrorContext, e);
					}
					if (renderingContext != null)
					{
						renderingContext.SubReportProcessedWithError = this.m_processedWithError;
					}
				}
				if (this.m_processedWithError)
				{
					this.m_noRows = false;
					if (this.m_errorCode == SubReportErrorCodes.Success)
					{
						this.m_errorCode = SubReportErrorCodes.ProcessingError;
						this.m_errorMessage = RPRes.rsRenderSubreportError;
					}
				}
				this.m_isNewContext = false;
			}
		}

		private RenderingContext GetOrCreateRenderingContext(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			RenderingContext renderingContext = null;
			if (this.m_report == null)
			{
				renderingContext = new RenderingContext(base.m_renderingContext, subReport.OdpContext);
				this.m_report = new Report(this, subReport.Report, reportInstance, renderingContext, subReport.ReportName, subReport.Description, base.m_inSubtotal);
			}
			else
			{
				renderingContext = this.m_report.RenderingContext;
				this.m_report.SetNewContext(reportInstance);
			}
			return renderingContext;
		}

		private void CheckRetrievalStatus(AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status status)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport subReport = (AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport)base.m_reportItemDef;
			switch (status)
			{
			case AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.NotRetrieved:
			case AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DefinitionRetrieveFailed:
				this.m_errorCode = SubReportErrorCodes.MissingSubReport;
				this.m_errorMessage = RPRes.rsMissingSubReport(subReport.Name, subReport.OriginalCatalogPath);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DataRetrieveFailed:
				this.m_errorCode = SubReportErrorCodes.DataRetrievalFailed;
				this.m_errorMessage = RPRes.rsSubReportDataRetrievalFailed(subReport.Name, subReport.OriginalCatalogPath);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.DataNotRetrieved:
				this.m_errorCode = SubReportErrorCodes.DataNotRetrieved;
				this.m_errorMessage = RPRes.rsSubReportDataNotRetrieved(subReport.Name, subReport.OriginalCatalogPath);
				break;
			case AspNetCore.ReportingServices.ReportIntermediateFormat.SubReport.Status.ParametersNotSpecified:
				this.m_errorCode = SubReportErrorCodes.ParametersNotSpecified;
				this.m_errorMessage = RPRes.rsSubReportParametersNotSpecified(subReport.Name, subReport.OriginalCatalogPath);
				break;
			default:
				this.m_errorCode = SubReportErrorCodes.Success;
				this.m_errorMessage = null;
				break;
			}
			this.FinalizeErrorMessageAndThrow();
		}

		private void FinalizeErrorMessageAndThrow()
		{
			if (this.m_errorMessage == null)
			{
				return;
			}
			IConfiguration configuration = base.m_renderingContext.OdpContext.Configuration;
			string errorMessage = this.m_errorMessage;
			if (configuration == null || !configuration.ShowSubreportErrorDetails)
			{
				this.m_errorMessage = RPRes.rsRenderSubreportError;
			}
			throw new RenderingObjectModelException(errorMessage);
		}

		internal override void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			this.SetNewContext();
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			this.m_isNewContext = true;
			this.m_noRows = true;
			this.m_processedWithError = false;
			this.m_errorCode = SubReportErrorCodes.Success;
		}

		internal override void SetNewContextChildren()
		{
			if (this.m_report != null)
			{
				this.m_report.SetNewContext();
			}
		}
	}
}
