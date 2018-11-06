using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.BIFF8.Records;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class StyleContainer : IStyle, IFont
	{
		private StyleState m_context;

		private Dictionary<string, StyleProperties> m_cache = new Dictionary<string, StyleProperties>();

		private List<BIFF8Font> m_fonts = new List<BIFF8Font>();

		private Dictionary<BIFF8Font, int> m_fontMap = new Dictionary<BIFF8Font, int>();

		private List<BIFF8Style> m_styles = new List<BIFF8Style>();

		private Dictionary<BIFF8Style, int> m_styleMap = new Dictionary<BIFF8Style, int>();

		private List<BIFF8Format> m_formats = new List<BIFF8Format>();

		private Dictionary<string, int> m_formatStringMap = new Dictionary<string, int>();

		private Dictionary<int, int> m_formatIntMap = new Dictionary<int, int>();

		private int m_currentCustomFormatIndex = 164;

		private ushort m_cellIxfe = 15;

		internal int CellIxfe
		{
			get
			{
				return this.m_cellIxfe;
			}
			set
			{
				this.m_cellIxfe = (ushort)value;
			}
		}

		public ExcelBorderStyle BorderLeftStyle
		{
			set
			{
				this.CheckContext();
				this.m_context.BorderLeftStyle = value;
			}
		}

		public ExcelBorderStyle BorderRightStyle
		{
			set
			{
				this.CheckContext();
				this.m_context.BorderRightStyle = value;
			}
		}

		public ExcelBorderStyle BorderTopStyle
		{
			set
			{
				this.CheckContext();
				this.m_context.BorderTopStyle = value;
			}
		}

		public ExcelBorderStyle BorderBottomStyle
		{
			set
			{
				this.CheckContext();
				this.m_context.BorderBottomStyle = value;
			}
		}

		public ExcelBorderStyle BorderDiagStyle
		{
			set
			{
				this.CheckContext();
				this.m_context.BorderDiagStyle = value;
			}
		}

		public IColor BorderLeftColor
		{
			set
			{
				this.CheckContext();
				this.m_context.BorderLeftColor = value;
			}
		}

		public IColor BorderRightColor
		{
			set
			{
				this.CheckContext();
				this.m_context.BorderRightColor = value;
			}
		}

		public IColor BorderTopColor
		{
			set
			{
				this.CheckContext();
				this.m_context.BorderTopColor = value;
			}
		}

		public IColor BorderBottomColor
		{
			set
			{
				this.CheckContext();
				this.m_context.BorderBottomColor = value;
			}
		}

		public IColor BorderDiagColor
		{
			set
			{
				this.CheckContext();
				this.m_context.BorderDiagColor = value;
			}
		}

		public ExcelBorderPart BorderDiagPart
		{
			set
			{
				this.CheckContext();
				this.m_context.BorderDiagPart = value;
			}
		}

		public IColor BackgroundColor
		{
			set
			{
				if (value != null)
				{
					this.CheckContext();
					this.m_context.BackgroundColor = value;
				}
			}
		}

		public int IndentLevel
		{
			set
			{
				this.CheckContext();
				this.m_context.IndentLevel = value;
			}
		}

		public bool WrapText
		{
			set
			{
				this.CheckContext();
				this.m_context.WrapText = value;
			}
		}

		public int Orientation
		{
			set
			{
				this.CheckContext();
				this.m_context.Orientation = value;
			}
		}

		public string NumberFormat
		{
			get
			{
				if (this.m_context == null)
				{
					return null;
				}
				return this.m_context.NumberFormat;
			}
			set
			{
				this.CheckContext();
				this.m_context.NumberFormat = value;
			}
		}

		public HorizontalAlignment HorizontalAlignment
		{
			set
			{
				this.CheckContext();
				this.m_context.HorizontalAlignment = value;
			}
		}

		public VerticalAlignment VerticalAlignment
		{
			set
			{
				this.CheckContext();
				this.m_context.VerticalAlignment = value;
			}
		}

		public TextDirection TextDirection
		{
			set
			{
				this.CheckContext();
				this.m_context.TextDirection = value;
			}
		}

		public int Bold
		{
			set
			{
				this.CheckContext();
				this.m_context.Bold = value;
			}
		}

		public bool Italic
		{
			set
			{
				this.CheckContext();
				this.m_context.Italic = value;
			}
		}

		public bool Strikethrough
		{
			set
			{
				this.CheckContext();
				this.m_context.Strikethrough = value;
			}
		}

		public ScriptStyle ScriptStyle
		{
			set
			{
				this.CheckContext();
				this.m_context.ScriptStyle = value;
			}
		}

		public IColor Color
		{
			set
			{
				this.CheckContext();
				this.m_context.Color = value;
			}
		}

		public Underline Underline
		{
			set
			{
				this.CheckContext();
				this.m_context.Underline = value;
			}
		}

		public string Name
		{
			set
			{
				this.CheckContext();
				this.m_context.Name = value;
			}
		}

		public double Size
		{
			set
			{
				this.CheckContext();
				this.m_context.Size = value;
			}
		}

		internal StyleContainer()
		{
			this.AddBuiltInFormats();
		}

		internal void Finish()
		{
			if (this.m_context != null)
			{
				this.m_context.Finished();
			}
			else
			{
				this.m_cellIxfe = 15;
			}
			this.m_context = null;
		}

		internal void Reset()
		{
			this.m_context = null;
			this.m_cellIxfe = 15;
		}

		internal void DefineSharedStyle(string id)
		{
			this.m_context = new DefineSharedStyle(this, id);
		}

		internal bool UseSharedStyle(string id)
		{
			if (this.m_cache.ContainsKey(id))
			{
				this.m_context = new UseSharedStyle(this, this.GetSharedStyle(id));
				return true;
			}
			return false;
		}

		internal void SetContext(StyleState state)
		{
			this.m_context = state;
		}

		internal BIFF8Style GetStyle(int ixfe)
		{
			return this.m_styles[ixfe - 21];
		}

		internal string GetFormat(int ifmt)
		{
			int index = default(int);
			if (this.m_formatIntMap.TryGetValue(ifmt, out index))
			{
				return this.m_formats[index].String;
			}
			throw new ReportRenderingException(ExcelRenderRes.InvalidIndexException(ifmt.ToString(CultureInfo.InvariantCulture)));
		}

		internal BIFF8Font GetFont(int ifnt)
		{
			return this.m_fonts[ifnt - 5];
		}

		internal int AddStyle(BIFF8Style style)
		{
			int num = default(int);
			if (!this.m_styleMap.TryGetValue(style, out num))
			{
				num = this.m_styles.Count + 21;
				this.m_styleMap.Add(style, num);
				this.m_styles.Add(style);
			}
			return num;
		}

		internal int AddStyle(StyleProperties props)
		{
			BIFF8Style bIFF8Style = new BIFF8Style(props);
			BIFF8Font font = new BIFF8Font(props);
			bIFF8Style.Ifnt = this.AddFont(font);
			bIFF8Style.Ifmt = this.AddFormat(props.NumberFormat);
			return this.AddStyle(bIFF8Style);
		}

		internal int AddFormat(string format)
		{
			if (format != null && format.Length != 0)
			{
				int currentCustomFormatIndex = default(int);
				if (!this.m_formatStringMap.TryGetValue(format, out currentCustomFormatIndex))
				{
					currentCustomFormatIndex = this.m_currentCustomFormatIndex;
					BIFF8Format item = new BIFF8Format(format, currentCustomFormatIndex);
					this.m_formatStringMap.Add(format, currentCustomFormatIndex);
					this.m_formatIntMap.Add(currentCustomFormatIndex, this.m_formats.Count);
					this.m_formats.Add(item);
					this.m_currentCustomFormatIndex++;
				}
				return currentCustomFormatIndex;
			}
			return 0;
		}

		internal int AddFont(BIFF8Font font)
		{
			int num = default(int);
			if (!this.m_fontMap.TryGetValue(font, out num))
			{
				num = this.m_fonts.Count + 5;
				this.m_fontMap.Add(font, num);
				this.m_fonts.Add(font);
			}
			return num;
		}

		internal void AddSharedStyle(string id, StyleProperties style)
		{
			this.m_cache[id] = style;
		}

		internal StyleProperties GetSharedStyle(string id)
		{
			return this.m_cache[id];
		}

		internal void Write(BinaryWriter writer)
		{
			foreach (BIFF8Font font in this.m_fonts)
			{
				RecordFactory.FONT(writer, font);
			}
			this.m_fonts = null;
			this.m_fontMap = null;
			foreach (BIFF8Format format in this.m_formats)
			{
				RecordFactory.FORMAT(writer, format.String, format.Index);
			}
			this.m_formats = null;
			this.m_formatIntMap = null;
			this.m_formatStringMap = null;
			writer.BaseStream.Write(Constants.GLOBAL2, 0, Constants.GLOBAL2.Length);
			foreach (BIFF8Style style in this.m_styles)
			{
				RecordFactory.XF(writer, style.RecordData);
			}
			this.m_styles = null;
			this.m_styleMap = null;
		}

		private void AddBuiltInFormats()
		{
			BIFF8Format item = new BIFF8Format("General", 0);
			this.m_formats.Add(item);
			this.m_formatIntMap.Add(item.Index, 0);
			this.m_formatStringMap.Add(item.String, item.Index);
			this.m_formats.Add(new BIFF8Format("0", 1));
			this.m_formats.Add(new BIFF8Format("0.00", 2));
			this.m_formats.Add(new BIFF8Format("#,##0", 3));
			this.m_formats.Add(new BIFF8Format("#,##0.00", 4));
			this.m_formats.Add(new BIFF8Format("\"$\"#,##0_);\\(\"$\"#,##0\\)", 5));
			this.m_formats.Add(new BIFF8Format("\"$\"#,##0_);[Red]\\(\"$\"#,##0\\)", 6));
			this.m_formats.Add(new BIFF8Format("\"$\"#,##0.00_);\\(\"$\"#,##0.00\\)", 7));
			this.m_formats.Add(new BIFF8Format("\"$\"#,##0.00_);[Red]\\(\"$\"#,##0.00\\)", 8));
			this.m_formats.Add(new BIFF8Format("0%", 9));
			this.m_formats.Add(new BIFF8Format("0.00E+00", 11));
			this.m_formats.Add(new BIFF8Format("#?/?", 12));
			this.m_formats.Add(new BIFF8Format("#??/??", 13));
			this.m_formats.Add(new BIFF8Format("M/D/YY", 14));
			this.m_formats.Add(new BIFF8Format("D-MMM-YY", 15));
			this.m_formats.Add(new BIFF8Format("D-MMM", 16));
			this.m_formats.Add(new BIFF8Format("MMM-YY", 17));
			this.m_formats.Add(new BIFF8Format("h:mm AM/PM", 18));
			this.m_formats.Add(new BIFF8Format("h:mm:ss AM/PM", 19));
			this.m_formats.Add(new BIFF8Format("h:mm", 20));
			this.m_formats.Add(new BIFF8Format("h:mm:ss", 21));
			this.m_formats.Add(new BIFF8Format("M/D/YYYY h:mm", 22));
			this.m_formats.Add(new BIFF8Format("(#,##0_);(#,##0)", 37));
			this.m_formats.Add(new BIFF8Format("(#,##0_);[Red](#,##0)", 38));
			this.m_formats.Add(new BIFF8Format("(#,##0.00_);(#,##0.00)", 39));
			this.m_formats.Add(new BIFF8Format("(#,##0.00_);[Red](#,##0.00)", 40));
			this.m_formats.Add(new BIFF8Format("_(* #,##0_);_(* \\(#,##0\\);_(* \"-\"_);_(@_)", 41));
			this.m_formats.Add(new BIFF8Format("_(\"$\"* #,##0_);_(\"$\"* \\(#,##0\\);_(\"$\"* \"-\"_);_(@_)", 42));
			this.m_formats.Add(new BIFF8Format("_(* #,##0.00_);_(* \\(#,##0.00\\);_(* \"-\"??_);_(@_)", 43));
			this.m_formats.Add(new BIFF8Format("_(\"$\"* #,##0.00_);_(\"$\"* \\(#,##0.00\\);_(\"$\"* \"-\"??_);_(@_)", 44));
			this.m_formats.Add(new BIFF8Format("mm:ss", 45));
			this.m_formats.Add(new BIFF8Format("[h]:mm:ss", 46));
			this.m_formats.Add(new BIFF8Format("mm:ss.0", 47));
			this.m_formats.Add(new BIFF8Format("##0.0E+0", 48));
			this.m_formats.Add(new BIFF8Format("@", 49));
		}

		private void CheckContext()
		{
			if (this.m_context == null)
			{
				this.m_context = new InstanceStyle(this);
			}
		}
	}
}
