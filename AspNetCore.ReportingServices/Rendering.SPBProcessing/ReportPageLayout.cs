using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class ReportPageLayout : PageElement
	{
		public override long Offset
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		internal override string SourceID
		{
			get
			{
				return base.m_source.ID;
			}
		}

		internal override string SourceUniqueName
		{
			get
			{
				return base.m_source.InstanceUniqueName;
			}
		}

		internal ReportPageLayout(Page source)
			: base(source)
		{
		}

		internal void WriteElementStyle(RPLWriter rplWriter, PageContext pageContext)
		{
			if (rplWriter != null)
			{
				Style style = base.m_source.Style;
				if (style != null)
				{
					BinaryWriter binaryWriter = rplWriter.BinaryWriter;
					if (binaryWriter != null)
					{
						this.WriteSharedStyle(binaryWriter, style, pageContext, 6);
						StyleInstance styleInstance = base.GetStyleInstance(base.m_source, null);
						if (styleInstance != null)
						{
							this.WriteNonSharedStyle(binaryWriter, style, styleInstance, pageContext, null, null);
						}
						binaryWriter.Write((byte)255);
					}
					else
					{
						RPLPageLayout pageLayout = rplWriter.Report.RPLPaginatedPages[0].PageLayout;
						RPLStyleProps rPLStyleProps = this.WriteSharedStyle(style, pageContext);
						RPLStyleProps rPLStyleProps2 = null;
						StyleInstance styleInstance2 = base.GetStyleInstance(base.m_source, null);
						if (styleInstance2 != null)
						{
							rPLStyleProps2 = this.WriteNonSharedStyle(style, styleInstance2, pageContext, null);
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

		internal override void WriteItemSharedStyleProps(BinaryWriter spbifWriter, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
			this.WriteBackgroundImage(spbifWriter, style, true, pageContext);
		}

		internal override void WriteItemSharedStyleProps(RPLStyleProps rplStyleProps, Style style, PageContext pageContext)
		{
			base.WriteStyleProp(style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
			this.WriteBackgroundImage(rplStyleProps, style, true, pageContext);
		}

		internal override void WriteItemNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				this.WriteBackgroundImage(spbifWriter, styleDef, false, pageContext);
				break;
			}
		}

		internal override void WriteItemNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAtt, PageContext pageContext)
		{
			switch (styleAtt)
			{
			case StyleAttributeNames.BackgroundColor:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BackgroundColor, 34);
				break;
			case StyleAttributeNames.BackgroundImage:
				this.WriteBackgroundImage(rplStyleProps, styleDef, false, pageContext);
				break;
			}
		}

		internal override void WriteNonSharedStyleProp(BinaryWriter spbifWriter, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.BorderColor:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColor, 0);
				break;
			case StyleAttributeNames.BorderColorBottom:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorBottom, 4);
				break;
			case StyleAttributeNames.BorderColorLeft:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorLeft, 1);
				break;
			case StyleAttributeNames.BorderColorRight:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorRight, 2);
				break;
			case StyleAttributeNames.BorderColorTop:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderColorTop, 3);
				break;
			case StyleAttributeNames.BorderStyle:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyle, 5);
				break;
			case StyleAttributeNames.BorderStyleBottom:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleBottom, 9);
				break;
			case StyleAttributeNames.BorderStyleLeft:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleLeft, 6);
				break;
			case StyleAttributeNames.BorderStyleRight:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleRight, 7);
				break;
			case StyleAttributeNames.BorderStyleTop:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderStyleTop, 8);
				break;
			case StyleAttributeNames.BorderWidth:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidth, 10);
				break;
			case StyleAttributeNames.BorderWidthBottom:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthBottom, 14);
				break;
			case StyleAttributeNames.BorderWidthLeft:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthLeft, 11);
				break;
			case StyleAttributeNames.BorderWidthRight:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthRight, 12);
				break;
			case StyleAttributeNames.BorderWidthTop:
				base.WriteStyleProp(styleDef, style, spbifWriter, StyleAttributeNames.BorderWidthTop, 13);
				break;
			default:
				this.WriteItemNonSharedStyleProp(spbifWriter, styleDef, style, styleAttribute, pageContext);
				break;
			}
		}

		internal override void WriteNonSharedStyleProp(RPLStyleProps rplStyleProps, Style styleDef, StyleInstance style, StyleAttributeNames styleAttribute, PageContext pageContext)
		{
			switch (styleAttribute)
			{
			case StyleAttributeNames.BorderColor:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColor, 0);
				break;
			case StyleAttributeNames.BorderColorBottom:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorBottom, 4);
				break;
			case StyleAttributeNames.BorderColorLeft:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorLeft, 1);
				break;
			case StyleAttributeNames.BorderColorRight:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorRight, 2);
				break;
			case StyleAttributeNames.BorderColorTop:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderColorTop, 3);
				break;
			case StyleAttributeNames.BorderStyle:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyle, 5);
				break;
			case StyleAttributeNames.BorderStyleBottom:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleBottom, 9);
				break;
			case StyleAttributeNames.BorderStyleLeft:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleLeft, 6);
				break;
			case StyleAttributeNames.BorderStyleRight:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleRight, 7);
				break;
			case StyleAttributeNames.BorderStyleTop:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderStyleTop, 8);
				break;
			case StyleAttributeNames.BorderWidth:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidth, 10);
				break;
			case StyleAttributeNames.BorderWidthBottom:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthBottom, 14);
				break;
			case StyleAttributeNames.BorderWidthLeft:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthLeft, 11);
				break;
			case StyleAttributeNames.BorderWidthRight:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthRight, 12);
				break;
			case StyleAttributeNames.BorderWidthTop:
				base.WriteStyleProp(styleDef, style, rplStyleProps, StyleAttributeNames.BorderWidthTop, 13);
				break;
			default:
				this.WriteItemNonSharedStyleProp(rplStyleProps, styleDef, style, styleAttribute, pageContext);
				break;
			}
		}
	}
}
