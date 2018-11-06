using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class TableRowList : ArrayList
	{
		internal new TableRow this[int index]
		{
			get
			{
				return (TableRow)base[index];
			}
		}

		internal TableRowList()
		{
		}

		internal TableRowList(int capacity)
			: base(capacity)
		{
		}

		internal void Register(InitializationContext context)
		{
			for (int i = 0; i < this.Count; i++)
			{
				Global.Tracer.Assert(null != this[i]);
				context.RegisterReportItems(this[i].ReportItems);
			}
		}

		internal void UnRegister(InitializationContext context)
		{
			for (int i = 0; i < this.Count; i++)
			{
				Global.Tracer.Assert(null != this[i]);
				context.UnRegisterReportItems(this[i].ReportItems);
			}
		}

		internal double GetHeightValue()
		{
			double num = 0.0;
			for (int i = 0; i < this.Count; i++)
			{
				if (!this[i].StartHidden)
				{
					num += this[i].HeightValue;
				}
			}
			return num;
		}
	}
}
