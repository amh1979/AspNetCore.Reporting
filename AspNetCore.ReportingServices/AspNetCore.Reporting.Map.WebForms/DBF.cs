using System;
using System.Data;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class DBF : RecordFileReader, IDisposable
	{
		private struct DBFColumnHeader
		{
			private string _fieldName;

			private char _fieldType;

			private byte _fieldLength;

			private byte _decimalCount;

			private string _value;

			public object Value
			{
				get
				{
					if (this._value.Length == 0 && this._fieldType != 'C')
					{
						return null;
					}
					try
					{
						switch (this._fieldType)
						{
						case 'D':
							return DateTime.ParseExact(this._value, "yyyyMMdd", CultureInfo.InvariantCulture);
						case 'N':
							return decimal.Parse(this._value, CultureInfo.InvariantCulture);
						case 'L':
							if (this._value == "T")
							{
								return true;
							}
							return false;
						case '+':
						case 'I':
							return int.Parse(this._value, CultureInfo.InvariantCulture);
						case 'F':
							return float.Parse(this._value, CultureInfo.InvariantCulture);
						case 'O':
							return double.Parse(this._value, CultureInfo.InvariantCulture);
						default:
							return this._value;
						}
					}
					catch
					{
						return null;
					}
				}
			}

			public string FieldName
			{
				get
				{
					return this._fieldName;
				}
			}

			public Type FieldType
			{
				get
				{
					switch (this._fieldType)
					{
					case 'D':
						return typeof(DateTime);
					case 'N':
						return typeof(decimal);
					case 'L':
						return typeof(bool);
					case '+':
					case 'I':
						return typeof(int);
					case 'F':
						return typeof(float);
					case 'O':
						return typeof(double);
					default:
						return typeof(string);
					}
				}
			}

			public void ReadHeader(SqlBytesReader reader)
			{
				char[] array = reader.ReadChars(11);
				int i;
				for (i = 0; i < array.Length && array[i] != 0; i++)
				{
				}
				this._fieldName = new string(array, 0, i);
				this._fieldType = reader.ReadChar();
				reader.ReadUInt32();
				this._fieldLength = reader.ReadByte();
				this._decimalCount = reader.ReadByte();
				reader.ReadUInt16();
				reader.ReadByte();
				reader.ReadUInt16();
				reader.ReadByte();
				reader.ReadUInt32();
				reader.ReadUInt16();
				reader.ReadByte();
				reader.ReadByte();
				if (this._fieldType == 'C')
				{
					if (this._fieldLength < 255)
					{
						return;
					}
					throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture.NumberFormat, SR.UnsupportedDbfFieldLength, this._fieldType, this._fieldLength));
				}
				if (this._fieldType == 'N')
				{
					if (this._fieldLength <= 20)
					{
						return;
					}
					throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture.NumberFormat, SR.UnsupportedDbfFieldLength, this._fieldType, this._fieldLength));
				}
				if (this._fieldType == 'L')
				{
					if (this._fieldLength == 1)
					{
						return;
					}
					throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture.NumberFormat, SR.UnsupportedDbfFieldLength, this._fieldType, this._fieldLength));
				}
				if (this._fieldType == 'D')
				{
					if (this._fieldLength == 8)
					{
						return;
					}
					throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture.NumberFormat, SR.UnsupportedDbfFieldLength, this._fieldType, this._fieldLength));
				}
				if (this._fieldType == 'F')
				{
					if (this._fieldLength <= 20)
					{
						return;
					}
					throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture.NumberFormat, SR.UnsupportedDbfFieldLength, this._fieldType, this._fieldLength));
				}
				throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture.NumberFormat, SR.UnsupportedDbfField, this._fieldType));
			}

			public void ReadFieldData(SqlBytesReader reader)
			{
				char[] value = reader.ReadChars(this._fieldLength);
				this._value = new string(value).Trim();
			}
		}

		private byte _version;

		private uint _numberOfRecords;

		private ushort _headerLength;

		private ushort _recordLength;

		private bool _dataOK;

		private DBFColumnHeader[] _fields;

		public DBF(SqlBytes data)
			: base(data)
		{
			this.ReadHeader();
		}

		protected long ReadHeader()
		{
			this._dataOK = false;
			this._version = base.Reader.ReadByte();
			if ((this._version & 7) != 3)
			{
				throw new InvalidDataException(SR.UnsupportedDbfFormat);
			}
			base.Reader.ReadByte();
			base.Reader.ReadByte();
			base.Reader.ReadByte();
			this._numberOfRecords = base.Reader.ReadUInt32();
			if (this._numberOfRecords >= 1000000000)
			{
				throw new InvalidDataException(SR.UnsupportedDbfNumberOfRecords);
			}
			this._headerLength = base.Reader.ReadUInt16();
			this._recordLength = base.Reader.ReadUInt16();
			if (this._recordLength > 4000)
			{
				throw new InvalidDataException(SR.UnsupportedDbfLongRecords);
			}
			base.Reader.ReadUInt16();
			if (base.Reader.ReadByte() != 0)
			{
				throw new InvalidDataException(SR.UnsupportedDbfTransactions);
			}
			if (base.Reader.ReadByte() != 0)
			{
				throw new InvalidDataException(SR.UnsupportedDbfEncryption);
			}
			base.Reader.ReadUInt32();
			base.Reader.ReadUInt32();
			base.Reader.ReadUInt32();
			base.Reader.ReadByte();
			base.Reader.ReadByte();
			base.Reader.ReadUInt16();
			int num = (this._headerLength - 1) / 32 - 1;
			if (num < 1)
			{
				throw new InvalidDataException(SR.UnsupportedDbfNumberOfFields0);
			}
			this._fields = new DBFColumnHeader[num];
			for (int i = 0; i < num; i++)
			{
				this._fields[i].ReadHeader(base.Reader);
			}
			if (base.Reader.ReadByte() != 13)
			{
				throw new InvalidDataException(SR.UnsupportedDbfHeader);
			}
			return this._numberOfRecords;
		}

		public override bool ReadRecord()
		{
			this._dataOK = false;
			if (base.Reader.EOF)
			{
				return false;
			}
			switch (base.Reader.ReadByte())
			{
			case 26:
				return false;
			case 42:
				throw new InvalidDataException(SR.UnsupportedDbfDeleted);
			default:
				throw new InvalidDataException(SR.UnsupportedDbfRecordFlag);
			case 32:
				for (int i = 0; i < this._fields.Length; i++)
				{
					this._fields[i].ReadFieldData(base.Reader);
				}
				this._dataOK = true;
				return true;
			}
		}

		public DataTable GetDataTable()
		{
			DataTable dataTable = new DataTable();
			dataTable.Locale = CultureInfo.CurrentCulture;
			DBFColumnHeader[] fields = this._fields;
			for (int i = 0; i < fields.Length; i++)
			{
				DBFColumnHeader dBFColumnHeader = fields[i];
				dataTable.Columns.Add(dBFColumnHeader.FieldName, dBFColumnHeader.FieldType);
			}
			dataTable.BeginLoadData();
			while (this.ReadRecord())
			{
				object[] array = new object[this._fields.Length];
				for (int j = 0; j < this._fields.Length; j++)
				{
					array[j] = this._fields[j].Value;
				}
				dataTable.LoadDataRow(array, true);
			}
			dataTable.EndLoadData();
			return dataTable;
		}

		public SqlXml GetXML()
		{
			SqlXml @null = SqlXml.Null;
			if (!this._dataOK)
			{
				return @null;
			}
			StringBuilder stringBuilder = new StringBuilder(4000);
			using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder))
			{
				xmlWriter.WriteStartElement("shapeData");
				for (int i = 0; i < this._fields.Length; i++)
				{
					xmlWriter.WriteElementString(XmlConvert.EncodeName(this._fields[i].FieldName), XmlConvert.EncodeName(this._fields[i].Value.ToString()));
				}
				xmlWriter.WriteEndElement();
				xmlWriter.Close();
			}
			string s = stringBuilder.ToString();
			using (StringReader input = new StringReader(s))
			{
				using (XmlReader value = XmlReader.Create(input))
				{
					return new SqlXml(value);
				}
			}
		}

		void IDisposable.Dispose()
		{
			base.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
