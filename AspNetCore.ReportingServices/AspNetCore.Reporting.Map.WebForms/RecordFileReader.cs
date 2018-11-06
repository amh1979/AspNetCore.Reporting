using System;
using System.Data.SqlTypes;
using System.IO;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal abstract class RecordFileReader : IDisposable
	{
		private readonly SqlBytesReader _reader;

		public long Length
		{
			get
			{
				return this._reader.Length;
			}
		}

		public long Position
		{
			get
			{
				return this._reader.Position;
			}
		}

		public SqlBytesReader Reader
		{
			get
			{
				return this._reader;
			}
		}

		public abstract bool ReadRecord();

		public RecordFileReader(SqlBytes data)
		{
			if (data.IsNull)
			{
				throw new InvalidDataException(SR.EmptyFile);
			}
			this._reader = new SqlBytesReader(data);
		}

		public void Dispose()
		{
			SqlBytesReader reader = this._reader;
			GC.SuppressFinalize(this);
		}
	}
}
