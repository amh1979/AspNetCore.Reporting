using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartHeadingInstanceList : ArrayList
	{
		[NonSerialized]
		private ChartHeadingInstance m_lastHeadingInstance;

		internal new ChartHeadingInstance this[int index]
		{
			get
			{
				return (ChartHeadingInstance)base[index];
			}
		}

		internal ChartHeadingInstanceList()
		{
		}

		internal ChartHeadingInstanceList(int capacity)
			: base(capacity)
		{
		}

		internal void Add(ChartHeadingInstance chartHeadingInstance, ReportProcessing.ProcessingContext pc)
		{
			if (this.m_lastHeadingInstance != null)
			{
				this.m_lastHeadingInstance.InstanceInfo.HeadingSpan = chartHeadingInstance.InstanceInfo.HeadingCellIndex - this.m_lastHeadingInstance.InstanceInfo.HeadingCellIndex;
				pc.ChunkManager.AddInstance(this.m_lastHeadingInstance.InstanceInfo, this.m_lastHeadingInstance, pc.InPageSection);
			}
			base.Add(chartHeadingInstance);
			this.m_lastHeadingInstance = chartHeadingInstance;
		}

		internal void SetLastHeadingSpan(int currentCellIndex, ReportProcessing.ProcessingContext pc)
		{
			if (this.m_lastHeadingInstance != null)
			{
				this.m_lastHeadingInstance.InstanceInfo.HeadingSpan = currentCellIndex - this.m_lastHeadingInstance.InstanceInfo.HeadingCellIndex;
				pc.ChunkManager.AddInstance(this.m_lastHeadingInstance.InstanceInfo, this.m_lastHeadingInstance, pc.InPageSection);
			}
		}
	}
}
