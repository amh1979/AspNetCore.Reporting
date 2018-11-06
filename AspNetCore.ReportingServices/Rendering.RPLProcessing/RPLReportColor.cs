using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLReportColor
	{
		private const RegexOptions RegExOptions = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline;

		private static Regex m_colorRegex = new Regex("^#(\\d|a|b|c|d|e|f){6}$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline);

		private string m_color;

		private Color m_GDIColor = Color.Empty;

		public RPLReportColor(string color)
		{
			this.m_color = color;
			this.ParseColor();
		}

		public override string ToString()
		{
			return this.m_color;
		}

		public Color ToColor()
		{
			return this.m_GDIColor;
		}

		private void ParseColor()
		{
			if (string.IsNullOrEmpty(this.m_color))
			{
				this.m_GDIColor = Color.Empty;
			}
			else if (this.m_color.Length == 7 && this.m_color[0] == '#' && RPLReportColor.m_colorRegex.Match(this.m_color).Success)
			{
				this.ColorFromArgb();
			}
			else
			{
				this.m_GDIColor = Color.FromName(this.m_color);
			}
		}

		private void ColorFromArgb()
		{
			try
			{
				int red = Convert.ToInt32(this.m_color.Substring(1, 2), 16);
				int green = Convert.ToInt32(this.m_color.Substring(3, 2), 16);
				int blue = Convert.ToInt32(this.m_color.Substring(5, 2), 16);
				this.m_GDIColor = Color.FromArgb(red, green, blue);
			}
			catch
			{
				this.m_GDIColor = Color.FromArgb(0, 0, 0);
			}
		}
	}
}
