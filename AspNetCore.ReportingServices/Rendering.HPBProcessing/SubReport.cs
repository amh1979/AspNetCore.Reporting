using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class SubReport : PageItemContainer, IStorable, IPersistable
	{
		private static Declaration m_declaration = SubReport.GetDeclaration();

		internal override byte RPLFormatType
		{
			get
			{
				return 12;
			}
		}

		internal SubReport()
		{
		}

		internal SubReport(AspNetCore.ReportingServices.OnDemandReportRendering.SubReport source)
			: base(source)
		{
			base.m_itemPageSizes = new ItemSizes(source);
			bool keepTogetherVertical = base.UnresolvedKTV = source.KeepTogether;
			base.KeepTogetherVertical = keepTogetherVertical;
			bool keepTogetherHorizontal = base.UnresolvedKTH = source.KeepTogether;
			base.KeepTogetherHorizontal = keepTogetherHorizontal;
		}

		protected override void CreateChildren(PageContext pageContext)
		{
			ReportSectionCollection reportSections = (base.m_source as AspNetCore.ReportingServices.OnDemandReportRendering.SubReport).Report.ReportSections;
			int count = reportSections.Count;
			base.m_children = new PageItem[count];
			base.m_indexesLeftToRight = new int[count];
			base.m_rightPadding = 0.0;
			base.m_bottomPadding = 0.0;
			double num = 0.0;
			for (int i = 0; i < count; i++)
			{
				ReportBody reportBody = new ReportBody(((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)reportSections)[i].Body, ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)reportSections)[i].Width);
				reportBody.CacheNonSharedProperties(pageContext);
				reportBody.ItemPageSizes.Top = num;
				base.m_indexesLeftToRight[i] = i;
				num += reportBody.ItemPageSizes.Height;
				if (i > 0)
				{
					List<int> list = new List<int>(1);
					list.Add(i - 1);
					reportBody.PageItemsAbove = list;
				}
				base.m_children[i] = reportBody;
			}
		}

		protected override void DetermineContentHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors, bool resolveState, bool resolveItem)
		{
			base.DetermineContentHorizontalSize(pageContext, leftInParentSystem, rightInParentSystem, ancestors, anyAncestorHasKT, hasUnpinnedAncestors, resolveState, resolveItem);
			if (base.m_children.Length > 1)
			{
				for (int i = 0; i < base.m_children.Length; i++)
				{
					ReportBody reportBody = base.m_children[i] as ReportBody;
					if (reportBody != null)
					{
						if (!reportBody.OnThisVerticalPage)
						{
							break;
						}
						if (reportBody.ItemPageSizes.Width < base.m_itemPageSizes.Width)
						{
							reportBody.ItemPageSizes.Width = base.m_itemPageSizes.Width;
						}
					}
				}
			}
		}

		internal override void OmitBorderOnPageBreak(RPLWriter rplWriter, double pageLeft, double pageTop, double pageRight, double pageBottom)
		{
			if (rplWriter != null && ((AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)base.m_source).OmitBorderOnPageBreak)
			{
				base.OmitBorderOnPageBreak(pageLeft, pageTop, pageRight, pageBottom);
			}
		}

		internal override RPLElement CreateRPLElement()
		{
			return new RPLSubReport();
		}

		internal override RPLElement CreateRPLElement(RPLElementProps props, PageContext pageContext)
		{
			RPLItemProps rplElementProps = props as RPLItemProps;
			return new RPLSubReport(rplElementProps);
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				BinaryWriter binaryWriter = rplWriter.BinaryWriter;
				if (binaryWriter != null)
				{
					Stream baseStream = binaryWriter.BaseStream;
					base.m_offset = baseStream.Position;
					binaryWriter.Write((byte)12);
					base.WriteElementProps(binaryWriter, rplWriter, pageContext, base.m_offset + 1);
				}
				else if (base.m_rplElement == null)
				{
					base.m_rplElement = new RPLSubReport();
					base.WriteElementProps(base.m_rplElement.ElementProps, pageContext);
				}
				else
				{
					RPLItemProps rplElementProps = base.m_rplElement.ElementProps as RPLItemProps;
					base.m_rplElement = new RPLSubReport(rplElementProps);
				}
			}
		}

		internal override void WriteCustomSharedItemProps(BinaryWriter spbifWriter, RPLWriter rplWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = (AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)base.m_source;
			if (subReport.ReportName != null)
			{
				spbifWriter.Write((byte)15);
				spbifWriter.Write(subReport.ReportName);
			}
		}

		internal override void WriteCustomSharedItemProps(RPLElementPropsDef sharedProps, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.SubReport subReport = (AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)base.m_source;
			if (subReport.ReportName != null)
			{
				((RPLSubReportPropsDef)sharedProps).ReportName = subReport.ReportName;
			}
		}

		internal override void WriteCustomNonSharedItemProps(BinaryWriter spbifWriter, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Report report = ((AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)base.m_source).Report;
			if (report != null)
			{
				string reportLanguage = Report.GetReportLanguage(report);
				if (reportLanguage != null)
				{
					spbifWriter.Write((byte)11);
					spbifWriter.Write(reportLanguage);
				}
			}
		}

		internal override void WriteCustomNonSharedItemProps(RPLElementProps nonSharedProps, PageContext pageContext)
		{
			AspNetCore.ReportingServices.OnDemandReportRendering.Report report = ((AspNetCore.ReportingServices.OnDemandReportRendering.SubReport)base.m_source).Report;
			if (report != null)
			{
				string reportLanguage = Report.GetReportLanguage(report);
				if (reportLanguage != null)
				{
					((RPLSubReportProps)nonSharedProps).Language = reportLanguage;
				}
			}
		}

		internal override bool AddToPage(RPLWriter rplWriter, PageContext pageContext, double pageLeft, double pageTop, double pageRight, double pageBottom, RepeatState repeatState)
		{
			bool inSubReport = pageContext.Common.InSubReport;
			pageContext.Common.InSubReport = true;
			bool result = base.AddToPage(rplWriter, pageContext, pageLeft, pageTop, pageRight, pageBottom, repeatState);
			pageContext.Common.InSubReport = inSubReport;
			return result;
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(SubReport.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false, string.Empty);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(SubReport.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false, string.Empty);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.SubReport;
		}

		internal new static Declaration GetDeclaration()
		{
			if (SubReport.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.SubReport, ObjectType.PageItemContainer, memberInfoList);
			}
			return SubReport.m_declaration;
		}
	}
}
