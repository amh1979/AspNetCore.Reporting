using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class Rectangle : PageItemContainer
	{
		private static Declaration m_declaration = Rectangle.GetDeclaration();

		internal override byte RPLFormatType
		{
			get
			{
				return 10;
			}
		}

		internal Rectangle()
		{
		}

		internal Rectangle(AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle source, PageContext pageContext)
			: base(source)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle rectangle = (AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)base.m_source;
			RectangleInstance rectangleInstance = (RectangleInstance)base.m_source.Instance;
			base.m_itemPageSizes = new ItemSizes(source);
			base.m_pageBreakProperties = PageBreakProperties.Create(rectangle.PageBreak, this, pageContext);
			if (!pageContext.IgnorePageBreaks)
			{
				base.m_pageName = rectangleInstance.PageName;
			}
			else if (pageContext.Common.DiagnosticsEnabled && rectangle.PageBreak.BreakLocation != 0)
			{
				pageContext.Common.TracePageBreakIgnored(this, pageContext.IgnorePageBreaksReason);
			}
			base.KeepTogetherHorizontal = source.KeepTogether;
			base.KeepTogetherVertical = source.KeepTogether;
			bool unresolvedKTV = base.UnresolvedKTH = source.KeepTogether;
			base.UnresolvedKTV = unresolvedKTV;
			bool unresolvedPBE = base.UnresolvedPBS = true;
			base.UnresolvedPBE = unresolvedPBE;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(Rectangle.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false, string.Empty);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(Rectangle.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false, string.Empty);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.Rectangle;
		}

		internal new static Declaration GetDeclaration()
		{
			if (Rectangle.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.Rectangle, ObjectType.PageItemContainer, memberInfoList);
			}
			return Rectangle.m_declaration;
		}

		protected override void CreateChildren(PageContext pageContext)
		{
			base.CreateChildren(((AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)base.m_source).ReportItemCollection, pageContext);
		}

		internal override void OmitBorderOnPageBreak(RPLWriter rplWriter, double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
			if (rplWriter != null && ((AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)base.m_source).OmitBorderOnPageBreak)
			{
				base.OmitBorderOnPageBreak(pageLeft, pageTop, pageRight, pageBottom);
			}
		}

		internal override RPLElement CreateRPLElement()
		{
			return new RPLRectangle();
		}

		internal override RPLElement CreateRPLElement(RPLElementProps props, PageContext pageContext)
		{
			RPLItemProps rplElementProps = props as RPLItemProps;
			return new RPLRectangle(rplElementProps);
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle rectangle = (AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)base.m_source;
			ReportItemCollection reportItemCollection = rectangle.ReportItemCollection;
			if (rectangle.LinkToChild >= 0 && rectangle.LinkToChild < reportItemCollection.Count)
			{
				ReportItem reportItem = ((ReportElementCollectionBase<ReportItem>)reportItemCollection)[rectangle.LinkToChild];
				spbifWriter.Write((byte)43);
				spbifWriter.Write(reportItem.ID);
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle rectangle = (AspNetCore.ReportingServices.OnDemandReportRendering.Rectangle)base.m_source;
			ReportItemCollection reportItemCollection = rectangle.ReportItemCollection;
			if (rectangle.LinkToChild >= 0 && rectangle.LinkToChild < reportItemCollection.Count)
			{
				ReportItem reportItem = ((ReportElementCollectionBase<ReportItem>)reportItemCollection)[rectangle.LinkToChild];
				((RPLRectanglePropsDef)sharedProps).LinkToChildId = reportItem.ID;
			}
		}
	}
}
