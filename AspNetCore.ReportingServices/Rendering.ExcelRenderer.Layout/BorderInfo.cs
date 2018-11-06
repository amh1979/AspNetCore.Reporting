using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Layout
{
	internal class BorderInfo
	{
		private IColor m_backgroundColor;

		private BorderProperties m_leftBorder;

		private BorderProperties m_topBorder;

		private BorderProperties m_rightBorder;

		private BorderProperties m_bottomBorder;

		private BorderProperties m_diagonal;

		private bool m_omitBorderTop;

		private bool m_omitBorderBottom;

		internal BorderProperties RightBorder
		{
			get
			{
				return this.m_rightBorder;
			}
		}

		internal BorderProperties BottomBorder
		{
			get
			{
				return this.m_bottomBorder;
			}
		}

		internal IColor BackgroundColor
		{
			get
			{
				return this.m_backgroundColor;
			}
		}

		internal BorderProperties LeftBorder
		{
			get
			{
				return this.m_leftBorder;
			}
		}

		internal BorderProperties TopBorder
		{
			get
			{
				return this.m_topBorder;
			}
		}

		internal BorderProperties Diagonal
		{
			get
			{
				return this.m_diagonal;
			}
		}

		internal bool OmitBorderTop
		{
			get
			{
				return this.m_omitBorderTop;
			}
			set
			{
				this.m_omitBorderTop = value;
			}
		}

		internal bool OmitBorderBottom
		{
			get
			{
				return this.m_omitBorderBottom;
			}
			set
			{
				this.m_omitBorderBottom = value;
			}
		}

		internal BorderInfo()
		{
		}

		internal BorderInfo(RPLStyleProps style, bool omitBorderTop, bool omitBorderBottom, IExcelGenerator excel)
		{
			this.m_omitBorderTop = omitBorderTop;
			this.m_omitBorderBottom = omitBorderBottom;
			BorderInfo.FillAllBorders(style, ref this.m_leftBorder, ref this.m_rightBorder, ref this.m_topBorder, ref this.m_bottomBorder, ref this.m_backgroundColor, excel);
		}

		internal BorderInfo(RPLElementStyle style, int width, int height, bool slant, bool omitBorderTop, bool omitBorderBottom, bool defaultLine, IExcelGenerator excel)
		{
			this.m_omitBorderTop = omitBorderTop;
			this.m_omitBorderBottom = omitBorderBottom;
			if (height == 0)
			{
				if (defaultLine)
				{
					this.m_topBorder = new BorderProperties(ExcelBorderPart.Top);
					this.FillBorderProperties(excel, this.m_topBorder, style[5], style[10], style[0]);
				}
				else
				{
					this.m_bottomBorder = new BorderProperties(ExcelBorderPart.Bottom);
					this.FillBorderProperties(excel, this.m_bottomBorder, style[5], style[10], style[0]);
				}
			}
			else if (width == 0)
			{
				this.m_leftBorder = new BorderProperties(ExcelBorderPart.Left);
				this.FillBorderProperties(excel, this.m_leftBorder, style[5], style[10], style[0]);
			}
			else if (slant)
			{
				this.m_diagonal = new BorderProperties(ExcelBorderPart.DiagonalUp);
				this.FillBorderProperties(excel, this.m_diagonal, style[5], style[10], style[0]);
			}
			else
			{
				this.m_diagonal = new BorderProperties(ExcelBorderPart.DiagonalDown);
				this.FillBorderProperties(excel, this.m_diagonal, style[5], style[10], style[0]);
			}
		}

		internal static void FillAllBorders(RPLStyleProps style, ref BorderProperties leftBorder, ref BorderProperties rightBorder, ref BorderProperties topBorder, ref BorderProperties bottomBorder, ref IColor backgroundColor, IExcelGenerator excel)
		{
			if (style[34] != null && !style[34].Equals("Transparent"))
			{
				backgroundColor = excel.AddColor((string)style[34]);
			}
			BorderInfo.FillLeftBorderProperties(style, excel, ref leftBorder);
			BorderInfo.FillRightBorderProperties(style, excel, ref rightBorder);
			BorderInfo.FillTopBorderProperties(style, excel, ref topBorder);
			BorderInfo.FillBottomBorderProperties(style, excel, ref bottomBorder);
		}

		private static void FillLeftBorderProperties(RPLStyleProps style, IExcelGenerator excel, ref BorderProperties leftBorder)
		{
			BorderProperties currBorder = BorderInfo.FillBorderProperties(excel, null, leftBorder, ExcelBorderPart.Left, style[5], style[10], style[0]);
			currBorder = BorderInfo.FillBorderProperties(excel, currBorder, leftBorder, ExcelBorderPart.Left, style[6], style[11], style[1]);
			if (currBorder != null)
			{
				leftBorder = currBorder;
			}
		}

		private static void FillRightBorderProperties(RPLStyleProps style, IExcelGenerator excel, ref BorderProperties rightBorder)
		{
			BorderProperties currBorder = BorderInfo.FillBorderProperties(excel, null, rightBorder, ExcelBorderPart.Right, style[5], style[10], style[0]);
			currBorder = BorderInfo.FillBorderProperties(excel, currBorder, rightBorder, ExcelBorderPart.Right, style[7], style[12], style[2]);
			if (currBorder != null)
			{
				rightBorder = currBorder;
			}
		}

		private static void FillTopBorderProperties(RPLStyleProps style, IExcelGenerator excel, ref BorderProperties topBorder)
		{
			BorderProperties currBorder = BorderInfo.FillBorderProperties(excel, null, topBorder, ExcelBorderPart.Top, style[5], style[10], style[0]);
			currBorder = BorderInfo.FillBorderProperties(excel, currBorder, topBorder, ExcelBorderPart.Top, style[8], style[13], style[3]);
			if (currBorder != null)
			{
				topBorder = currBorder;
			}
		}

		private static void FillBottomBorderProperties(RPLStyleProps style, IExcelGenerator excel, ref BorderProperties bottomBorder)
		{
			BorderProperties currBorder = BorderInfo.FillBorderProperties(excel, null, bottomBorder, ExcelBorderPart.Bottom, style[5], style[10], style[0]);
			currBorder = BorderInfo.FillBorderProperties(excel, currBorder, bottomBorder, ExcelBorderPart.Bottom, style[9], style[14], style[4]);
			if (currBorder != null)
			{
				bottomBorder = currBorder;
			}
		}

		private static BorderProperties FillBorderProperties(IExcelGenerator excel, BorderProperties currBorder, BorderProperties border, ExcelBorderPart part, object style, object width, object color)
		{
			BorderProperties borderProperties = currBorder;
			if (style != null)
			{
				if (borderProperties == null)
				{
					borderProperties = new BorderProperties(border, part);
				}
				borderProperties.Style = LayoutConvert.ToBorderLineStyle((RPLFormat.BorderStyles)style);
			}
			if (width != null)
			{
				if (borderProperties == null)
				{
					borderProperties = new BorderProperties(border, part);
				}
				borderProperties.Width = LayoutConvert.ToPoints((string)width);
			}
			if (color != null && !color.Equals("Transparent"))
			{
				if (borderProperties == null)
				{
					borderProperties = new BorderProperties(border, part);
				}
				borderProperties.Color = excel.AddColor((string)color);
			}
			return borderProperties;
		}

		private void FillBorderProperties(IExcelGenerator excel, BorderProperties border, object style, object width, object color)
		{
			if (style != null)
			{
				border.Style = LayoutConvert.ToBorderLineStyle((RPLFormat.BorderStyles)style);
			}
			if (width != null)
			{
				border.Width = LayoutConvert.ToPoints((string)width);
			}
			if (color != null && !color.Equals("Transparent"))
			{
				border.Color = excel.AddColor((string)color);
			}
		}

		internal void RenderBorders(IExcelGenerator excel)
		{
			IStyle cellStyle = excel.GetCellStyle();
			if (this.m_backgroundColor != null)
			{
				cellStyle.BackgroundColor = this.m_backgroundColor;
			}
			if (this.m_diagonal != null)
			{
				this.m_diagonal.Render(cellStyle);
			}
			if (this.m_topBorder != null && !this.m_omitBorderTop)
			{
				this.m_topBorder.Render(cellStyle);
			}
			if (this.m_bottomBorder != null && !this.m_omitBorderBottom)
			{
				this.m_bottomBorder.Render(cellStyle);
			}
			if (this.m_leftBorder != null)
			{
				this.m_leftBorder.Render(cellStyle);
			}
			if (this.m_rightBorder != null)
			{
				this.m_rightBorder.Render(cellStyle);
			}
		}
	}
}
