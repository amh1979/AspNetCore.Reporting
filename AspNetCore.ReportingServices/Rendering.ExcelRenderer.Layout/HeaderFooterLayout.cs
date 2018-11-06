using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.SPBIFReader.Callbacks;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal sealed class HeaderFooterLayout : ALayout
	{
		private List<ReportItemInfo> m_fullList;

		private int m_centerWidth;

		private int m_rightWidth;

		private List<ReportItemInfo> m_leftList;

		private List<ReportItemInfo> m_rightList;

		private List<ReportItemInfo> m_centerList;

		private float m_height;

		internal float Height
		{
			get
			{
				return this.m_height;
			}
		}

		internal override bool HeaderInBody
		{
			get
			{
				return true;
			}
		}

		internal override bool FooterInBody
		{
			get
			{
				return true;
			}
		}

		internal override bool? SummaryRowAfter
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		internal override bool? SummaryColumnAfter
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		internal HeaderFooterLayout(RPLReport report, float aWidth, float aHeight)
			: base(report)
		{
			this.m_fullList = new List<ReportItemInfo>();
			int num = LayoutConvert.ConvertMMTo20thPoints((double)aWidth);
			this.m_height = aHeight;
			this.m_centerWidth = num / 3;
			this.m_rightWidth = this.m_centerWidth * 2;
			this.m_leftList = new List<ReportItemInfo>();
			this.m_centerList = new List<ReportItemInfo>();
			this.m_rightList = new List<ReportItemInfo>();
		}

		internal override void AddReportItem(object rplSource, int top, int left, int width, int height, int generationIndex, byte state, string subreportLanguage, Dictionary<string, ToggleParent> toggleParents)
		{
			ReportItemInfo item = new ReportItemInfo(rplSource, top, left, left + width, (state & 0x20) != 0, toggleParents);
			this.m_fullList.Add(item);
		}

		internal override void AddStructuralItem(int top, int left, int width, int height, bool isToggglable, int generationIndex, RPLTablixMemberCell member, TogglePosition togglePosition)
		{
		}

		internal override void AddStructuralItem(int top, int left, int width, int height, int generationIndex, int rowHeaderWidth, int columnHeaderHeight, bool rtl)
		{
		}

		internal override ALayout GetPageHeaderLayout(float aWidth, float aHeight)
		{
			return this;
		}

		internal override ALayout GetPageFooterLayout(float aWidth, float aHeight)
		{
			return this;
		}

		internal override void CompleteSection()
		{
		}

		internal override void CompletePage()
		{
		}

		internal override void SetIsLastSection(bool isLastSection)
		{
		}

		internal void RenderStrings(RPLReport report, IExcelGenerator excel, out string left, out string center, out string right)
		{
			foreach (ReportItemInfo full in this.m_fullList)
			{
				RPLPageLayout rPLPageLayout = full.RPLSource as RPLPageLayout;
				if (rPLPageLayout == null)
				{
					RPLTextBox rPLTextBox = full.RPLSource as RPLTextBox;
					RPLItemProps rPLItemProps;
					byte b = default(byte);
					if (rPLTextBox != null)
					{
						if (rPLTextBox.StartOffset > 0)
						{
							rPLItemProps = base.m_report.GetItemProps(rPLTextBox.StartOffset, out b);
						}
						else
						{
							rPLItemProps = (RPLItemProps)rPLTextBox.ElementProps;
							b = 7;
						}
					}
					else
					{
						rPLItemProps = base.m_report.GetItemProps(full.RPLSource, out b);
					}
					if (b == 7)
					{
						full.Values = (RPLTextBoxProps)rPLItemProps;
						RPLElementStyle style = rPLItemProps.Style;
						HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left;
						object obj = style[25];
						if (obj != null)
						{
							horizontalAlignment = LayoutConvert.ToHorizontalAlignEnum((RPLFormat.TextAlignments)obj);
						}
						int num = 0;
						int num2 = 0;
						string text = (string)rPLItemProps.Style[15];
						string text2 = (string)rPLItemProps.Style[16];
						if (text != null)
						{
							num = LayoutConvert.ConvertMMTo20thPoints(LayoutConvert.ToMillimeters(text));
						}
						if (text2 != null)
						{
							num2 = LayoutConvert.ConvertMMTo20thPoints(LayoutConvert.ToMillimeters(text2));
						}
						switch (horizontalAlignment)
						{
						case HorizontalAlignment.Left:
							full.AlignmentPoint = full.Left + num;
							break;
						case HorizontalAlignment.Right:
							full.AlignmentPoint = full.Right - num2;
							break;
						default:
							full.AlignmentPoint = full.Left + (full.Right - full.Left + num - num2) / 2;
							break;
						}
						if (full.AlignmentPoint < this.m_centerWidth)
						{
							this.m_leftList.Add(full);
						}
						else if (full.AlignmentPoint < this.m_rightWidth)
						{
							this.m_centerList.Add(full);
						}
						else
						{
							this.m_rightList.Add(full);
						}
					}
				}
			}
			this.m_leftList.Sort(ReportItemInfo.CompareTopsThenLefts);
			this.m_centerList.Sort(ReportItemInfo.CompareTopsThenLefts);
			this.m_rightList.Sort(ReportItemInfo.CompareTopsThenLefts);
			left = this.RenderString(this.m_leftList, excel);
			center = this.RenderString(this.m_centerList, excel);
			right = this.RenderString(this.m_rightList, excel);
		}

		private string RenderString(List<ReportItemInfo> list, IExcelGenerator excel)
		{
			StringBuilder stringBuilder = new StringBuilder();
			HeaderFooterRichTextInfo headerFooterRichTextInfo = null;
			double num = 0.0;
			string text = string.Empty;
			bool flag = false;
			foreach (ReportItemInfo item in list)
			{
				if (stringBuilder.Length > 0 && !stringBuilder[stringBuilder.Length - 1].Equals("\n"))
				{
					stringBuilder.Append("\n");
				}
				RPLTextBoxPropsDef rPLTextBoxPropsDef = item.Values.Definition as RPLTextBoxPropsDef;
				if (rPLTextBoxPropsDef.IsSimple)
				{
					if (flag)
					{
						headerFooterRichTextInfo.CompleteCurrentFormatting();
					}
					excel.BuildHeaderFooterString(stringBuilder, item.Values, ref text, ref num);
					flag = false;
				}
				else
				{
					flag = true;
					RPLTextBox rPLTextBox = (RPLTextBox)item.RPLSource;
					if (headerFooterRichTextInfo == null)
					{
						headerFooterRichTextInfo = new HeaderFooterRichTextInfo(stringBuilder);
					}
					HorizontalAlignment horizontalAlignment = HorizontalAlignment.General;
					bool renderListPrefixes = true;
					object obj = rPLTextBox.ElementProps.Style[29];
					if (obj != null)
					{
						renderListPrefixes = ((RPLFormat.Directions)obj == RPLFormat.Directions.LTR);
					}
					LayoutEngine.RenderRichText(null, rPLTextBox, headerFooterRichTextInfo, true, null, renderListPrefixes, ref horizontalAlignment);
					num = headerFooterRichTextInfo.LastFontSize;
					text = headerFooterRichTextInfo.LastFontName;
					headerFooterRichTextInfo.CompleteRun();
				}
			}
			return stringBuilder.ToString(0, Math.Min(stringBuilder.Length, 256));
		}
	}
}
