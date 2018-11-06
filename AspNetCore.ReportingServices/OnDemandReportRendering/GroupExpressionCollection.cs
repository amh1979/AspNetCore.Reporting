using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class GroupExpressionCollection : ReportElementCollectionBase<ReportVariantProperty>
	{
		private List<ReportVariantProperty> m_list;

		public override ReportVariantProperty this[int index]
		{
			get
			{
				if (index >= 0 && index < this.Count)
				{
					return this.m_list[index];
				}
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterRange, index, 0, this.Count);
			}
		}

		public override int Count
		{
			get
			{
				return this.m_list.Count;
			}
		}

		internal GroupExpressionCollection()
		{
			this.m_list = new List<ReportVariantProperty>();
		}

		internal GroupExpressionCollection(AspNetCore.ReportingServices.ReportIntermediateFormat.Grouping grouping)
		{
			if (grouping == null || grouping.GroupExpressions == null)
			{
				this.m_list = new List<ReportVariantProperty>();
			}
			else
			{
				int count = grouping.GroupExpressions.Count;
				this.m_list = new List<ReportVariantProperty>(count);
				for (int i = 0; i < count; i++)
				{
					this.m_list.Add(new ReportVariantProperty(grouping.GroupExpressions[i]));
				}
			}
		}

		internal GroupExpressionCollection(AspNetCore.ReportingServices.ReportProcessing.Grouping grouping)
		{
			if (grouping == null || grouping.GroupExpressions == null)
			{
				this.m_list = new List<ReportVariantProperty>();
			}
			else
			{
				int count = grouping.GroupExpressions.Count;
				this.m_list = new List<ReportVariantProperty>(count);
				for (int i = 0; i < count; i++)
				{
					this.m_list.Add(new ReportVariantProperty(grouping.GroupExpressions[i]));
				}
			}
		}
	}
}
