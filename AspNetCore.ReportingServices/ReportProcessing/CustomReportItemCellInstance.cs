using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemCellInstance
	{
		private int m_rowIndex;

		private int m_columnIndex;

		private DataValueInstanceList m_dataValueInstances;

		internal int RowIndex
		{
			get
			{
				return this.m_rowIndex;
			}
			set
			{
				this.m_rowIndex = value;
			}
		}

		internal int ColumnIndex
		{
			get
			{
				return this.m_columnIndex;
			}
			set
			{
				this.m_columnIndex = value;
			}
		}

		internal DataValueInstanceList DataValueInstances
		{
			get
			{
				return this.m_dataValueInstances;
			}
			set
			{
				this.m_dataValueInstances = value;
			}
		}

		internal CustomReportItemCellInstance(int rowIndex, int colIndex, CustomReportItem definition, ReportProcessing.ProcessingContext pc)
		{
			this.m_rowIndex = rowIndex;
			this.m_columnIndex = colIndex;
			Global.Tracer.Assert(definition != null && definition.DataRowCells != null && rowIndex < definition.DataRowCells.Count && colIndex < definition.DataRowCells[rowIndex].Count && 0 < definition.DataRowCells[rowIndex][colIndex].Count);
			DataValueCRIList dataValueCRIList = definition.DataRowCells[rowIndex][colIndex];
			Global.Tracer.Assert(null != dataValueCRIList);
			this.m_dataValueInstances = dataValueCRIList.EvaluateExpressions(definition.ObjectType, definition.Name, null, dataValueCRIList.RDLRowIndex, dataValueCRIList.RDLColumnIndex, pc);
			Global.Tracer.Assert(null != this.m_dataValueInstances);
		}

		internal CustomReportItemCellInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.RowIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.ColumnIndex, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.DataValueInstances, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.DataValueInstanceList));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
