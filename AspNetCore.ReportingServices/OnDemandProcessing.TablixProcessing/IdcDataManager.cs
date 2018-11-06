using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class IdcDataManager : LinearIdcDataManager
	{
		private readonly IRIFReportDataScope m_idcReportDataScope;

		private IReference<IDataCorrelation> m_lastRuntimeReceiver;

		public IdcDataManager(OnDemandProcessingContext odpContext, IRIFReportDataScope idcReportDataScope)
			: base(odpContext, idcReportDataScope.DataScopeInfo.DataSet, IdcDataManager.GetActiveRelationship(idcReportDataScope))
		{
			this.m_idcReportDataScope = idcReportDataScope;
		}

		private static Relationship GetActiveRelationship(IRIFReportDataScope idcReportDataScope)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet dataSet = idcReportDataScope.DataScopeInfo.DataSet;
			LinearJoinInfo linearJoinInfo = idcReportDataScope.DataScopeInfo.JoinInfo as LinearJoinInfo;
			Global.Tracer.Assert(linearJoinInfo != null, "Did not find expected LinearJoinInfo");
			Relationship activeRelationship = linearJoinInfo.GetActiveRelationship(dataSet);
			Global.Tracer.Assert(activeRelationship != null, "Could not find active relationship");
			return activeRelationship;
		}

		internal void SetSkippingFilter(List<AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo> expressions, List<object> values)
		{
			base.m_skippingFilter = new RowSkippingFilter(base.m_odpContext, this.m_idcReportDataScope, expressions, values);
		}

		internal void ClearSkippingFilter()
		{
			base.m_skippingFilter = null;
		}

		protected override void UpdateActiveParent(IReference<IOnDemandScopeInstance> parentScopeInstanceRef)
		{
			this.m_lastRuntimeReceiver = parentScopeInstanceRef.Value().GetIdcReceiver(this.m_idcReportDataScope);
		}

		public override void Advance()
		{
			OnDemandStateManager stateManager = base.m_odpContext.StateManager;
			ObjectModelImpl reportObjectModel = base.m_odpContext.ReportObjectModel;
			if (base.m_idcDataSet.DataSetCore.FieldsContext != null)
			{
				reportObjectModel.RestoreFields(base.m_idcDataSet.DataSetCore.FieldsContext);
			}
			using (this.m_lastRuntimeReceiver.PinValue())
			{
				IDataCorrelation dataCorrelation = this.m_lastRuntimeReceiver.Value();
				while (base.SetupCorrelatedRow(false))
				{
					base.ApplyGroupingFieldsForServerAggregates(this.m_idcReportDataScope);
					bool rowAccepted = dataCorrelation.NextCorrelatedRow();
					if (stateManager.ShouldStopPipelineAdvance(rowAccepted))
					{
						break;
					}
				}
			}
		}

		public override void Close()
		{
			base.Close();
			this.m_lastRuntimeReceiver = null;
		}
	}
}
