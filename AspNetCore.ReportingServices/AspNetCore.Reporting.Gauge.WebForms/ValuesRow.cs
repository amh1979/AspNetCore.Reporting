using System;
using System.Data;
using System.Diagnostics;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[DebuggerStepThrough]
	internal class ValuesRow : DataRow
	{
		private ValuesDataTable tableValues;

		public DateTime DateStamp
		{
			get
			{
				return (DateTime)base[this.tableValues.DateStampColumn];
			}
			set
			{
				base[this.tableValues.DateStampColumn] = value;
			}
		}

		public double Value
		{
			get
			{
				try
				{
					return (double)base[this.tableValues.ValueColumn];
				}
				catch (InvalidCastException innerException)
				{
					throw new StrongTypingException(Utils.SRGetStr("ExceptionValueDbNull"), innerException);
				}
			}
			set
			{
				base[this.tableValues.ValueColumn] = value;
			}
		}

		internal ValuesRow(DataRowBuilder rb)
			: base(rb)
		{
			this.tableValues = (ValuesDataTable)base.Table;
		}

		public bool IsValueNull()
		{
			return base.IsNull(this.tableValues.ValueColumn);
		}

		public void SetValueNull()
		{
			base[this.tableValues.ValueColumn] = Convert.DBNull;
		}
	}
}
