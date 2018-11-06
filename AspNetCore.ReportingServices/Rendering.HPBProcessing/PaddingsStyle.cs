using AspNetCore.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class PaddingsStyle
	{
		[Flags]
		internal enum PaddingState : byte
		{
			Clear = 0,
			Left = 1,
			Right = 2,
			Top = 4,
			Bottom = 8
		}

		private double m_padHorizontal;

		private double m_padVertical;

		private double m_padTop;

		private PaddingState m_state;

		internal double PadHorizontal
		{
			get
			{
				return this.m_padHorizontal;
			}
			set
			{
				this.m_padHorizontal = value;
			}
		}

		internal double PadTop
		{
			set
			{
				this.m_padTop = value;
			}
		}

		internal double PadVertical
		{
			get
			{
				return this.m_padVertical;
			}
			set
			{
				this.m_padVertical = value;
			}
		}

		internal PaddingState State
		{
			get
			{
				return this.m_state;
			}
			set
			{
				this.m_state = value;
			}
		}

		private static ReportSize GetStyleValue(StyleAttributeNames styleName, ReportItem source)
		{
			bool flag = false;
			return PaddingsStyle.GetStyleValue(styleName, ref flag, source);
		}

		private static ReportSize GetStyleValue(StyleAttributeNames styleName, ref bool shared, ReportItem source)
		{
			object obj = null;
			ReportProperty reportProperty = ((StyleBase)source.Style)[styleName];
			if (reportProperty != null)
			{
				if (reportProperty.IsExpression)
				{
					shared = false;
					StyleInstance style = source.Instance.Style;
					obj = ((StyleBaseInstance)style)[styleName];
				}
				if (obj == null)
				{
					obj = ((ReportSizeProperty)reportProperty).Value;
				}
			}
			return obj as ReportSize;
		}

		internal static void CreatePaddingsStyle(PageContext pageContext, ReportItem source, out double padVertical, out double padHorizontal, out double padTop)
		{
			padVertical = 0.0;
			padHorizontal = 0.0;
			padTop = 0.0;
			PaddingsStyle paddingsStyle = null;
			bool flag = true;
			double num = 0.0;
			ReportSize styleValue = PaddingsStyle.GetStyleValue(StyleAttributeNames.PaddingTop, ref flag, source);
			if (styleValue != null)
			{
				num = styleValue.ToMillimeters();
				if (flag)
				{
					if (paddingsStyle == null)
					{
						paddingsStyle = new PaddingsStyle();
					}
					paddingsStyle.PadVertical += num;
					paddingsStyle.PadTop = num;
					paddingsStyle.State |= PaddingState.Top;
				}
				padTop = num;
				padVertical += num;
			}
			flag = true;
			styleValue = PaddingsStyle.GetStyleValue(StyleAttributeNames.PaddingBottom, ref flag, source);
			if (styleValue != null)
			{
				num = styleValue.ToMillimeters();
				if (flag)
				{
					if (paddingsStyle == null)
					{
						paddingsStyle = new PaddingsStyle();
					}
					paddingsStyle.PadVertical += num;
					paddingsStyle.State |= PaddingState.Bottom;
				}
				padVertical += num;
			}
			flag = true;
			styleValue = PaddingsStyle.GetStyleValue(StyleAttributeNames.PaddingLeft, ref flag, source);
			if (styleValue != null)
			{
				num = styleValue.ToMillimeters();
				if (flag)
				{
					if (paddingsStyle == null)
					{
						paddingsStyle = new PaddingsStyle();
					}
					paddingsStyle.PadHorizontal += num;
					paddingsStyle.State |= PaddingState.Left;
				}
				padHorizontal += num;
			}
			flag = true;
			styleValue = PaddingsStyle.GetStyleValue(StyleAttributeNames.PaddingRight, ref flag, source);
			if (styleValue != null)
			{
				num = styleValue.ToMillimeters();
				if (flag)
				{
					if (paddingsStyle == null)
					{
						paddingsStyle = new PaddingsStyle();
					}
					paddingsStyle.PadHorizontal += num;
					paddingsStyle.State |= PaddingState.Right;
				}
				padHorizontal += num;
			}
			if (paddingsStyle != null)
			{
				if (pageContext.ItemPaddingsStyle == null)
				{
					pageContext.ItemPaddingsStyle = new Hashtable();
				}
				pageContext.ItemPaddingsStyle.Add(source.ID, paddingsStyle);
			}
		}

		internal void GetPaddingValues(ReportItem source, out double padVertical, out double padHorizontal, out double padTop)
		{
			padVertical = this.m_padVertical;
			padHorizontal = this.m_padHorizontal;
			padTop = this.m_padTop;
			ReportSize reportSize = null;
			if ((this.m_state & PaddingState.Top) == PaddingState.Clear)
			{
				reportSize = PaddingsStyle.GetStyleValue(StyleAttributeNames.PaddingTop, source);
				if (reportSize != null)
				{
					padTop = reportSize.ToMillimeters();
					padVertical += padTop;
				}
			}
			if ((this.m_state & PaddingState.Bottom) == PaddingState.Clear)
			{
				reportSize = PaddingsStyle.GetStyleValue(StyleAttributeNames.PaddingBottom, source);
				if (reportSize != null)
				{
					padVertical += reportSize.ToMillimeters();
				}
			}
			if ((this.m_state & PaddingState.Left) == PaddingState.Clear)
			{
				reportSize = PaddingsStyle.GetStyleValue(StyleAttributeNames.PaddingLeft, source);
				if (reportSize != null)
				{
					padHorizontal += reportSize.ToMillimeters();
				}
			}
			if ((this.m_state & PaddingState.Right) == PaddingState.Clear)
			{
				reportSize = PaddingsStyle.GetStyleValue(StyleAttributeNames.PaddingRight, source);
				if (reportSize != null)
				{
					padHorizontal += reportSize.ToMillimeters();
				}
			}
		}
	}
}
