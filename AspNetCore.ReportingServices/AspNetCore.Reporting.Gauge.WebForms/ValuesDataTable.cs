using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[Serializable]
	[DebuggerStepThrough]
	internal sealed class ValuesDataTable : DataTable, IEnumerable, ISerializable
	{
		private const string DATESTAMP_COLUMN_SERIALIZATION_ID = "columnDateStamp";

		private const string VALUE_COLUMN_SERIALIZATION_ID = "columnValue";

		private DataColumn columnDateStamp;

		private DataColumn columnValue;

		[Browsable(false)]
		public int Count
		{
			get
			{
				return base.Rows.Count;
			}
		}

		internal DataColumn DateStampColumn
		{
			get
			{
				return this.columnDateStamp;
			}
		}

		internal DataColumn ValueColumn
		{
			get
			{
				return this.columnValue;
			}
		}

		public ValuesRow this[int index]
		{
			get
			{
				return (ValuesRow)base.Rows[index];
			}
		}

		public event ValuesRowChangeEventHandler ValuesRowChanged;

		public event ValuesRowChangeEventHandler ValuesRowChanging;

		public event ValuesRowChangeEventHandler ValuesRowDeleted;

		public event ValuesRowChangeEventHandler ValuesRowDeleting;

		internal ValuesDataTable()
			: base("Values")
		{
			this.InitClass();
		}

		internal ValuesDataTable(DataTable table)
			: base(table.TableName)
		{
			if (table.CaseSensitive != table.DataSet.CaseSensitive)
			{
				base.CaseSensitive = table.CaseSensitive;
			}
			if (table.Locale.ToString() != table.DataSet.Locale.ToString())
			{
				base.Locale = table.Locale;
			}
			if (table.Namespace != table.DataSet.Namespace)
			{
				base.Namespace = table.Namespace;
			}
			base.Prefix = table.Prefix;
			base.MinimumCapacity = table.MinimumCapacity;
			base.DisplayExpression = table.DisplayExpression;
		}

		private ValuesDataTable(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			string text = (string)info.GetValue("columnDateStamp", typeof(string));
			if (!string.IsNullOrEmpty(text) && base.Columns.IndexOf(text) > -1)
			{
				this.columnDateStamp = base.Columns[text];
			}
			text = (string)info.GetValue("columnValue", typeof(string));
			if (!string.IsNullOrEmpty(text) && base.Columns.IndexOf(text) > -1)
			{
				this.columnValue = base.Columns[text];
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("columnDateStamp", (this.columnDateStamp != null) ? this.columnDateStamp.ColumnName : "");
			info.AddValue("columnValue", (this.columnValue != null) ? this.columnValue.ColumnName : "");
		}

		public void AddValuesRow(ValuesRow row)
		{
			base.Rows.Add(row);
		}

		public ValuesRow AddValuesRow(DateTime dateStamp, double value)
		{
			ValuesRow valuesRow = (ValuesRow)base.NewRow();
			valuesRow.ItemArray = new object[2]
			{
				dateStamp,
				value
			};
			base.Rows.Add(valuesRow);
			return valuesRow;
		}

		public ValuesRow FindByDateStamp(DateTime dateStamp)
		{
			return (ValuesRow)base.Rows.Find(new object[1]
			{
				dateStamp
			});
		}

		public IEnumerator GetEnumerator()
		{
			return base.Rows.GetEnumerator();
		}

		public override DataTable Clone()
		{
			ValuesDataTable valuesDataTable = (ValuesDataTable)base.Clone();
			valuesDataTable.InitVars();
			return valuesDataTable;
		}

		protected override DataTable CreateInstance()
		{
			return new ValuesDataTable();
		}

		internal void InitVars()
		{
			this.columnDateStamp = base.Columns["DateStamp"];
			this.columnValue = base.Columns["Value"];
		}

		private void InitClass()
		{
			this.columnDateStamp = new DataColumn("DateStamp", typeof(DateTime), null, MappingType.Element);
			base.Columns.Add(this.columnDateStamp);
			this.columnValue = new DataColumn("Value", typeof(double), null, MappingType.Element);
			base.Columns.Add(this.columnValue);
			base.Constraints.Add(new UniqueConstraint("GaugeDataKey", new DataColumn[1]
			{
				this.columnDateStamp
			}, true));
			this.columnDateStamp.AllowDBNull = false;
			this.columnDateStamp.Unique = true;
		}

		public ValuesRow NewValuesRow()
		{
			return (ValuesRow)base.NewRow();
		}

		protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
		{
			return new ValuesRow(builder);
		}

		protected override Type GetRowType()
		{
			return typeof(ValuesRow);
		}

		protected override void OnRowChanged(DataRowChangeEventArgs e)
		{
			base.OnRowChanged(e);
			if (this.ValuesRowChanged != null)
			{
				this.ValuesRowChanged(this, new ValuesRowChangeEventArgs((ValuesRow)e.Row, e.Action));
			}
		}

		protected override void OnRowChanging(DataRowChangeEventArgs e)
		{
			base.OnRowChanging(e);
			if (this.ValuesRowChanging != null)
			{
				this.ValuesRowChanging(this, new ValuesRowChangeEventArgs((ValuesRow)e.Row, e.Action));
			}
		}

		protected override void OnRowDeleted(DataRowChangeEventArgs e)
		{
			base.OnRowDeleted(e);
			if (this.ValuesRowDeleted != null)
			{
				this.ValuesRowDeleted(this, new ValuesRowChangeEventArgs((ValuesRow)e.Row, e.Action));
			}
		}

		protected override void OnRowDeleting(DataRowChangeEventArgs e)
		{
			base.OnRowDeleting(e);
			if (this.ValuesRowDeleting != null)
			{
				this.ValuesRowDeleting(this, new ValuesRowChangeEventArgs((ValuesRow)e.Row, e.Action));
			}
		}

		public void RemoveValuesRow(ValuesRow row)
		{
			base.Rows.Remove(row);
		}
	}
}
