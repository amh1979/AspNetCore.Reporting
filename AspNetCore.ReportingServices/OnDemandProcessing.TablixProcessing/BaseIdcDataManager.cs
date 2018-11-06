using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportProcessing.OnDemandReportObjectModel;
using System;
using System.Diagnostics;

namespace AspNetCore.ReportingServices.OnDemandProcessing.TablixProcessing
{
	internal abstract class BaseIdcDataManager : IDisposable
	{
		protected OnDemandProcessingContext m_odpContext;

		protected readonly AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet m_idcDataSet;

		private readonly bool m_needsServerAggregateTranslation;

		private RuntimeIdcIncrementalDataSource m_dataSource;

		private DataFieldRow m_nextDataFieldRowToProcess;

		private long m_skippedRowCount;

		protected RowSkippingFilter m_skippingFilter;

		protected bool IsDataPipelineSetup
		{
			get
			{
				return this.m_dataSource != null;
			}
		}

		public BaseIdcDataManager(OnDemandProcessingContext odpContext, AspNetCore.ReportingServices.ReportIntermediateFormat.DataSet idcDataSet)
		{
			Global.Tracer.Assert(!odpContext.InSubreport, "IDC currently cannot be used inside subreports");
			this.m_odpContext = odpContext;
			this.m_idcDataSet = idcDataSet;
			this.m_needsServerAggregateTranslation = this.m_idcDataSet.HasScopeWithCustomAggregates;
		}

		public abstract void Advance();

		protected void ApplyGroupingFieldsForServerAggregates(IRIFReportDataScope idcReportDataScope)
		{
			if (this.m_needsServerAggregateTranslation)
			{
				idcReportDataScope.DataScopeInfo.ApplyGroupingFieldsForServerAggregates(this.m_odpContext.ReportObjectModel.FieldsImpl);
			}
		}

		protected virtual void PushBackLastRow()
		{
			FieldsImpl fieldsImplForUpdate = this.m_odpContext.ReportObjectModel.GetFieldsImplForUpdate(this.m_idcDataSet);
			if (fieldsImplForUpdate.IsAggregateRow)
			{
				this.m_nextDataFieldRowToProcess = new AggregateRow(fieldsImplForUpdate, true);
			}
			else
			{
				this.m_nextDataFieldRowToProcess = new DataFieldRow(fieldsImplForUpdate, true);
			}
		}

		protected virtual bool ReadRowFromDataSet()
		{
			if (this.m_nextDataFieldRowToProcess != null)
			{
				this.m_nextDataFieldRowToProcess.SetFields(this.m_odpContext.ReportObjectModel.GetFieldsImplForUpdate(this.m_idcDataSet));
				this.m_nextDataFieldRowToProcess = null;
			}
			else
			{
				if (this.m_dataSource == null)
				{
					if (this.m_odpContext.QueryRestartInfo != null)
					{
						this.SetupRelationshipQueryRestart();
					}
					this.m_dataSource = new RuntimeIdcIncrementalDataSource(this.m_idcDataSet, this.m_odpContext);
					this.m_dataSource.Initialize();
				}
				if (!this.m_dataSource.SetupNextRow())
				{
					return false;
				}
			}
			return true;
		}

		protected bool Correlate(Relationship relationship, AspNetCore.ReportingServices.RdlExpressions.VariantResult[] primaryKeys, bool advancedRowCursor)
		{
			SortDirection[] sortDirections = relationship.GetSortDirections();
			bool flag = false;
			bool flag2 = true;
			while (flag2 && !flag)
			{
				AspNetCore.ReportingServices.RdlExpressions.VariantResult[] array = relationship.EvaluateJoinConditionKeys(false, this.m_odpContext.ReportRuntime);
				flag = true;
				int num = 0;
				while (flag && primaryKeys != null && num < primaryKeys.Length)
				{
					int num2 = this.m_odpContext.CompareAndStopOnError(primaryKeys[num].Value, array[num].Value, ObjectType.DataSet, this.m_idcDataSet.Name, "JoinCondition", false);
					flag2 = ((sortDirections[num] != 0) ? (num2 <= 0) : (num2 >= 0));
					flag &= (num2 == 0);
					num++;
				}
				if (flag2 && flag && this.m_skippingFilter != null)
				{
					flag2 = this.m_skippingFilter.ShouldSkipCurrentRow();
					flag = !flag2;
				}
				if (flag2 && !flag)
				{
					if (advancedRowCursor)
					{
						this.m_skippedRowCount += 1L;
					}
					if (!this.ReadRowFromDataSet())
					{
						return false;
					}
					advancedRowCursor = true;
				}
			}
			return flag;
		}

		protected abstract void SetupRelationshipQueryRestart();

		protected void AddRelationshipRestartPosition(Relationship relationship, AspNetCore.ReportingServices.RdlExpressions.VariantResult[] primaryKeys)
		{
			if (relationship != null)
			{
				AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo[] foreignKeyExpressions = relationship.GetForeignKeyExpressions();
				if (foreignKeyExpressions != null && primaryKeys != null)
				{
					RelationshipRestartContext relationshipRestart = new RelationshipRestartContext(foreignKeyExpressions, primaryKeys, relationship.GetSortDirections(), this.m_idcDataSet);
					this.m_odpContext.QueryRestartInfo.AddRelationshipRestartPosition(this.m_idcDataSet, relationshipRestart);
				}
			}
		}

		[Conditional("DEBUG")]
		protected void AssertPrimaryKeysMatchForeignKeys(Relationship relationship, Array primaryKeys, Array foreignKeys)
		{
			if (!relationship.NaturalJoin || primaryKeys == null)
			{
				;
			}
		}

		public virtual void Close()
		{
			if (this.m_dataSource != null)
			{
				this.m_dataSource.RecordTimeDataRetrieval();
				this.m_dataSource.RecordSkippedRowCount(this.m_skippedRowCount);
				this.m_dataSource.Teardown();
				this.m_dataSource = null;
			}
		}

		public void Dispose()
		{
			this.Close();
		}
	}
}
