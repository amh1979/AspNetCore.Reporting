using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartDataPointInstance : InstanceInfoOwner
	{
		private int m_uniqueName;

		internal int UniqueName
		{
			get
			{
				return this.m_uniqueName;
			}
			set
			{
				this.m_uniqueName = value;
			}
		}

		internal ChartDataPointInstanceInfo InstanceInfo
		{
			get
			{
				if (base.m_instanceInfo is OffsetInfo)
				{
					Global.Tracer.Assert(false, string.Empty);
					return null;
				}
				return (ChartDataPointInstanceInfo)base.m_instanceInfo;
			}
		}

		internal ChartDataPointInstance(ReportProcessing.ProcessingContext pc, Chart chart, ChartDataPoint dataPointDef, int dataPointIndex)
		{
			this.m_uniqueName = pc.CreateUniqueName();
			base.m_instanceInfo = new ChartDataPointInstanceInfo(pc, chart, dataPointDef, dataPointIndex, this);
		}

		internal ChartDataPointInstance()
		{
		}

		internal ChartDataPointInstanceInfo GetInstanceInfo(ChunkManager.RenderingChunkManager chunkManager, ChartDataPointList chartDataPoints)
		{
			if (base.m_instanceInfo is OffsetInfo)
			{
				Global.Tracer.Assert(null != chunkManager);
				IntermediateFormatReader reader = chunkManager.GetReader(((OffsetInfo)base.m_instanceInfo).Offset);
				return reader.ReadChartDataPointInstanceInfo(chartDataPoints);
			}
			return (ChartDataPointInstanceInfo)base.m_instanceInfo;
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.InstanceInfoOwner, memberInfoList);
		}
	}
}
