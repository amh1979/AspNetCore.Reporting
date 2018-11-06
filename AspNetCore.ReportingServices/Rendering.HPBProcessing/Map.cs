using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Map : DynamicImage, IStorable, IPersistable
	{
		private static Declaration m_declaration = Map.GetDeclaration();

		protected override byte ElementToken
		{
			get
			{
				return 21;
			}
		}

		protected override bool SpecialBorderHandling
		{
			get
			{
				return ((AspNetCore.ReportingServices.OnDemandReportRendering.Map)base.m_source).SpecialBorderHandling;
			}
		}

		internal Map()
		{
		}

		internal Map(AspNetCore.ReportingServices.OnDemandReportRendering.Map source, PageContext pageContext)
			: base(source)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Map map = (AspNetCore.ReportingServices.OnDemandReportRendering.Map)base.m_source;
			base.m_pageBreakProperties = PageBreakProperties.Create(map.PageBreak, this, pageContext);
			if (!pageContext.IgnorePageBreaks)
			{
				base.m_pageName = map.Instance.PageName;
			}
			else if (pageContext.Common.DiagnosticsEnabled && map.PageBreak.BreakLocation != 0)
			{
				pageContext.Common.TracePageBreakIgnored(this, pageContext.IgnorePageBreaksReason);
			}
		}

		protected override RPLItem CreateRPLItem()
		{
			return new RPLMap();
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Map.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(Map.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.Map;
		}

		internal new static Declaration GetDeclaration()
		{
			if (Map.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.Map, ObjectType.DynamicImage, memberInfoList);
			}
			return Map.m_declaration;
		}
	}
}
