using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Drawing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ReportColor
	{
		private string m_color;

		private Color m_GDIColor = Color.Empty;

		private bool m_parsed;

		internal bool Parsed
		{
			get
			{
				return this.m_parsed;
			}
		}

		public ReportColor(Color color)
		{
			this.m_GDIColor = color;
			this.m_color = color.ToString();
			this.m_parsed = true;
		}

		public ReportColor(string color)
		{
			this.m_color = color;
			this.Validate();
			this.m_parsed = true;
		}

		internal ReportColor(string color, bool parsed)
		{
			this.m_color = color;
			this.m_parsed = parsed;
		}

		public override string ToString()
		{
			return this.m_color;
		}

		public Color ToColor()
		{
			if (!this.m_parsed)
			{
				Validator.ParseColor(this.m_color, out this.m_GDIColor);
				this.m_parsed = true;
			}
			return this.m_GDIColor;
		}

		internal void Validate()
		{
			if (Validator.ValidateColor(this.m_color, out this.m_GDIColor))
			{
				return;
			}
			throw new ReportRenderingException(ErrorCode.rrInvalidColor, this.m_color);
		}
	}
}
