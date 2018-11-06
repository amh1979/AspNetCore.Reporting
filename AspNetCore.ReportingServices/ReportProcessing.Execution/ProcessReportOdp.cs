using Microsoft.Cloud.Platform.Utils;
using AspNetCore.ReportingServices.Diagnostics;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System;
using System.Diagnostics;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing.Execution
{
	internal abstract class ProcessReportOdp
	{
		private readonly IConfiguration m_configuration;

		private readonly ProcessingContext m_publicProcessingContext;

		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.Report m_reportDefinition;

		private readonly ReportProcessing.StoreServerParameters m_storeServerParameters;

		private readonly GlobalIDOwnerCollection m_globalIDOwnerCollection;

		private readonly ExecutionLogContext m_executionLogContext;

		private readonly ErrorContext m_errorContext;

		protected abstract bool SnapshotProcessing
		{
			get;
		}

		protected abstract bool ReprocessSnapshot
		{
			get;
		}

		protected abstract bool ProcessWithCachedData
		{
			get;
		}

		protected abstract OnDemandProcessingContext.Mode OnDemandProcessingMode
		{
			get;
		}

		protected IConfiguration Configuration
		{
			get
			{
				return this.m_configuration;
			}
		}

		protected ProcessingContext PublicProcessingContext
		{
			get
			{
				return this.m_publicProcessingContext;
			}
		}

		protected AspNetCore.ReportingServices.ReportIntermediateFormat.Report ReportDefinition
		{
			get
			{
				return this.m_reportDefinition;
			}
		}

		protected GlobalIDOwnerCollection GlobalIDOwnerCollection
		{
			get
			{
				return this.m_globalIDOwnerCollection;
			}
		}

		protected ErrorContext ErrorContext
		{
			get
			{
				return this.m_errorContext;
			}
		}

		protected ReportProcessing.StoreServerParameters StoreServerParameters
		{
			get
			{
				return this.m_storeServerParameters;
			}
		}

		protected ExecutionLogContext ExecutionLogContext
		{
			get
			{
				return this.m_executionLogContext;
			}
		}

		public ProcessReportOdp(IConfiguration configuration, ProcessingContext pc, AspNetCore.ReportingServices.ReportIntermediateFormat.Report report, ErrorContext errorContext, ReportProcessing.StoreServerParameters storeServerParameters, GlobalIDOwnerCollection globalIDOwnerCollection, ExecutionLogContext executionLogContext)
		{
			this.m_configuration = configuration;
			this.m_publicProcessingContext = pc;
			this.m_reportDefinition = report;
			this.m_errorContext = errorContext;
			this.m_storeServerParameters = storeServerParameters;
			this.m_globalIDOwnerCollection = globalIDOwnerCollection;
			this.m_executionLogContext = executionLogContext;
		}

		public AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot Execute(out OnDemandProcessingContext odpContext)
		{
			ReportProcessingCompatibilityVersion.TraceCompatibilityVersion(this.m_configuration);
			odpContext = null;
			OnDemandMetadata onDemandMetadata = this.PrepareMetadata();
			onDemandMetadata.GlobalIDOwnerCollection = this.m_globalIDOwnerCollection;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot = onDemandMetadata.ReportSnapshot;
			Global.Tracer.Assert(reportSnapshot != null, "ReportSnapshot object must exist");
			try
			{
				UserProfileState userProfileState = UserProfileState.None;
				if (this.PublicProcessingContext.Parameters != null)
				{
					userProfileState |= this.PublicProcessingContext.Parameters.UserProfileState;
				}
				odpContext = this.CreateOnDemandContext(onDemandMetadata, reportSnapshot, userProfileState);
				this.CompleteOdpContext(odpContext);
				Merge odpMerge = default(Merge);
				AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance = this.CreateReportInstance(odpContext, onDemandMetadata, reportSnapshot, out odpMerge);
				this.PreProcessSnapshot(odpContext, odpMerge, reportInstance, reportSnapshot);
				odpContext.SnapshotProcessing = true;
				odpContext.IsUnrestrictedRenderFormatReferenceMode = true;
				this.ResetEnvironment(odpContext, reportInstance);
				if (odpContext.ThreadCulture != null)
				{
					Thread.CurrentThread.CurrentCulture = odpContext.ThreadCulture;
				}
				this.UpdateUserProfileLocation(odpContext);
				return reportSnapshot;
			}
			finally
			{
				this.CleanupAbortHandler(odpContext);
				if (odpContext != null && odpContext.GlobalDataSourceInfo != null && odpContext.GlobalDataSourceInfo.Values != null)
				{
					foreach (ReportProcessing.DataSourceInfo value in odpContext.GlobalDataSourceInfo.Values)
					{
						if (value.TransactionInfo != null)
						{
							if (value.TransactionInfo.RollbackRequired)
							{
								if (Global.Tracer.TraceInfo)
								{
									Global.Tracer.Trace(TraceLevel.Info, "Data source '{0}': Rolling back transaction.", value.DataSourceName.MarkAsModelInfo());
								}
								try
								{
									value.TransactionInfo.Transaction.Rollback();
								}
								catch (Exception innerException)
								{
									throw new ReportProcessingException(ErrorCode.rsErrorRollbackTransaction, innerException, value.DataSourceName.MarkAsModelInfo());
								}
							}
							else
							{
								if (Global.Tracer.TraceVerbose)
								{
									Global.Tracer.Trace(TraceLevel.Verbose, "Data source '{0}': Committing transaction.", value.DataSourceName.MarkAsModelInfo());
								}
								try
								{
									value.TransactionInfo.Transaction.Commit();
								}
								catch (Exception innerException2)
								{
									throw new ReportProcessingException(ErrorCode.rsErrorCommitTransaction, innerException2, value.DataSourceName.MarkAsModelInfo());
								}
							}
						}
						if (value.Connection != null)
						{
							try
							{
								odpContext.CreateAndSetupDataExtensionFunction.CloseConnection(value.Connection, value.ProcDataSourceInfo, value.DataExtDataSourceInfo);
							}
							catch (Exception innerException3)
							{
								throw new ReportProcessingException(ErrorCode.rsErrorClosingConnection, innerException3, value.DataSourceName.MarkAsModelInfo());
							}
						}
					}
				}
			}
		}

		protected virtual void UpdateUserProfileLocation(OnDemandProcessingContext odpContext)
		{
			odpContext.ReportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.OnDemandExpressions);
		}

		protected virtual void CleanupAbortHandler(OnDemandProcessingContext odpContext)
		{
			if (odpContext != null)
			{
				odpContext.UnregisterAbortInfo();
			}
		}

		protected virtual OnDemandProcessingContext CreateOnDemandContext(OnDemandMetadata odpMetadata, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot, UserProfileState initialUserDependency)
		{
			return new OnDemandProcessingContext(this.PublicProcessingContext, this.ReportDefinition, odpMetadata, this.m_errorContext, reportSnapshot.ExecutionTime, this.SnapshotProcessing, this.ReprocessSnapshot, this.ProcessWithCachedData, this.m_storeServerParameters, initialUserDependency, this.m_executionLogContext, this.Configuration, this.OnDemandProcessingMode, this.GetAbortHelper());
		}

		protected virtual IAbortHelper GetAbortHelper()
		{
			if (this.PublicProcessingContext.JobContext == null)
			{
				return null;
			}
			return this.PublicProcessingContext.JobContext.GetAbortHelper();
		}

		protected virtual void ResetEnvironment(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance)
		{
			odpContext.SetupEnvironment(reportInstance);
			odpContext.ReportObjectModel.AggregatesImpl.ResetAll();
		}

		protected virtual AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance CreateReportInstance(OnDemandProcessingContext odpContext, OnDemandMetadata odpMetadata, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot, out Merge odpMerge)
		{
			odpMerge = new Merge(this.ReportDefinition, odpContext);
			AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ChunkManager.OnDemandProcessingManager.EnsureGroupTreeStorageSetup(odpMetadata, odpContext.ChunkFactory, odpMetadata.GlobalIDOwnerCollection, false, odpContext.GetActiveCompatibilityVersion(), odpContext.ProhibitSerializableValues);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance2 = odpContext.CurrentReportInstance = odpMerge.PrepareReportInstance(odpMetadata);
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance4 = reportSnapshot.ReportInstance = reportInstance2;
			AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance5 = reportInstance4;
			odpMerge.Init(this.PublicProcessingContext.Parameters);
			this.SetupReportLanguage(odpMerge, reportInstance5);
			odpMerge.SetupReport(reportInstance5);
			return reportInstance5;
		}

		protected abstract void PreProcessSnapshot(OnDemandProcessingContext odpContext, Merge odpMerge, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot);

		protected abstract OnDemandMetadata PrepareMetadata();

		protected virtual void CompleteOdpContext(OnDemandProcessingContext odpContext)
		{
		}

		protected abstract void SetupReportLanguage(Merge odpMerge, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance);

		protected void SetupInitialOdpState(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportInstance reportInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.ReportSnapshot reportSnapshot)
		{
			reportSnapshot.HasUserSortFilter = this.ReportDefinition.ReportOrDescendentHasUserSortFilter;
			odpContext.SetupEnvironment(reportInstance);
		}
	}
}
