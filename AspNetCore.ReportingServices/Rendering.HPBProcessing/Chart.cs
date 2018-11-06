using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Chart : DynamicImage, IStorable, IPersistable
	{
		private static Declaration m_declaration = Chart.GetDeclaration();

		private double m_dynamicWidth;

		private double m_dynamicHeight;

		public override int Size
		{
			get
			{
				return base.Size + 16;
			}
		}

		protected override byte ElementToken
		{
			get
			{
				return 11;
			}
		}

		protected override bool SpecialBorderHandling
		{
			get
			{
				return ((AspNetCore.ReportingServices.OnDemandReportRendering.Chart)base.m_source).SpecialBorderHandling;
			}
		}

		internal Chart()
		{
		}

		internal Chart(AspNetCore.ReportingServices.OnDemandReportRendering.Chart source, PageContext pageContext)
			: base(source)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Chart chart = (AspNetCore.ReportingServices.OnDemandReportRendering.Chart)base.m_source;
			ChartInstance chartInstance = (ChartInstance)base.m_source.Instance;
			base.m_pageBreakProperties = PageBreakProperties.Create(chart.PageBreak, this, pageContext);
			if (!pageContext.IgnorePageBreaks)
			{
				base.m_pageName = chartInstance.PageName;
			}
			else if (pageContext.Common.DiagnosticsEnabled && chart.PageBreak.BreakLocation != 0)
			{
				pageContext.Common.TracePageBreakIgnored(this, pageContext.IgnorePageBreaksReason);
			}
			this.m_dynamicWidth = chartInstance.DynamicWidth.ToMillimeters();
			this.m_dynamicHeight = chartInstance.DynamicHeight.ToMillimeters();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Chart.m_declaration);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.DynamicWidth:
					writer.Write(this.m_dynamicWidth);
					break;
				case MemberName.DynamicHeight:
					writer.Write(this.m_dynamicHeight);
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(Chart.m_declaration);
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.DynamicWidth:
					this.m_dynamicWidth = reader.ReadDouble();
					break;
				case MemberName.DynamicHeight:
					this.m_dynamicHeight = reader.ReadDouble();
					break;
				default:
					RSTrace.RenderingTracer.Assert(false, string.Empty);
					break;
				}
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.Chart;
		}

		internal new static Declaration GetDeclaration()
		{
			if (Chart.m_declaration == null)
			{
				List<MemberInfo> list = new List<MemberInfo>();
				list.Add(new MemberInfo(MemberName.DynamicWidth, Token.Double));
				list.Add(new MemberInfo(MemberName.DynamicHeight, Token.Double));
				return new Declaration(ObjectType.Chart, ObjectType.DynamicImage, list);
			}
			return Chart.m_declaration;
		}

		protected override RPLItem CreateRPLItem()
		{
			return new RPLChart();
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			base.m_itemPageSizes.AdjustWidthTo(this.m_dynamicWidth);
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			base.m_itemPageSizes.AdjustHeightTo(this.m_dynamicHeight);
		}
	}
}
