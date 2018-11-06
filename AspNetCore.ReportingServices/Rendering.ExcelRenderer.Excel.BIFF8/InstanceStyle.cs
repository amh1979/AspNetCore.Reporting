namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class InstanceStyle : StyleState
	{
		private BIFF8Style m_xf;

		private BIFF8Font m_font;

		private string m_format;

		private bool m_fontModified;

		public override ExcelBorderStyle BorderLeftStyle
		{
			set
			{
				this.m_xf.BorderLeftStyle = value;
			}
		}

		public override ExcelBorderStyle BorderRightStyle
		{
			set
			{
				this.m_xf.BorderRightStyle = value;
			}
		}

		public override ExcelBorderStyle BorderTopStyle
		{
			set
			{
				this.m_xf.BorderTopStyle = value;
			}
		}

		public override ExcelBorderStyle BorderBottomStyle
		{
			set
			{
				this.m_xf.BorderBottomStyle = value;
			}
		}

		public override ExcelBorderStyle BorderDiagStyle
		{
			set
			{
				this.m_xf.BorderDiagStyle = value;
			}
		}

		public override IColor BorderLeftColor
		{
			set
			{
				this.m_xf.BorderLeftColor = value;
			}
		}

		public override IColor BorderRightColor
		{
			set
			{
				this.m_xf.BorderRightColor = value;
			}
		}

		public override IColor BorderTopColor
		{
			set
			{
				this.m_xf.BorderTopColor = value;
			}
		}

		public override IColor BorderBottomColor
		{
			set
			{
				this.m_xf.BorderBottomColor = value;
			}
		}

		public override IColor BorderDiagColor
		{
			set
			{
				this.m_xf.BorderDiagColor = value;
			}
		}

		public override ExcelBorderPart BorderDiagPart
		{
			set
			{
				this.m_xf.BorderDiagPart = value;
			}
		}

		public override IColor BackgroundColor
		{
			set
			{
				this.m_xf.BackgroundColor = value;
			}
		}

		public override int IndentLevel
		{
			set
			{
				this.m_xf.IndentLevel = value;
			}
		}

		public override bool WrapText
		{
			set
			{
				this.m_xf.WrapText = value;
			}
		}

		public override int Orientation
		{
			set
			{
				this.m_xf.Orientation = value;
			}
		}

		public override string NumberFormat
		{
			get
			{
				this.InitFormat();
				return this.m_format;
			}
			set
			{
				this.InitFormat();
				if (!this.m_format.Equals(value))
				{
					this.m_format = value;
				}
			}
		}

		public override HorizontalAlignment HorizontalAlignment
		{
			set
			{
				this.m_xf.HorizontalAlignment = value;
			}
		}

		public override VerticalAlignment VerticalAlignment
		{
			set
			{
				this.m_xf.VerticalAlignment = value;
			}
		}

		public override TextDirection TextDirection
		{
			set
			{
				this.m_xf.TextDirection = value;
			}
		}

		public override int Bold
		{
			set
			{
				if (this.m_fontModified)
				{
					this.GetFont().Bold = value;
				}
				else if (this.GetFont().Bold != value)
				{
					this.CloneFont();
					this.m_font.Bold = value;
				}
			}
		}

		public override bool Italic
		{
			set
			{
				if (this.m_fontModified)
				{
					this.GetFont().Italic = value;
				}
				else if (this.GetFont().Italic != value)
				{
					this.CloneFont();
					this.m_font.Italic = value;
				}
			}
		}

		public override bool Strikethrough
		{
			set
			{
				if (this.m_fontModified)
				{
					this.GetFont().Strikethrough = value;
				}
				else if (this.GetFont().Strikethrough != value)
				{
					this.CloneFont();
					this.m_font.Strikethrough = value;
				}
			}
		}

		public override ScriptStyle ScriptStyle
		{
			set
			{
				if (this.m_fontModified)
				{
					this.GetFont().ScriptStyle = value;
				}
				else if (this.GetFont().ScriptStyle != value)
				{
					this.CloneFont();
					this.m_font.ScriptStyle = value;
				}
			}
		}

		public override IColor Color
		{
			set
			{
				int paletteIndex = ((BIFF8Color)value).PaletteIndex;
				if (this.m_fontModified)
				{
					this.GetFont().Color = paletteIndex;
				}
				else if (this.GetFont().Color != paletteIndex)
				{
					this.CloneFont();
					this.m_font.Color = paletteIndex;
				}
			}
		}

		public override Underline Underline
		{
			set
			{
				if (this.m_fontModified)
				{
					this.GetFont().Underline = value;
				}
				else if (this.GetFont().Underline != value)
				{
					this.CloneFont();
					this.m_font.Underline = value;
				}
			}
		}

		public override string Name
		{
			set
			{
				if (this.m_fontModified)
				{
					this.GetFont().Name = value;
				}
				else if (!this.GetFont().Name.Equals(value))
				{
					this.CloneFont();
					this.m_font.Name = value;
				}
			}
		}

		public override double Size
		{
			set
			{
				if (this.m_fontModified)
				{
					this.GetFont().Size = value;
				}
				else if (this.GetFont().Size != value)
				{
					this.CloneFont();
					this.m_font.Size = value;
				}
			}
		}

		internal InstanceStyle(StyleContainer parent, StyleProperties props)
			: base(parent)
		{
			if (props.Ixfe != 0)
			{
				this.m_xf = (BIFF8Style)base.m_context.GetStyle(props.Ixfe).Clone();
			}
			else
			{
				this.m_xf = new BIFF8Style(props);
				this.m_font = new BIFF8Font(props);
				this.m_fontModified = true;
				this.m_format = props.NumberFormat;
			}
		}

		internal InstanceStyle(StyleContainer aParent)
			: base(aParent)
		{
			this.m_xf = new BIFF8Style();
		}

		internal override void Finished()
		{
			if (this.m_font != null)
			{
				this.m_xf.Ifnt = base.m_context.AddFont(this.m_font);
				this.m_font = null;
			}
			if (this.m_format != null)
			{
				this.m_xf.Ifmt = base.m_context.AddFormat(this.m_format);
				this.m_format = null;
			}
			base.m_context.CellIxfe = base.m_context.AddStyle(this.m_xf);
		}

		private BIFF8Font GetFont()
		{
			if (this.m_font == null)
			{
				if (this.m_xf.Ifnt != 0)
				{
					this.m_font = base.m_context.GetFont(this.m_xf.Ifnt);
				}
				else
				{
					this.m_font = new BIFF8Font();
				}
			}
			return this.m_font;
		}

		private void CloneFont()
		{
			this.m_font = (BIFF8Font)this.m_font.Clone();
			this.m_fontModified = true;
		}

		private void InitFormat()
		{
			if (this.m_format == null)
			{
				if (this.m_xf.Ifmt != 0)
				{
					this.m_format = base.m_context.GetFormat(this.m_xf.Ifmt);
				}
				else
				{
					this.m_format = string.Empty;
				}
			}
		}
	}
}
