using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportColor
	{
		private string m_color;

		private Color m_GDIColor = Color.Empty;

		private bool m_parsed;

		public ReportColor(string color)
			: this(color, false)
		{
		}

		public ReportColor(string color, bool allowTransparency)
		{
			this.m_color = color;
			this.Validate(allowTransparency);
			this.m_parsed = true;
		}

		internal ReportColor(string color, Color gdiColor, bool parsed)
		{
			this.m_color = color;
			this.m_parsed = parsed;
			this.m_GDIColor = gdiColor;
		}

		internal ReportColor(AspNetCore.ReportingServices.ReportRendering.ReportColor oldColor)
		{
			this.m_color = oldColor.ToString();
			this.m_parsed = oldColor.Parsed;
			if (this.m_parsed)
			{
				this.m_GDIColor = oldColor.ToColor();
			}
		}

		public override string ToString()
		{
			return this.m_color;
		}

		public Color ToColor()
		{
			if (!this.m_parsed)
			{
				Validator.ParseColor(this.m_color, out this.m_GDIColor, false);
				this.m_parsed = true;
			}
			return this.m_GDIColor;
		}

		internal void Validate(bool allowTransparency)
		{
			if (Validator.ValidateColor(this.m_color, out this.m_GDIColor, allowTransparency))
			{
				return;
			}
			throw new RenderingObjectModelException(ErrorCode.rrInvalidColor, this.m_color);
		}

		public static bool TryParse(string value, out ReportColor reportColor)
		{
			return ReportColor.TryParse(value, false, out reportColor);
		}

		public static bool TryParse(string value, bool allowTransparency, out ReportColor reportColor)
		{
			Color gdiColor = default(Color);
			if (Validator.ValidateColor(value, out gdiColor, allowTransparency))
			{
				reportColor = new ReportColor(value, gdiColor, true);
				return true;
			}
			reportColor = null;
			return false;
		}
	}
}
