using AspNetCore.ReportingServices.ReportProcessing.Persistence;
using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ChartTitleInstance
	{
		private int m_uniqueName;

		private string m_caption;

		private object[] m_styleAttributeValues;

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

		internal string Caption
		{
			get
			{
				return this.m_caption;
			}
			set
			{
				this.m_caption = value;
			}
		}

		internal object[] StyleAttributeValues
		{
			get
			{
				return this.m_styleAttributeValues;
			}
			set
			{
				this.m_styleAttributeValues = value;
			}
		}

		internal ChartTitleInstance(ReportProcessing.ProcessingContext pc, Chart chart, ChartTitle titleDef, string propertyName)
		{
			this.m_uniqueName = pc.CreateUniqueName();
			this.m_caption = pc.ReportRuntime.EvaluateChartTitleCaptionExpression(titleDef, chart.Name, propertyName);
			this.m_styleAttributeValues = Chart.CreateStyle(pc, titleDef.StyleClass, chart.Name + "." + propertyName, this.m_uniqueName);
		}

		internal ChartTitleInstance()
		{
		}

		internal static Declaration GetDeclaration()
		{
			MemberInfoList memberInfoList = new MemberInfoList();
			memberInfoList.Add(new MemberInfo(MemberName.UniqueName, Token.Int32));
			memberInfoList.Add(new MemberInfo(MemberName.Caption, Token.String));
			memberInfoList.Add(new MemberInfo(MemberName.StyleAttributeValues, Token.Array, AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.Variant));
			return new Declaration(AspNetCore.ReportingServices.ReportProcessing.Persistence.ObjectType.None, memberInfoList);
		}
	}
}
