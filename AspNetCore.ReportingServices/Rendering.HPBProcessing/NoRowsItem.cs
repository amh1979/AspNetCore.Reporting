using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandProcessing.Scalability;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class NoRowsItem : PageItem, IStorable, IPersistable
	{
		private static Declaration m_declaration = NoRowsItem.GetDeclaration();

		internal override bool ContentOnPage
		{
			get
			{
				return false;
			}
		}

		internal NoRowsItem()
		{
		}

		internal NoRowsItem(ReportItem source)
			: base(source)
		{
			base.m_itemPageSizes = new ItemSizes(source);
			base.KeepTogetherHorizontal = false;
			base.KeepTogetherVertical = false;
			bool unresolvedKTV = base.UnresolvedKTH = false;
			base.UnresolvedKTV = unresolvedKTV;
		}

		protected override void DetermineVerticalSize(PageContext pageContext, double topInParentSystem, double bottomInParentSystem, List<PageItem> ancestors, ref bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			base.m_itemPageSizes.AdjustHeightTo(0.0);
		}

		protected override void DetermineHorizontalSize(PageContext pageContext, double leftInParentSystem, double rightInParentSystem, List<PageItem> ancestors, bool anyAncestorHasKT, bool hasUnpinnedAncestors)
		{
			base.m_itemPageSizes.AdjustWidthTo(0.0);
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
		}

		public override void Serialize(IntermediateFormatWriter writer)
		{
			base.Serialize(writer);
			writer.RegisterDeclaration(NoRowsItem.m_declaration);
			while (writer.NextMember())
			{
				MemberName memberName = writer.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false, string.Empty);
			}
		}

		public override void Deserialize(IntermediateFormatReader reader)
		{
			base.Deserialize(reader);
			reader.RegisterDeclaration(NoRowsItem.m_declaration);
			while (reader.NextMember())
			{
				MemberName memberName = reader.CurrentMember.MemberName;
				RSTrace.RenderingTracer.Assert(false, string.Empty);
			}
		}

		public override ObjectType GetObjectType()
		{
			return ObjectType.NoRowsItem;
		}

		internal new static Declaration GetDeclaration()
		{
			if (NoRowsItem.m_declaration == null)
			{
				List<MemberInfo> memberInfoList = new List<MemberInfo>();
				return new Declaration(ObjectType.NoRowsItem, ObjectType.PageItem, memberInfoList);
			}
			return NoRowsItem.m_declaration;
		}
	}
}
