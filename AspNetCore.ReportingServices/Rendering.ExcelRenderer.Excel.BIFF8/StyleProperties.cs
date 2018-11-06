using AspNetCore.ReportingServices.Rendering.RichText;
using System.Collections.Generic;
using System.Drawing;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class StyleProperties
	{
		private const float DEFAULT_FONT_SIZE = 400f;

		private static Dictionary<string, CharSet> m_charSetLookup = new Dictionary<string, CharSet>();

		private static object m_charSetLookupLock = new object();

		private IColor m_backgroundColor;

		private int m_indentLevel;

		private bool m_wrapText = true;

		private int m_orientation;

		private string m_format;

		private HorizontalAlignment m_horizAlign;

		private VerticalAlignment m_vertAlign;

		private TextDirection m_textDir = TextDirection.LeftToRight;

		private ExcelBorderStyle m_borderLeftStyle;

		private ExcelBorderStyle m_borderRightStyle;

		private ExcelBorderStyle m_borderTopStyle;

		private ExcelBorderStyle m_borderBottomStyle;

		private ExcelBorderStyle m_borderDiagStyle;

		private IColor m_borderLeftColor;

		private IColor m_borderRightColor;

		private IColor m_borderTopColor;

		private IColor m_borderBottomColor;

		private IColor m_borderDiagColor;

		private ExcelBorderPart m_borderDiagPart = ExcelBorderPart.DiagonalBoth;

		private int m_bold = 400;

		private bool m_italic;

		private bool m_strikethrough;

		private Underline m_underline;

		private ScriptStyle m_scriptStyle;

		private IColor m_fontColor;

		private string m_fontName;

		private double m_fontSize = 10.0;

		private int m_ixfe;

		internal IColor BackgroundColor
		{
			get
			{
				return this.m_backgroundColor;
			}
			set
			{
				this.m_backgroundColor = value;
			}
		}

		internal int IndentLevel
		{
			get
			{
				return this.m_indentLevel;
			}
			set
			{
				this.m_indentLevel = value;
			}
		}

		internal bool WrapText
		{
			get
			{
				return this.m_wrapText;
			}
			set
			{
				this.m_wrapText = value;
			}
		}

		internal int Orientation
		{
			get
			{
				return this.m_orientation;
			}
			set
			{
				this.m_orientation = value;
			}
		}

		internal int Bold
		{
			get
			{
				return this.m_bold;
			}
			set
			{
				this.m_bold = value;
			}
		}

		internal bool Italic
		{
			get
			{
				return this.m_italic;
			}
			set
			{
				this.m_italic = value;
			}
		}

		internal bool Strikethrough
		{
			get
			{
				return this.m_strikethrough;
			}
			set
			{
				this.m_strikethrough = value;
			}
		}

		internal Underline Underline
		{
			get
			{
				return this.m_underline;
			}
			set
			{
				this.m_underline = value;
			}
		}

		internal ScriptStyle ScriptStyle
		{
			get
			{
				return this.m_scriptStyle;
			}
			set
			{
				this.m_scriptStyle = value;
			}
		}

		internal IColor Color
		{
			get
			{
				return this.m_fontColor;
			}
			set
			{
				this.m_fontColor = value;
			}
		}

		internal string Name
		{
			get
			{
				return this.m_fontName;
			}
			set
			{
				this.m_fontName = value;
				StyleProperties.AddCharSet(this.m_fontName);
			}
		}

		internal double Size
		{
			get
			{
				return this.m_fontSize;
			}
			set
			{
				this.m_fontSize = value;
			}
		}

		internal CharSet CharSet
		{
			get
			{
				return StyleProperties.GetCharSet(this.m_fontName);
			}
		}

		internal string NumberFormat
		{
			get
			{
				return this.m_format;
			}
			set
			{
				this.m_format = value;
			}
		}

		internal HorizontalAlignment HorizontalAlignment
		{
			get
			{
				return this.m_horizAlign;
			}
			set
			{
				this.m_horizAlign = value;
			}
		}

		internal VerticalAlignment VerticalAlignment
		{
			get
			{
				return this.m_vertAlign;
			}
			set
			{
				this.m_vertAlign = value;
			}
		}

		internal TextDirection TextDirection
		{
			get
			{
				return this.m_textDir;
			}
			set
			{
				this.m_textDir = value;
			}
		}

		internal ExcelBorderStyle BorderLeftStyle
		{
			get
			{
				return this.m_borderLeftStyle;
			}
			set
			{
				this.m_borderLeftStyle = value;
			}
		}

		internal ExcelBorderStyle BorderRightStyle
		{
			get
			{
				return this.m_borderRightStyle;
			}
			set
			{
				this.m_borderRightStyle = value;
			}
		}

		internal ExcelBorderStyle BorderTopStyle
		{
			get
			{
				return this.m_borderTopStyle;
			}
			set
			{
				this.m_borderTopStyle = value;
			}
		}

		internal ExcelBorderStyle BorderBottomStyle
		{
			get
			{
				return this.m_borderBottomStyle;
			}
			set
			{
				this.m_borderBottomStyle = value;
			}
		}

		internal ExcelBorderStyle BorderDiagStyle
		{
			get
			{
				return this.m_borderDiagStyle;
			}
			set
			{
				this.m_borderDiagStyle = value;
			}
		}

		internal IColor BorderLeftColor
		{
			get
			{
				return this.m_borderLeftColor;
			}
			set
			{
				this.m_borderLeftColor = value;
			}
		}

		internal IColor BorderRightColor
		{
			get
			{
				return this.m_borderRightColor;
			}
			set
			{
				this.m_borderRightColor = value;
			}
		}

		internal IColor BorderTopColor
		{
			get
			{
				return this.m_borderTopColor;
			}
			set
			{
				this.m_borderTopColor = value;
			}
		}

		internal IColor BorderBottomColor
		{
			get
			{
				return this.m_borderBottomColor;
			}
			set
			{
				this.m_borderBottomColor = value;
			}
		}

		internal IColor BorderDiagColor
		{
			get
			{
				return this.m_borderDiagColor;
			}
			set
			{
				this.m_borderDiagColor = value;
			}
		}

		internal ExcelBorderPart BorderDiagPart
		{
			get
			{
				return this.m_borderDiagPart;
			}
			set
			{
				this.m_borderDiagPart = value;
			}
		}

		internal int Ixfe
		{
			get
			{
				return this.m_ixfe;
			}
			set
			{
				this.m_ixfe = value;
			}
		}

		internal StyleProperties()
		{
		}

		private static void AddCharSet(string fontName)
		{
			if (fontName != null)
			{
				lock (StyleProperties.m_charSetLookupLock)
				{
					if (!StyleProperties.m_charSetLookup.ContainsKey(fontName))
					{
						Font font = null;
						try
						{
							bool flag = false;
							bool flag2 = false;
							font = FontCache.CreateGdiPlusFont(fontName, 400f, ref flag, ref flag2, false, false);
							LOGFONT lOGFONT = new LOGFONT();
							font.ToLogFont(lOGFONT);
							StyleProperties.m_charSetLookup.Add(fontName, (CharSet)lOGFONT.lfCharSet);
						}
						finally
						{
							if (font != null)
							{
								font.Dispose();
								font = null;
							}
						}
					}
				}
			}
		}

		private static CharSet GetCharSet(string fontName)
		{
			if (fontName != null)
			{
				lock (StyleProperties.m_charSetLookupLock)
				{
					CharSet result = default(CharSet);
					if (StyleProperties.m_charSetLookup.TryGetValue(fontName, out result))
					{
						return result;
					}
				}
			}
			return CharSet.DEFAULT_CHARSET;
		}
	}
}
