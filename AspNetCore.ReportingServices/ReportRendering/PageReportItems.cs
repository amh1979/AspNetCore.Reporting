using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class PageReportItems
	{
		private ArrayList m_innerArrayList = new ArrayList();

		public ReportItem this[int index]
		{
			get
			{
				if (0 <= index && index < this.Count)
				{
					return (ReportItem)this.m_innerArrayList[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
			set
			{
				if (0 <= index && index < this.Count)
				{
					this.m_innerArrayList[index] = value;
					return;
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public int Count
		{
			get
			{
				return this.m_innerArrayList.Count;
			}
		}

		public void Add(ReportItem value)
		{
			if (value != null)
			{
				this.m_innerArrayList.Add(value);
			}
		}

		public void Clear()
		{
			this.m_innerArrayList.Clear();
		}
	}
}
