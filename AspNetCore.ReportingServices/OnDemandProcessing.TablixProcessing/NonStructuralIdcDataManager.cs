using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class NonStructuralIdcDataManager : LinearIdcDataManager
	{
		private readonly IRIFReportDataScope m_sourceDataScope;

		private bool m_lastCorrelationHadMatchingRow;

		private IReference<IOnDemandScopeInstance> m_lastParentScopeInstance;

		internal IRIFReportDataScope SourceDataScope
		{
			get
			{
				return this.m_sourceDataScope;
			}
		}

		internal IReference<IOnDemandScopeInstance> LastParentScopeInstance
		{
			get
			{
				return this.m_lastParentScopeInstance;
			}
		}

		public NonStructuralIdcDataManager(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet targetDataSet, IRIFReportDataScope sourceDataScope)
			: base(odpContext, targetDataSet, NonStructuralIdcDataManager.GetActiveRelationship(targetDataSet, sourceDataScope))
		{
			this.m_sourceDataScope = sourceDataScope;
		}

		private static Relationship GetActiveRelationship(AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet targetDataSet, IRIFReportDataScope sourceDataScope)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = sourceDataScope.DataScopeInfo.DataSet;
			Relationship defaultRelationship = targetDataSet.GetDefaultRelationship(dataSet);
			Global.Tracer.Assert(defaultRelationship != null, "Could not find active relationship");
			return defaultRelationship;
		}

		protected override void UpdateActiveParent(IReference<IOnDemandScopeInstance> parentScopeInstanceRef)
		{
			this.m_lastParentScopeInstance = parentScopeInstanceRef;
		}

		public override void Advance()
		{
			OnDemandStateManager stateManager = base.m_odpContext.StateManager;
			ObjectModelImpl reportObjectModel = base.m_odpContext.ReportObjectModel;
			if (base.m_idcDataSet.DataSetCore.FieldsContext != null)
			{
				reportObjectModel.RestoreFields(base.m_idcDataSet.DataSetCore.FieldsContext);
			}
			if (base.SetupCorrelatedRow(true))
			{
				this.m_lastCorrelationHadMatchingRow = true;
			}
			else
			{
				this.m_lastCorrelationHadMatchingRow = false;
				reportObjectModel.ResetFieldValues();
			}
		}

		public override void Close()
		{
			base.Close();
			this.m_lastParentScopeInstance = null;
		}

		public void SetupEnvironment()
		{
			ObjectModelImpl reportObjectModel = base.m_odpContext.ReportObjectModel;
			reportObjectModel.RestoreFields(base.m_idcDataSet.DataSetCore.FieldsContext);
			if (!this.m_lastCorrelationHadMatchingRow)
			{
				base.m_odpContext.ReportObjectModel.ResetFieldValues();
			}
		}
	}
}
