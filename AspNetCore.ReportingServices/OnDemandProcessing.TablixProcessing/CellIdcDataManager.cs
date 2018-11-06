using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal sealed class CellIdcDataManager : BaseIdcDataManager
	{
		private readonly IntersectJoinInfo m_joinInfo;

		private readonly bool m_shareOuterGroupDataSet;

		private readonly IRIFReportIntersectionScope m_cellScope;

		private readonly AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet m_sharedDataSet;

		private RuntimeDataTablixGroupLeafObjReference m_lastOuterGroupLeafRef;

		private RuntimeDataTablixGroupLeafObjReference m_lastInnerGroupLeafRef;

		private readonly Relationship m_activeOuterRelationship;

		private readonly Relationship m_activeInnerRelationship;

		private AspNetCore.ReportingServices.RdlExpressions.VariantResult[] m_lastOuterPrimaryKeyValues;

		private AspNetCore.ReportingServices.RdlExpressions.VariantResult[] m_lastInnerPrimaryKeyValues;

		public CellIdcDataManager(OnDemandProcessingContext odpContext, IRIFReportDataScope idcReportDataScope)
			: base(odpContext, idcReportDataScope.DataScopeInfo.DataSet)
		{
			this.m_joinInfo = (idcReportDataScope.DataScopeInfo.JoinInfo as IntersectJoinInfo);
			Global.Tracer.Assert(this.m_joinInfo != null, "Did not find expected IntersectionJoinInfo");
			this.m_cellScope = (IRIFReportIntersectionScope)idcReportDataScope;
			if (!this.m_cellScope.IsColumnOuterGrouping)
			{
				this.m_activeOuterRelationship = this.m_joinInfo.GetActiveRowRelationship(base.m_idcDataSet);
				this.m_activeInnerRelationship = this.m_joinInfo.GetActiveColumnRelationship(base.m_idcDataSet);
				this.m_sharedDataSet = this.m_joinInfo.RowParentDataSet;
			}
			else
			{
				this.m_activeInnerRelationship = this.m_joinInfo.GetActiveRowRelationship(base.m_idcDataSet);
				this.m_activeOuterRelationship = this.m_joinInfo.GetActiveColumnRelationship(base.m_idcDataSet);
				this.m_sharedDataSet = this.m_joinInfo.ColumnParentDataSet;
			}
			this.m_shareOuterGroupDataSet = (this.m_activeOuterRelationship == null);
		}

		public void RegisterActiveIntersection(RuntimeDataTablixGroupLeafObjReference innerGroupLeafRef, RuntimeDataTablixGroupLeafObjReference outerGroupLeafRef)
		{
			if ((BaseReference)this.m_lastOuterGroupLeafRef != (object)outerGroupLeafRef)
			{
				if ((BaseReference)this.m_lastOuterGroupLeafRef != (object)null)
				{
					using (this.m_lastOuterGroupLeafRef.PinValue())
					{
						this.m_lastOuterGroupLeafRef.Value().ResetStreamingModeIdcRowBuffer();
					}
				}
				this.m_lastOuterGroupLeafRef = outerGroupLeafRef;
				if (this.m_activeOuterRelationship != null)
				{
					this.m_lastOuterPrimaryKeyValues = this.EvaluatePrimaryKeyExpressions(this.m_lastOuterGroupLeafRef, this.m_activeOuterRelationship);
				}
			}
			if ((BaseReference)this.m_lastInnerGroupLeafRef != (object)innerGroupLeafRef)
			{
				this.m_lastInnerGroupLeafRef = innerGroupLeafRef;
				this.m_lastInnerPrimaryKeyValues = this.EvaluatePrimaryKeyExpressions(this.m_lastInnerGroupLeafRef, this.m_activeInnerRelationship);
			}
		}

		private AspNetCore.ReportingServices.RdlExpressions.VariantResult[] EvaluatePrimaryKeyExpressions(RuntimeDataTablixGroupLeafObjReference groupLeafRef, Relationship relationship)
		{
			RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = groupLeafRef.Value();
			DataFieldRow dataFieldRow = runtimeDataTablixGroupLeafObj.DataRows[0];
			dataFieldRow.RestoreDataSetAndSetFields(base.m_odpContext, relationship.RelatedDataSet.DataSetCore.FieldsContext);
			return relationship.EvaluateJoinConditionKeys(true, base.m_odpContext.ReportRuntime);
		}

		public override void Advance()
		{
			using (this.m_lastInnerGroupLeafRef.PinValue())
			{
				using (this.m_lastOuterGroupLeafRef.PinValue())
				{
					RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = this.m_lastInnerGroupLeafRef.Value();
					RuntimeDataTablixGroupLeafObj rowGroupLeaf = this.m_lastOuterGroupLeafRef.Value();
					OnDemandStateManager stateManager = base.m_odpContext.StateManager;
					ObjectModelImpl reportObjectModel = base.m_odpContext.ReportObjectModel;
					if (base.m_idcDataSet.DataSetCore.FieldsContext != null)
					{
						reportObjectModel.RestoreFields(base.m_idcDataSet.DataSetCore.FieldsContext);
					}
					while (this.SetupNextRow(this.m_lastOuterPrimaryKeyValues, this.m_activeOuterRelationship, this.m_lastInnerPrimaryKeyValues, this.m_activeInnerRelationship))
					{
						base.ApplyGroupingFieldsForServerAggregates(this.m_cellScope);
						RuntimeCell orCreateCell = runtimeDataTablixGroupLeafObj.GetOrCreateCell(rowGroupLeaf);
						bool rowAccepted = orCreateCell.NextRow();
						if (stateManager.ShouldStopPipelineAdvance(rowAccepted))
						{
							break;
						}
					}
				}
			}
		}

		private bool SetupNextRow(AspNetCore.ReportingServices.RdlExpressions.VariantResult[] rowPrimaryKeys, Relationship rowRelationship, AspNetCore.ReportingServices.RdlExpressions.VariantResult[] columnPrimaryKeys, Relationship columnRelationship)
		{
			if (!this.ReadRowFromDataSet())
			{
				return false;
			}
			bool flag = false;
			if (this.m_shareOuterGroupDataSet)
			{
				flag = base.Correlate(columnRelationship, columnPrimaryKeys, true);
			}
			else if (flag = base.Correlate(rowRelationship, rowPrimaryKeys, true))
			{
				flag &= base.Correlate(columnRelationship, columnPrimaryKeys, true);
			}
			if (!flag)
			{
				this.PushBackLastRow();
			}
			return flag;
		}

		protected override void PushBackLastRow()
		{
			if (this.m_shareOuterGroupDataSet)
			{
				using (this.m_lastOuterGroupLeafRef.PinValue())
				{
					this.m_lastOuterGroupLeafRef.Value().PushBackStreamingModeIdcRowToBuffer();
				}
			}
			else
			{
				base.PushBackLastRow();
			}
		}

		protected override bool ReadRowFromDataSet()
		{
			if (this.m_shareOuterGroupDataSet)
			{
				using (this.m_lastOuterGroupLeafRef.PinValue())
				{
					RuntimeDataTablixGroupLeafObj runtimeDataTablixGroupLeafObj = this.m_lastOuterGroupLeafRef.Value();
					return runtimeDataTablixGroupLeafObj.ReadStreamingModeIdcRowFromBufferOrDataSet(this.m_sharedDataSet.DataSetCore.FieldsContext);
				}
			}
			return base.ReadRowFromDataSet();
		}

		protected override void SetupRelationshipQueryRestart()
		{
			base.AddRelationshipRestartPosition(this.m_activeOuterRelationship, this.m_lastOuterPrimaryKeyValues);
			base.AddRelationshipRestartPosition(this.m_activeInnerRelationship, this.m_lastInnerPrimaryKeyValues);
		}

		public override void Close()
		{
			base.Close();
			this.m_lastOuterGroupLeafRef = null;
			this.m_lastInnerGroupLeafRef = null;
		}
	}
}
