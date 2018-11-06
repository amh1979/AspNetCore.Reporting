using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemHeadingList : TablixHeadingList
	{
		internal new CustomReportItemHeading this[int index]
		{
			get
			{
				return (CustomReportItemHeading)base[index];
			}
		}

		internal CustomReportItemHeadingList()
		{
		}

		internal CustomReportItemHeadingList(int capacity)
			: base(capacity)
		{
		}

		internal int Initialize(int level, DataCellsList dataRowCells, ref int currentIndex, ref int maxLevel, InitializationContext context)
		{
			int num = this.Count;
			int num2 = 0;
			for (int i = 0; i < num; i++)
			{
				Global.Tracer.Assert(null != this[i]);
				if (this[i].Initialize(level, this, i, dataRowCells, ref currentIndex, ref maxLevel, context))
				{
					num++;
					num2 += this[i].HeadingSpan;
				}
			}
			return num2;
		}

		internal void TransferHeadingAggregates()
		{
			int count = this.Count;
			for (int i = 0; i < count; i++)
			{
				Global.Tracer.Assert(null != this[i]);
				this[i].TransferHeadingAggregates();
			}
		}

		internal override TablixHeadingList InnerHeadings()
		{
			if (this.Count > 0)
			{
				return this[0].InnerHeadings;
			}
			return null;
		}
	}
}
