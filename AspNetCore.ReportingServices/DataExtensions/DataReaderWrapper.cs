using AspNetCore.ReportingServices.DataProcessing;
using System;
using System.Data;

namespace AspNetCore.ReportingServices.DataExtensions
{
	internal class DataReaderWrapper : BaseDataWrapper, AspNetCore.ReportingServices.DataProcessing.IDataReader, IDisposable
	{
		public virtual int FieldCount
		{
			get
			{
				return this.UnderlyingReader.FieldCount;
			}
		}

		public System.Data.IDataReader UnderlyingReader
		{
			get
			{
				return (System.Data.IDataReader)base.UnderlyingObject;
			}
		}

		public DataReaderWrapper(System.Data.IDataReader underlyingReader)
			: base(underlyingReader)
		{
		}

		public virtual string GetName(int fieldIndex)
		{
			return this.UnderlyingReader.GetName(fieldIndex);
		}

		public virtual int GetOrdinal(string fieldName)
		{
			int result = -1;
			if (fieldName != null)
			{
				try
				{
					result = this.UnderlyingReader.GetOrdinal(fieldName);
					return result;
				}
				catch (IndexOutOfRangeException)
				{
					return result;
				}
			}
			return result;
		}

		public virtual bool Read()
		{
			if (this.UnderlyingReader.Read())
			{
				return true;
			}
			this.UnderlyingReader.NextResult();
			return false;
		}

		public virtual Type GetFieldType(int fieldIndex)
		{
			return this.UnderlyingReader.GetFieldType(fieldIndex);
		}

		public virtual object GetValue(int fieldIndex)
		{
			return this.UnderlyingReader.GetValue(fieldIndex);
		}
	}
}
