using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class ReportSectionPage : PageItem
	{
		private new Page m_source;

		private RPLPageLayout m_pageLayout;

		internal override string SourceUniqueName
		{
			get
			{
				return this.m_source.Instance.UniqueName;
			}
		}

		internal override string SourceID
		{
			get
			{
				return this.m_source.ID;
			}
		}

		internal RPLPageLayout PageLayout
		{
			set
			{
				this.m_pageLayout = value;
			}
		}

		internal ReportSectionPage(Page source)
			: base(null)
		{
			this.m_source = source;
		}

		internal override void WriteStartItemToStream(RPLWriter rplWriter, PageContext pageContext)
		{
		}

		internal void WriteItemStyle(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				Style style = this.m_source.Style;
				if (style != null)
				{
					BinaryWriter binaryWriter = rplWriter.BinaryWriter;
					if (binaryWriter != null)
					{
						base.WriteSharedStyle(binaryWriter, style, pageContext);
						StyleInstance style2 = this.m_source.Instance.Style;
						if (style2 != null)
						{
							this.WriteNonSharedStyleWithoutTag(binaryWriter, style, style2, pageContext);
						}
						binaryWriter.Write((byte)255);
					}
					else
					{
						RPLPageLayout pageLayout = this.m_pageLayout;
						if (pageLayout != null)
						{
							RPLStyleProps rPLStyleProps = base.WriteSharedStyle(style, pageContext);
							RPLStyleProps rPLStyleProps2 = null;
							StyleInstance style3 = this.m_source.Instance.Style;
							if (style3 != null)
							{
								rPLStyleProps2 = this.WriteNonSharedStyle(style, style3, pageContext);
							}
							if (rPLStyleProps == null && rPLStyleProps2 == null)
							{
								return;
							}
							pageLayout.Style = new RPLElementStyle(rPLStyleProps2, rPLStyleProps);
						}
					}
				}
			}
		}

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			base.WriteBackgroundImage(style, true, spbifWriter, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			PageItem.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			base.WriteBackgroundImage(style, true, rplStyleProps, pageContext);
		}

		internal override void WriteItemNonSharedStyleProps(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				base.WriteBackgroundImage(styleDef, false, spbifWriter, pageContext);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProps(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				base.WriteBackgroundImage(styleDef, false, rplStyleProps, pageContext);
				break;
			}
		}
	}
}
