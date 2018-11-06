using AspNetCore.ReportingServices.DataProcessing;
using System;
using System.Data;

namespace AspNetCore.Reporting
{
	internal class DataTableReader : AspNetCore.ReportingServices.DataProcessing.IDataReader, IDisposable
	{
		private int m_currentRowNumber = -1;

		private DataTable m_dataTable;

		public int FieldCount
		{
			get
			{
				if (this.m_dataTable.Columns == null)
				{
					return 0;
				}
				return this.m_dataTable.Columns.Count;
			}
		}

		internal DataTableReader(DataTable dataTable)
		{
			this.m_dataTable = dataTable;
		}

		public string GetName(int fieldIndex)
		{
			return this.m_dataTable.Columns[fieldIndex].ColumnName;
		}

		public int GetOrdinal(string fieldName)
		{
			int result = -1;
			if (fieldName != null && this.m_dataTable.Columns != null && this.m_dataTable.Columns.Count > 0)
			{
				DataColumn dataColumn = this.m_dataTable.Columns[fieldName];
				if (dataColumn != null)
				{
					result = dataColumn.Ordinal;
				}
			}
			return result;
		}

		public Type GetFieldType(int fieldIndex)
		{
			return this.m_dataTable.Columns[fieldIndex].GetType();
		}

		public bool Read()
		{
			if (this.m_dataTable.Rows == null)
			{
				return false;
			}
			this.m_currentRowNumber++;
			if (this.m_currentRowNumber < this.m_dataTable.Rows.Count)
			{
				return true;
			}
			return false;
		}

		public object GetValue(int fieldIndex)
		{
			return this.m_dataTable.Rows[this.m_currentRowNumber][fieldIndex];
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
