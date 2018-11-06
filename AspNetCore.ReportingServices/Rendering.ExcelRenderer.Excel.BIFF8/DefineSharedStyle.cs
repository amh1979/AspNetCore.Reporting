namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class DefineSharedStyle : StyleState
	{
		private StyleProperties m_styleProps;

		private string m_id;

		public override ExcelBorderStyle BorderLeftStyle
		{
			set
			{
				this.m_styleProps.BorderLeftStyle = value;
			}
		}

		public override ExcelBorderStyle BorderRightStyle
		{
			set
			{
				this.m_styleProps.BorderRightStyle = value;
			}
		}

		public override ExcelBorderStyle BorderTopStyle
		{
			set
			{
				this.m_styleProps.BorderTopStyle = value;
			}
		}

		public override ExcelBorderStyle BorderBottomStyle
		{
			set
			{
				this.m_styleProps.BorderBottomStyle = value;
			}
		}

		public override ExcelBorderStyle BorderDiagStyle
		{
			set
			{
				this.m_styleProps.BorderDiagStyle = value;
			}
		}

		public override IColor BorderLeftColor
		{
			set
			{
				this.m_styleProps.BorderLeftColor = value;
			}
		}

		public override IColor BorderRightColor
		{
			set
			{
				this.m_styleProps.BorderRightColor = value;
			}
		}

		public override IColor BorderTopColor
		{
			set
			{
				this.m_styleProps.BorderTopColor = value;
			}
		}

		public override IColor BorderBottomColor
		{
			set
			{
				this.m_styleProps.BorderBottomColor = value;
			}
		}

		public override IColor BorderDiagColor
		{
			set
			{
				this.m_styleProps.BorderDiagColor = value;
			}
		}

		public override ExcelBorderPart BorderDiagPart
		{
			set
			{
				this.m_styleProps.BorderDiagPart = value;
			}
		}

		public override IColor BackgroundColor
		{
			set
			{
				this.m_styleProps.BackgroundColor = value;
			}
		}

		public override int IndentLevel
		{
			set
			{
				this.m_styleProps.IndentLevel = value;
			}
		}

		public override bool WrapText
		{
			set
			{
				this.m_styleProps.WrapText = value;
			}
		}

		public override int Orientation
		{
			set
			{
				this.m_styleProps.Orientation = value;
			}
		}

		public override string NumberFormat
		{
			get
			{
				return this.m_styleProps.NumberFormat;
			}
			set
			{
				this.m_styleProps.NumberFormat = value;
			}
		}

		public override HorizontalAlignment HorizontalAlignment
		{
			set
			{
				this.m_styleProps.HorizontalAlignment = value;
			}
		}

		public override VerticalAlignment VerticalAlignment
		{
			set
			{
				this.m_styleProps.VerticalAlignment = value;
			}
		}

		public override TextDirection TextDirection
		{
			set
			{
				this.m_styleProps.TextDirection = value;
			}
		}

		public override int Bold
		{
			set
			{
				this.m_styleProps.Bold = value;
			}
		}

		public override bool Italic
		{
			set
			{
				this.m_styleProps.Italic = value;
			}
		}

		public override bool Strikethrough
		{
			set
			{
				this.m_styleProps.Strikethrough = value;
			}
		}

		public override ScriptStyle ScriptStyle
		{
			set
			{
				this.m_styleProps.ScriptStyle = value;
			}
		}

		public override IColor Color
		{
			set
			{
				this.m_styleProps.Color = value;
			}
		}

		public override Underline Underline
		{
			set
			{
				this.m_styleProps.Underline = value;
			}
		}

		public override string Name
		{
			set
			{
				this.m_styleProps.Name = value;
			}
		}

		public override double Size
		{
			set
			{
				this.m_styleProps.Size = value;
			}
		}

		internal DefineSharedStyle(StyleContainer parent, string id)
			: base(parent)
		{
			this.m_styleProps = new StyleProperties();
			this.m_id = id;
		}

		internal override void Finished()
		{
			base.m_context.AddSharedStyle(this.m_id, this.m_styleProps);
		}
	}
}
