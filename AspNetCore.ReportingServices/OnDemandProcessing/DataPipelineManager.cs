using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandProcessing
{
	internal abstract class DataPipelineManager
	{
		protected readonly OnDemandProcessingContext m_odpContext;

		protected readonly AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet m_dataSet;

		public abstract IOnDemandScopeInstance GroupTreeRoot
		{
			get;
		}

		protected abstract RuntimeDataSource RuntimeDataSource
		{
			get;
		}

		public int DataSetIndex
		{
			get
			{
				return this.m_dataSet.IndexInCollection;
			}
		}

		public DataPipelineManager(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet)
		{
			this.m_odpContext = odpContext;
			this.m_dataSet = dataSet;
		}

		public void StartProcessing()
		{
			if (!this.m_odpContext.InSubreport)
			{
				this.m_odpContext.ExecutionLogContext.StartTablixProcessingTimer();
			}
			bool isTablixProcessingMode = this.m_odpContext.IsTablixProcessingMode;
			UserProfileState? nullable = null;
			try
			{
				this.m_odpContext.IsTablixProcessingMode = true;
				nullable = this.m_odpContext.ReportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(UserProfileState.InReport);
				this.InternalStartProcessing();
			}
			finally
			{
				this.m_odpContext.ReportRuntime.CurrentScope = null;
				this.m_odpContext.IsTablixProcessingMode = isTablixProcessingMode;
				if (nullable.HasValue)
				{
					this.m_odpContext.ReportObjectModel.UserImpl.UpdateUserProfileLocationWithoutLocking(nullable.Value);
				}
				if (!this.m_odpContext.InSubreport)
				{
					this.m_odpContext.ExecutionLogContext.StopTablixProcessingTimer();
				}
			}
		}

		protected abstract void InternalStartProcessing();

		public void StopProcessing()
		{
			this.m_dataSet.ClearDataRegionStreamingScopeInstances();
			if (this.RuntimeDataSource != null)
			{
				this.RuntimeDataSource.RecordTimeDataRetrieval();
			}
			this.InternalStopProcessing();
		}

		protected abstract void InternalStopProcessing();

		public abstract void Advance();

		public virtual void Abort()
		{
		}
	}
}
