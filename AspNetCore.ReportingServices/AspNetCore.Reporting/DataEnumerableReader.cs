using AspNetCore.ReportingServices.DataProcessing;
using System;
using System.Collections;
using System.ComponentModel;

namespace AspNetCore.Reporting
{
	internal class DataEnumerableReader : IDataReader, IDisposable
	{
		private IEnumerator m_dataEnumerator;

		private PropertyDescriptorCollection m_columns;

		private object m_row;

		private bool m_firstRow;

		public int FieldCount
		{
			get
			{
				if (this.m_columns == null)
				{
					return 0;
				}
				return this.m_columns.Count;
			}
		}

		internal DataEnumerableReader(IEnumerable dataEnumerable)
		{
			this.m_dataEnumerator = dataEnumerable.GetEnumerator();
			if (this.m_dataEnumerator != null && this.m_dataEnumerator.MoveNext())
			{
				this.m_firstRow = true;
				this.m_row = this.m_dataEnumerator.Current;
				this.m_columns = TypeDescriptor.GetProperties(this.m_row);
			}
		}

		public string GetName(int fieldIndex)
		{
			return this.m_columns[fieldIndex].Name;
		}

		public int GetOrdinal(string fieldName)
		{
			int result = -1;
			if (fieldName != null && this.m_columns != null && this.m_columns.Count > 0)
			{
				PropertyDescriptor propertyDescriptor = this.m_columns[fieldName];
				if (propertyDescriptor != null)
				{
					result = this.m_columns.IndexOf(propertyDescriptor);
				}
			}
			return result;
		}

		public Type GetFieldType(int fieldIndex)
		{
			return this.m_columns[fieldIndex].PropertyType;
		}

		public bool Read()
		{
			if (this.m_firstRow)
			{
				this.m_firstRow = false;
				return true;
			}
			if (this.m_dataEnumerator == null)
			{
				return false;
			}
			if (this.m_dataEnumerator.MoveNext())
			{
				this.m_row = this.m_dataEnumerator.Current;
				return true;
			}
			return false;
		}

		public object GetValue(int fieldIndex)
		{
			return this.m_columns[fieldIndex].GetValue(this.m_row);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}
