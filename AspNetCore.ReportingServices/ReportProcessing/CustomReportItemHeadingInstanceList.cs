using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class CustomReportItemHeadingInstanceList : ArrayList
	{
		[NonSerialized]
		private CustomReportItemHeadingInstance m_lastHeadingInstance;

		internal new CustomReportItemHeadingInstance this[int index]
		{
			get
			{
				return (CustomReportItemHeadingInstance)base[index];
			}
		}

		internal CustomReportItemHeadingInstanceList()
		{
		}

		internal CustomReportItemHeadingInstanceList(int capacity)
			: base(capacity)
		{
		}

		internal void Add(CustomReportItemHeadingInstance headingInstance, ReportProcessing.ProcessingContext pc)
		{
			if (this.m_lastHeadingInstance != null)
			{
				this.m_lastHeadingInstance.HeadingSpan = headingInstance.HeadingCellIndex - this.m_lastHeadingInstance.HeadingCellIndex;
			}
			base.Add(headingInstance);
			this.m_lastHeadingInstance = headingInstance;
		}

		internal void SetLastHeadingSpan(int currentCellIndex, ReportProcessing.ProcessingContext pc)
		{
			if (this.m_lastHeadingInstance != null)
			{
				this.m_lastHeadingInstance.HeadingSpan = currentCellIndex - this.m_lastHeadingInstance.HeadingCellIndex;
			}
		}
	}
}
