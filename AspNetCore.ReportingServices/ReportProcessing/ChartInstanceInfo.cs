using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;
using System.Threading;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartInstanceInfo : ReportItemInstanceInfo
	{
		private AxisInstance m_categoryAxis;

		private AxisInstance m_valueAxis;

		private ChartTitleInstance m_title;

		private object[] m_plotAreaStyleAttributeValues;

		private object[] m_legendStyleAttributeValues;

		private string m_cultureName;

		private string m_noRows;

		internal AxisInstance CategoryAxis
		{
			get
			{
				return this.m_categoryAxis;
			}
			set
			{
				this.m_categoryAxis = value;
			}
		}

		internal AxisInstance ValueAxis
		{
			get
			{
				return this.m_valueAxis;
			}
			set
			{
				this.m_valueAxis = value;
			}
		}

		internal ChartTitleInstance Title
		{
			get
			{
				return this.m_title;
			}
			set
			{
				this.m_title = value;
			}
		}

		internal object[] PlotAreaStyleAttributeValues
		{
			get
			{
				return this.m_plotAreaStyleAttributeValues;
			}
			set
			{
				this.m_plotAreaStyleAttributeValues = value;
			}
		}

		internal object[] LegendStyleAttributeValues
		{
			get
			{
				return this.m_legendStyleAttributeValues;
			}
			set
			{
				this.m_legendStyleAttributeValues = value;
			}
		}

		internal string CultureName
		{
			get
			{
				return this.m_cultureName;
			}
			set
			{
				this.m_cultureName = value;
			}
		}

		internal string NoRows
		{
			get
			{
				return this.m_noRows;
			}
			set
			{
				this.m_noRows = value;
			}
		}

		internal ChartInstanceInfo(ReportProcessing.ProcessingContext pc, Chart reportItemDef, ChartInstance owner)
			: base(pc, reportItemDef, owner, true)
		{
			if (reportItemDef.Title != null)
			{
				this.m_title = new ChartTitleInstance(pc, reportItemDef, reportItemDef.Title, "Title");
			}
			if (reportItemDef.CategoryAxis != null)
			{
				this.m_categoryAxis = new AxisInstance(pc, reportItemDef, reportItemDef.CategoryAxis, Axis.Mode.CategoryAxis);
			}
			if (reportItemDef.ValueAxis != null)
			{
				this.m_valueAxis = new AxisInstance(pc, reportItemDef, reportItemDef.ValueAxis, Axis.Mode.ValueAxis);
			}
			if (reportItemDef.Legend != null)
			{
				this.m_legendStyleAttributeValues = Chart.CreateStyle(pc, reportItemDef.Legend.StyleClass, reportItemDef.Name + ".Legend", owner.UniqueName);
			}
			if (reportItemDef.PlotArea != null)
			{
				this.m_plotAreaStyleAttributeValues = Chart.CreateStyle(pc, reportItemDef.PlotArea.StyleClass, reportItemDef.Name + ".PlotArea", owner.UniqueName);
			}
			this.SaveChartCulture();
			this.m_noRows = pc.ReportRuntime.EvaluateDataRegionNoRowsExpression(reportItemDef, reportItemDef.ObjectType, reportItemDef.Name, "NoRows");
		}

		internal ChartInstanceInfo(Chart reportItemDef)
			: base(reportItemDef)
		{
		}

		private void SaveChartCulture()
		{
			if (base.m_reportItemDef.StyleClass != null && base.m_reportItemDef.StyleClass.StyleAttributes != null)
			{
				AttributeInfo attributeInfo = base.m_reportItemDef.StyleClass.StyleAttributes["Language"];
				if (attributeInfo != null)
				{
					if (attributeInfo.IsExpression)
					{
						this.m_cultureName = (string)base.m_styleAttributeValues[attributeInfo.IntValue];
					}
					else
					{
						this.m_cultureName = attributeInfo.Value;
					}
				}
			}
			if (this.m_cultureName == null)
			{
				this.m_cultureName = Thread.CurrentThread.CurrentCulture.Name;
			}
		}

		internal new static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.CategoryAxis, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.AxisInstance));
			memberInfoList.Add(new MemberInfo(MemberName.ValueAxis, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.AxisInstance));
			memberInfoList.Add(new MemberInfo(MemberName.Title, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.ChartTitleInstance));
			memberInfoList.Add(new MemberInfo(MemberName.PlotAreaStyleAttributeValues, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.LegendStyleAttributeValues, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			memberInfoList.Add(new MemberInfo(MemberName.CultureName, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.NoRows, Token.String));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
