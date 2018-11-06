using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class GaugePanel : DynamicImage, IStorable, IPersistable
	{
		private static Declaration m_declaration = GaugePanel.GetDeclaration();

		protected override byte ElementToken
		{
			get
			{
				return 14;
			}
		}

		protected override bool SpecialBorderHandling
		{
			get
			{
				return false;
			}
		}

		internal GaugePanel()
		{
		}

		internal GaugePanel(AspNetCore.ReportingServices.OnDemandReportRendering.GaugePanel source, PageContext pageContext)
			: base(source)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.GaugePanel gaugePanel = (AspNetCore.ReportingServices.OnDemandReportRendering.GaugePanel)base.m_source;
			base.m_pageBreakProperties = PageBreakProperties.Create(gaugePanel.PageBreak, this, pageContext);
			if (!pageContext.IgnorePageBreaks)
			{
				base.m_pageName = gaugePanel.Instance.PageName;
			}
			else if (pageContext.Common.DiagnosticsEnabled && gaugePanel.PageBreak.BreakLocation != 0)
			{
				pageContext.Common.TracePageBreakIgnored(this, pageContext.IgnorePageBreaksReason);
			}
		}

		protected override RPLItem CreateRPLItem()
		{
			return new RPLGaugePanel();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(GaugePanel.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false, string.Empty);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(GaugePanel.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false, string.Empty);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.GaugePanel;
		}

		internal new static Declaration GetDeclaration()
		{
			if (GaugePanel.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.GaugePanel, ObjectType.DynamicImage, memberInfoList);
			}
			return GaugePanel.m_declaration;
		}
	}
}
