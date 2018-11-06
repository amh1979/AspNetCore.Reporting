using AspNetCore.ReportingServices.Rendering.RichText;
using AspNetCore.ReportingServices.Rendering.RPLProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.ImageRenderer
{
	internal class ReportTextRun : ITextRunProps
	{
		private string m_uniqueName;

		private RPLActionInfo m_actionInfo;

		private RPLElementStyle m_sourceStyle;

		private float m_fontSize = 10f;

		private string m_fontKey;

		private Color m_color;

		public string UniqueName
		{
			get
			{
				return this.m_uniqueName;
			}
		}

		public RPLActionInfo ActionInfo
		{
			get
			{
				return this.m_actionInfo;
			}
		}

		public string FontFamily
		{
			get
			{
				object obj = this.m_sourceStyle[20];
				if (obj == null)
				{
					return "Arial";
				}
				return (string)obj;
			}
		}

		public float FontSize
		{
			get
			{
				return this.m_fontSize;
			}
		}

		public Color Color
		{
			get
			{
				return this.m_color;
			}
		}

		public bool Bold
		{
			get
			{
				object obj = this.m_sourceStyle[22];
				if (obj == null)
				{
					return false;
				}
				return SharedRenderer.IsWeightBold((RPLFormat.FontWeights)obj);
			}
		}

		public bool Italic
		{
			get
			{
				object obj = this.m_sourceStyle[19];
				if (obj == null)
				{
					return false;
				}
				return (RPLFormat.FontStyles)obj == RPLFormat.FontStyles.Italic;
			}
		}

		public RPLFormat.TextDecorations TextDecoration
		{
			get
			{
				object obj = this.m_sourceStyle[24];
				if (obj == null)
				{
					return RPLFormat.TextDecorations.None;
				}
				return (RPLFormat.TextDecorations)obj;
			}
		}

		public int IndexInParagraph
		{
			get
			{
				return -1;
			}
		}

		public string FontKey
		{
			get
			{
				return this.m_fontKey;
			}
			set
			{
				this.m_fontKey = value;
			}
		}

		internal ReportTextRun(RPLElementStyle sourceStyle, Dictionary<string, float> cachedReportSizes, Dictionary<string, Color> cachedReportColors)
		{
			this.m_sourceStyle = sourceStyle;
			this.SetFontSize(cachedReportSizes);
			this.SetFontColor(cachedReportColors);
		}

		internal ReportTextRun(RPLElementStyle sourceStyle, string uniqueName, RPLActionInfo sourceActionInfo, Dictionary<string, float> cachedReportSizes, Dictionary<string, Color> cachedReportColors)
			: this(sourceStyle, cachedReportSizes, cachedReportColors)
		{
			this.m_uniqueName = uniqueName;
			this.m_actionInfo = sourceActionInfo;
		}

		public void AddSplitIndex(int index)
		{
		}

		private void SetFontSize(Dictionary<string, float> cachedReportSizes)
		{
			string text = (string)this.m_sourceStyle[21];
			if (!string.IsNullOrEmpty(text))
			{
				float num = default(float);
				if (cachedReportSizes == null)
				{
					num = (float)new RPLReportSize(text).ToPoints();
				}
				else if (!cachedReportSizes.TryGetValue(text, out num))
				{
					num = (float)new RPLReportSize(text).ToPoints();
					cachedReportSizes.Add(text, num);
				}
				this.m_fontSize = num;
			}
		}

		private void SetFontColor(Dictionary<string, Color> cachedReportColors)
		{
			string text = (string)this.m_sourceStyle[27];
			if (string.IsNullOrEmpty(text) || string.Compare(text, "TRANSPARENT", StringComparison.OrdinalIgnoreCase) == 0)
			{
				this.m_color = Color.Black;
			}
			else if (cachedReportColors == null)
			{
				this.m_color = new RPLReportColor(text).ToColor();
			}
			else if (!cachedReportColors.TryGetValue(text, out this.m_color))
			{
				this.m_color = new RPLReportColor(text).ToColor();
				cachedReportColors.Add(text, this.m_color);
			}
		}
	}
}
