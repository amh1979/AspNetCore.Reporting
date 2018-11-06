namespace AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel.BIFF8
{
	internal sealed class UseSharedStyle : StyleState
	{
		private StyleProperties m_styleProps;

		public override ExcelBorderStyle BorderLeftStyle
		{
			set
			{
				if (this.m_styleProps.BorderLeftStyle != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.BorderLeftStyle = value;
				}
			}
		}

		public override ExcelBorderStyle BorderRightStyle
		{
			set
			{
				if (this.m_styleProps.BorderRightStyle != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.BorderRightStyle = value;
				}
			}
		}

		public override ExcelBorderStyle BorderTopStyle
		{
			set
			{
				if (this.m_styleProps.BorderTopStyle != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.BorderTopStyle = value;
				}
			}
		}

		public override ExcelBorderStyle BorderBottomStyle
		{
			set
			{
				if (this.m_styleProps.BorderBottomStyle != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.BorderBottomStyle = value;
				}
			}
		}

		public override ExcelBorderStyle BorderDiagStyle
		{
			set
			{
				if (this.m_styleProps.BorderDiagStyle != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.BorderDiagStyle = value;
				}
			}
		}

		public override IColor BorderLeftColor
		{
			set
			{
				if (this.m_styleProps.BorderLeftColor != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.BorderLeftColor = value;
				}
			}
		}

		public override IColor BorderRightColor
		{
			set
			{
				if (this.m_styleProps.BorderRightColor != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.BorderRightColor = value;
				}
			}
		}

		public override IColor BorderTopColor
		{
			set
			{
				if (this.m_styleProps.BorderTopColor != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.BorderTopColor = value;
				}
			}
		}

		public override IColor BorderBottomColor
		{
			set
			{
				if (this.m_styleProps.BorderBottomColor != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.BorderBottomColor = value;
				}
			}
		}

		public override IColor BorderDiagColor
		{
			set
			{
				if (this.m_styleProps.BorderDiagColor != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.BorderDiagColor = value;
				}
			}
		}

		public override ExcelBorderPart BorderDiagPart
		{
			set
			{
				if (this.m_styleProps.BorderDiagPart != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.BorderDiagPart = value;
				}
			}
		}

		public override IColor BackgroundColor
		{
			set
			{
				if (this.m_styleProps.BackgroundColor != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.BackgroundColor = value;
				}
			}
		}

		public override int IndentLevel
		{
			set
			{
				if (this.m_styleProps.IndentLevel != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.IndentLevel = value;
				}
			}
		}

		public override bool WrapText
		{
			set
			{
				if (this.m_styleProps.WrapText != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.WrapText = value;
				}
			}
		}

		public override int Orientation
		{
			set
			{
				if (this.m_styleProps.Orientation != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.Orientation = value;
				}
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
				if (this.m_styleProps.NumberFormat != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.NumberFormat = value;
				}
			}
		}

		public override HorizontalAlignment HorizontalAlignment
		{
			set
			{
				if (this.m_styleProps.HorizontalAlignment != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.HorizontalAlignment = value;
				}
			}
		}

		public override VerticalAlignment VerticalAlignment
		{
			set
			{
				if (this.m_styleProps.VerticalAlignment != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.VerticalAlignment = value;
				}
			}
		}

		public override TextDirection TextDirection
		{
			set
			{
				if (this.m_styleProps.TextDirection != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.TextDirection = value;
				}
			}
		}

		public override int Bold
		{
			set
			{
				if (this.m_styleProps.Bold != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.Bold = value;
				}
			}
		}

		public override bool Italic
		{
			set
			{
				if (this.m_styleProps.Italic != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.Italic = value;
				}
			}
		}

		public override bool Strikethrough
		{
			set
			{
				if (this.m_styleProps.Strikethrough != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.Strikethrough = value;
				}
			}
		}

		public override ScriptStyle ScriptStyle
		{
			set
			{
				if (this.m_styleProps.ScriptStyle != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.ScriptStyle = value;
				}
			}
		}

		public override IColor Color
		{
			set
			{
				if (this.m_styleProps.Color != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.Color = value;
				}
			}
		}

		public override Underline Underline
		{
			set
			{
				if (this.m_styleProps.Underline != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.Underline = value;
				}
			}
		}

		public override string Name
		{
			set
			{
				if (this.m_styleProps.Name != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.Name = value;
				}
			}
		}

		public override double Size
		{
			set
			{
				if (this.m_styleProps.Size != value)
				{
					base.m_context.SetContext(new InstanceStyle(base.m_context, this.m_styleProps));
					base.m_context.Size = value;
				}
			}
		}

		internal UseSharedStyle(StyleContainer parent, StyleProperties props)
			: base(parent)
		{
			this.m_styleProps = props;
		}

		internal override void Finished()
		{
			if (this.m_styleProps.Ixfe != 0)
			{
				base.m_context.CellIxfe = this.m_styleProps.Ixfe;
			}
			else
			{
				this.m_styleProps.Ixfe = base.m_context.AddStyle(this.m_styleProps);
				base.m_context.CellIxfe = this.m_styleProps.Ixfe;
			}
		}
	}
}
