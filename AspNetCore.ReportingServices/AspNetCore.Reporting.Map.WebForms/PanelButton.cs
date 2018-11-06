using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class PanelButton : MapObject, IToolTipProvider
	{
		private const int ButtonPadding = 4;

		private EventHandler clickEventHandler;

		private PanelButtonStyle style;

		private PanelButtonType type;

		private RectangleF bounds = RectangleF.Empty;

		private Color borderColor = Color.DarkGray;

		private Color backColor = Color.Gray;

		private Color symbolBorderColor = Color.Black;

		private Color symbolColor = Color.White;

		private double intitalAutoRepeatDelay = 1000.0;

		private double autoRepeatDelay = 500.0;

		public PanelButtonStyle Style
		{
			get
			{
				return this.style;
			}
			set
			{
				this.style = value;
			}
		}

		public PanelButtonType Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public RectangleF Bounds
		{
			get
			{
				return this.bounds;
			}
			set
			{
				this.bounds = value;
			}
		}

		public PointF Location
		{
			get
			{
				return this.bounds.Location;
			}
			set
			{
				this.bounds.Location = value;
			}
		}

		public SizeF Size
		{
			get
			{
				return this.bounds.Size;
			}
			set
			{
				this.bounds.Size = value;
			}
		}

		public Color BorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
			}
		}

		public Color BackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
			}
		}

		public Color SymbolBorderColor
		{
			get
			{
				return this.symbolBorderColor;
			}
			set
			{
				this.symbolBorderColor = value;
			}
		}

		public Color SymbolColor
		{
			get
			{
				return this.symbolColor;
			}
			set
			{
				this.symbolColor = value;
			}
		}

		public double InititalAutoRepeatDelay
		{
			get
			{
				return this.intitalAutoRepeatDelay;
			}
			set
			{
				this.intitalAutoRepeatDelay = value;
			}
		}

		public double AutoRepeatDelay
		{
			get
			{
				return this.autoRepeatDelay;
			}
			set
			{
				this.autoRepeatDelay = value;
			}
		}

		public PanelButton(object parent)
			: this(parent, PanelButtonType.Unknown, PanelButtonStyle.Rectangle, null)
		{
		}

		public PanelButton(object parent, PanelButtonType type, EventHandler clickHandler)
			: this(parent, type, PanelButtonStyle.Rectangle, clickHandler)
		{
		}

		public PanelButton(object parent, PanelButtonType buttonType, PanelButtonStyle buttonStyle, EventHandler clickHandler)
			: base(parent)
		{
			this.Parent = parent;
			this.Type = buttonType;
			this.Style = buttonStyle;
			this.clickEventHandler = clickHandler;
		}

		public void Render(MapGraphics g)
		{
			using (GraphicsPath graphicsPath = this.GetButtonPath(g))
			{
				g.DrawPathAbs(graphicsPath, this.BackColor, MapHatchStyle.None, string.Empty, MapImageWrapMode.Unscaled, Color.Black, MapImageAlign.Center, GradientType.None, Color.White, this.BorderColor, 1, MapDashStyle.Solid, PenAlignment.Center);
				if (this.Common != null)
				{
					this.Common.MapCore.HotRegionList.SetHotRegion(g, this, graphicsPath);
				}
			}
			using (GraphicsPath path = this.GetButtonFacePath(g))
			{
				g.DrawPathAbs(path, this.SymbolColor, MapHatchStyle.None, string.Empty, MapImageWrapMode.Unscaled, Color.Black, MapImageAlign.Center, GradientType.None, Color.White, this.SymbolBorderColor, 1, MapDashStyle.Solid, PenAlignment.Center);
			}
		}

		private GraphicsPath GetButtonPath(MapGraphics g)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			switch (this.Style)
			{
			case PanelButtonStyle.Rectangle:
				graphicsPath.AddRectangle(g.GetAbsoluteRectangle(this.Bounds));
				break;
			case PanelButtonStyle.Circle:
				graphicsPath.AddEllipse(g.GetAbsoluteRectangle(this.Bounds));
				break;
			case PanelButtonStyle.RoundedRectangle:
			{
				RectangleF absoluteRectangle = g.GetAbsoluteRectangle(this.Bounds);
				if (!(absoluteRectangle.Width < 1.0) && !(absoluteRectangle.Height < 1.0))
				{
					float num = (float)(absoluteRectangle.Width / 8.0);
					float[] cornerRadius = new float[8]
					{
						num,
						num,
						num,
						num,
						num,
						num,
						num,
						num
					};
					graphicsPath.AddPath(g.CreateRoundedRectPath(absoluteRectangle, cornerRadius), false);
					break;
				}
				return graphicsPath;
			}
			case PanelButtonStyle.Triangle:
				switch (this.Type)
				{
				case PanelButtonType.NavigationButton:
					graphicsPath.AddLines(new PointF[3]
					{
						g.GetAbsolutePoint(new PointF(this.Bounds.Left, this.Bounds.Bottom)),
						g.GetAbsolutePoint(new PointF((float)((this.Bounds.Left + this.Bounds.Right) / 2.0), this.Bounds.Top)),
						g.GetAbsolutePoint(new PointF(this.Bounds.Right, this.Bounds.Bottom))
					});
					graphicsPath.CloseAllFigures();
					break;
				case PanelButtonType.NaviagateSouth:
					graphicsPath.AddLines(new PointF[3]
					{
						g.GetAbsolutePoint(new PointF(this.Bounds.Left, this.Bounds.Top)),
						g.GetAbsolutePoint(new PointF((float)((this.Bounds.Left + this.Bounds.Right) / 2.0), this.Bounds.Bottom)),
						g.GetAbsolutePoint(new PointF(this.Bounds.Right, this.Bounds.Top))
					});
					graphicsPath.CloseAllFigures();
					break;
				case PanelButtonType.NaviagateEast:
					graphicsPath.AddLines(new PointF[3]
					{
						g.GetAbsolutePoint(new PointF(this.Bounds.Left, this.Bounds.Top)),
						g.GetAbsolutePoint(new PointF(this.Bounds.Right, (float)((this.Bounds.Top + this.Bounds.Bottom) / 2.0))),
						g.GetAbsolutePoint(new PointF(this.Bounds.Left, this.Bounds.Bottom))
					});
					graphicsPath.CloseAllFigures();
					break;
				case PanelButtonType.NaviagateWest:
					graphicsPath.AddLines(new PointF[3]
					{
						g.GetAbsolutePoint(new PointF(this.Bounds.Right, this.Bounds.Top)),
						g.GetAbsolutePoint(new PointF(this.Bounds.Left, (float)((this.Bounds.Top + this.Bounds.Bottom) / 2.0))),
						g.GetAbsolutePoint(new PointF(this.Bounds.Right, this.Bounds.Bottom))
					});
					graphicsPath.CloseAllFigures();
					break;
				}
				break;
			}
			return graphicsPath;
		}

		private GraphicsPath GetButtonFacePath(MapGraphics g)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			RectangleF rectangleF = RectangleF.Inflate(g.GetAbsoluteRectangle(this.Bounds), -4f, -4f);
			if (!(rectangleF.Width < 1.0) && !(rectangleF.Height < 1.0))
			{
				float num = (float)(rectangleF.Width / 8.0);
				PointF pointF = new PointF((float)((rectangleF.Left + rectangleF.Right) / 2.0), rectangleF.Top);
				PointF pointF2 = new PointF((float)((rectangleF.Left + rectangleF.Right) / 2.0), rectangleF.Bottom);
				PointF pointF3 = new PointF(rectangleF.Left, (float)((rectangleF.Top + rectangleF.Bottom) / 2.0));
				PointF pointF4 = new PointF(rectangleF.Right, (float)((rectangleF.Top + rectangleF.Bottom) / 2.0));
				PointF pointF5 = new PointF((float)((rectangleF.Left + rectangleF.Right) / 2.0), (float)((rectangleF.Top + rectangleF.Bottom) / 2.0));
				if (this.Type == PanelButtonType.ZoomButton)
				{
					graphicsPath.AddLines(new PointF[12]
					{
						new PointF(pointF3.X, pointF3.Y - num),
						new PointF(pointF5.X - num, pointF5.Y - num),
						new PointF(pointF.X - num, pointF.Y),
						new PointF(pointF.X + num, pointF.Y),
						new PointF(pointF5.X + num, pointF5.Y - num),
						new PointF(pointF4.X, pointF4.Y - num),
						new PointF(pointF4.X, pointF4.Y + num),
						new PointF(pointF5.X + num, pointF5.Y + num),
						new PointF(pointF2.X + num, pointF2.Y),
						new PointF(pointF2.X - num, pointF2.Y),
						new PointF(pointF5.X - num, pointF5.Y + num),
						new PointF(pointF3.X, pointF3.Y + num)
					});
					graphicsPath.CloseAllFigures();
				}
				else if (this.Type == PanelButtonType.ZoomOut)
				{
					graphicsPath.AddLines(new PointF[4]
					{
						new PointF(pointF3.X, pointF3.Y - num),
						new PointF(pointF4.X, pointF3.Y - num),
						new PointF(pointF4.X, pointF3.Y + num),
						new PointF(pointF3.X, pointF3.Y + num)
					});
					graphicsPath.CloseAllFigures();
				}
				else if ((this.Type & PanelButtonType.NavigationButton) != 0)
				{
					this.CreateArrowPath(g, graphicsPath);
				}
				return graphicsPath;
			}
			return graphicsPath;
		}

		private void CreateArrowPath(MapGraphics g, GraphicsPath path)
		{
			if (this.Type != PanelButtonType.NaviagateCenter)
			{
				if (this.Style != 0 && this.Style != PanelButtonStyle.RoundedRectangle)
				{
					return;
				}
				RectangleF rectangleF = RectangleF.Inflate(g.GetAbsoluteRectangle(this.Bounds), -4f, -4f);
				float left = rectangleF.Left;
				float right = rectangleF.Right;
				float x = (float)(rectangleF.X + rectangleF.Width / 2.0);
				float x2 = (float)(rectangleF.X + rectangleF.Width * 0.30000001192092896);
				float x3 = (float)(rectangleF.X + rectangleF.Width * 0.699999988079071);
				float top = rectangleF.Top;
				float bottom = rectangleF.Bottom;
				float y = (float)(rectangleF.Y + rectangleF.Height * 0.699999988079071);
				float y2 = (float)(rectangleF.Y + rectangleF.Height / 2.0);
				path.StartFigure();
				path.AddLines(new PointF[7]
				{
					new PointF(x, top),
					new PointF(right, y),
					new PointF(x3, y),
					new PointF(x3, bottom),
					new PointF(x2, bottom),
					new PointF(x2, y),
					new PointF(left, y)
				});
				path.CloseAllFigures();
				Matrix matrix = new Matrix();
				switch (this.Type)
				{
				case PanelButtonType.NaviagateSouth:
					matrix.RotateAt(180f, new PointF(x, y2));
					break;
				case PanelButtonType.NaviagateEast:
					matrix.RotateAt(90f, new PointF(x, y2));
					break;
				case PanelButtonType.NaviagateWest:
					matrix.RotateAt(270f, new PointF(x, y2));
					break;
				}
				path.Transform(matrix);
			}
		}

		internal void DoClick()
		{
			if (this.clickEventHandler != null)
			{
				this.clickEventHandler(this, EventArgs.Empty);
			}
		}

		public string GetToolTip()
		{
			IToolTipProvider toolTipProvider = this.Parent as IToolTipProvider;
			if (toolTipProvider != null)
			{
				return toolTipProvider.GetToolTip();
			}
			return string.Empty;
		}
	}
}
