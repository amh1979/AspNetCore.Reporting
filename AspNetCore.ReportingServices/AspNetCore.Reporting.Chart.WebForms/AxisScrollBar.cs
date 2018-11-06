using AspNetCore.Reporting.Chart.WebForms.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class AxisScrollBar
	{
		internal Axis axis;

		private bool enabled = true;

		private ScrollBarButtonStyles scrollBarButtonStyle = ScrollBarButtonStyles.All;

		private double scrollBarSize = 14.0;

		private int pressedButtonType = 2147483647;

		private Color buttonColor = Color.Empty;

		private Color backColor = Color.Empty;

		private Color lineColor = Color.Empty;

		private Color buttonCurrentColor = Color.Empty;

		private Color backCurrentColor = Color.Empty;

		private Color lineCurrentColor = Color.Empty;

		private bool positionInside = true;

		[SRCategory("CategoryAttributeAxisView")]
		[SRDescription("DescriptionAttributeAxisScrollBar_PositionInside")]
		[Bindable(true)]
		[DefaultValue(true)]
		public bool PositionInside
		{
			get
			{
				return this.positionInside;
			}
			set
			{
				if (this.positionInside != value)
				{
					this.positionInside = value;
					if (this.axis != null)
					{
						this.axis.chartArea.Invalidate(false);
					}
				}
			}
		}

		[SRDescription("DescriptionAttributeAxisScrollBar_Enabled")]
		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(true)]
		public bool Enabled
		{
			get
			{
				return this.enabled;
			}
			set
			{
				if (this.enabled != value)
				{
					this.enabled = value;
					if (this.axis != null)
					{
						this.axis.chartArea.Invalidate(false);
					}
				}
			}
		}

		[Bindable(false)]
		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ChartArea ChartArea
		{
			get
			{
				return this.axis.chartArea;
			}
		}

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[Bindable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Axis Axis
		{
			get
			{
				return this.axis;
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAxisView")]
		[DefaultValue(ScrollBarButtonStyles.All)]
		[SRDescription("DescriptionAttributeAxisScrollBar_Buttons")]
		public ScrollBarButtonStyles Buttons
		{
			get
			{
				return this.scrollBarButtonStyle;
			}
			set
			{
				if (this.scrollBarButtonStyle != value)
				{
					this.scrollBarButtonStyle = value;
					if (this.axis != null)
					{
						this.axis.chartArea.Invalidate(false);
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[Bindable(true)]
		[DefaultValue(14.0)]
		[SRDescription("DescriptionAttributeAxisScrollBar_Size")]
		public double Size
		{
			get
			{
				return this.scrollBarSize;
			}
			set
			{
				if (this.scrollBarSize == value)
				{
					return;
				}
				if (!(value < 5.0) && !(value > 20.0))
				{
					this.scrollBarSize = value;
					if (this.axis != null)
					{
						this.axis.chartArea.Invalidate(false);
					}
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionScrollBarSizeInvalid);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAxisView")]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeAxisScrollBar_ButtonColor")]
		public Color ButtonColor
		{
			get
			{
				return this.buttonColor;
			}
			set
			{
				if (this.buttonColor != value)
				{
					this.buttonColor = value;
					if (this.axis != null)
					{
						this.axis.chartArea.Invalidate(false);
					}
				}
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttributeAxisView")]
		[SRDescription("DescriptionAttributeAxisScrollBar_LineColor")]
		[DefaultValue(typeof(Color), "")]
		public Color LineColor
		{
			get
			{
				return this.lineColor;
			}
			set
			{
				if (this.lineColor != value)
				{
					this.lineColor = value;
					if (this.axis != null)
					{
						this.axis.chartArea.Invalidate(false);
					}
				}
			}
		}

		[SRCategory("CategoryAttributeAxisView")]
		[SRDescription("DescriptionAttributeAxisScrollBar_BackColor")]
		[Bindable(true)]
		[DefaultValue(typeof(Color), "")]
		public Color BackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				if (this.backColor != value)
				{
					this.backColor = value;
					if (this.axis != null)
					{
						this.axis.chartArea.Invalidate(false);
					}
				}
			}
		}

		public AxisScrollBar()
		{
		}

		public AxisScrollBar(Axis axis)
		{
			this.axis = axis;
		}

		internal void Initialize()
		{
		}

		internal bool IsVisible()
		{
			return false;
		}

		internal void Paint(ChartGraphics graph)
		{
			int num = 1;
			if (this.IsVisible())
			{
				this.buttonCurrentColor = this.buttonColor;
				this.backCurrentColor = this.backColor;
				this.lineCurrentColor = this.lineColor;
				if (this.buttonCurrentColor == Color.Empty)
				{
					this.buttonCurrentColor = this.axis.chartArea.BackColor;
					if (this.buttonCurrentColor == Color.Empty)
					{
						this.buttonCurrentColor = Color.DarkGray;
					}
				}
				if (this.backCurrentColor == Color.Empty)
				{
					this.backCurrentColor = this.axis.chartArea.BackColor;
					if (this.backCurrentColor == Color.Empty)
					{
						this.backCurrentColor = Color.LightGray;
					}
				}
				if (this.lineCurrentColor == Color.Empty)
				{
					this.lineCurrentColor = this.axis.LineColor;
					if (this.lineCurrentColor == Color.Empty)
					{
						this.lineCurrentColor = Color.Black;
					}
				}
				RectangleF scrollBarRect = this.GetScrollBarRect();
				graph.FillRectangleRel(scrollBarRect, this.backCurrentColor, ChartHatchStyle.None, "", ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, this.lineCurrentColor, num, ChartDashStyle.Solid, Color.Empty, 0, PenAlignment.Outset);
				this.PaintScrollBarConnectionRect(graph, scrollBarRect, num);
				SizeF size = new SizeF((float)num, (float)num);
				size = graph.GetRelativeSize(size);
				RectangleF scrollBarClientRect = new RectangleF(scrollBarRect.Location, scrollBarRect.Size);
				scrollBarClientRect.Inflate((float)(0.0 - size.Width), (float)(0.0 - size.Height));
				foreach (ScrollBarButtonType value in Enum.GetValues(typeof(ScrollBarButtonType)))
				{
					RectangleF scrollBarButtonRect = this.GetScrollBarButtonRect(scrollBarClientRect, value);
					if (!scrollBarButtonRect.IsEmpty)
					{
						this.PaintScrollBar3DButton(graph, scrollBarButtonRect, this.pressedButtonType == (int)value, value);
					}
				}
			}
		}

		private void PaintScrollBarConnectionRect(ChartGraphics graph, RectangleF scrollBarRect, int borderWidth)
		{
			if (this.axis.AxisPosition != 0 && this.axis.AxisPosition != AxisPosition.Right)
			{
				float num = 0f;
				float num2 = 0f;
				Axis[] axes = this.axis.chartArea.Axes;
				foreach (Axis axis in axes)
				{
					if (axis.AxisPosition == AxisPosition.Left && axis.ScrollBar.IsVisible() && axis.ScrollBar.PositionInside == this.axis.ScrollBar.PositionInside)
					{
						num = (float)axis.ScrollBar.GetScrollBarRelativeSize();
					}
					if (axis.AxisPosition == AxisPosition.Right && axis.ScrollBar.IsVisible() && axis.ScrollBar.PositionInside == this.axis.ScrollBar.PositionInside)
					{
						num2 = (float)axis.ScrollBar.GetScrollBarRelativeSize();
					}
				}
				RectangleF rectF = new RectangleF(scrollBarRect.Location, scrollBarRect.Size);
				if (num > 0.0)
				{
					rectF.X = scrollBarRect.X - num;
					rectF.Width = num;
					graph.FillRectangleRel(rectF, this.backCurrentColor, ChartHatchStyle.None, "", ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, this.lineCurrentColor, borderWidth, ChartDashStyle.Solid, Color.Empty, 0, PenAlignment.Outset);
				}
				if (num2 > 0.0)
				{
					rectF.X = scrollBarRect.Right;
					rectF.Width = num2;
					graph.FillRectangleRel(rectF, this.backCurrentColor, ChartHatchStyle.None, "", ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, this.lineCurrentColor, borderWidth, ChartDashStyle.Solid, Color.Empty, 0, PenAlignment.Outset);
				}
			}
		}

		internal void PaintScrollBar3DButton(ChartGraphics graph, RectangleF buttonRect, bool pressedState, ScrollBarButtonType buttonType)
		{
			if (buttonType != ScrollBarButtonType.LargeIncrement && buttonType != ScrollBarButtonType.LargeDecrement)
			{
				Color gradientColor = ChartGraphics.GetGradientColor(this.buttonCurrentColor, Color.Black, 0.5);
				Color gradientColor2 = ChartGraphics.GetGradientColor(this.buttonCurrentColor, Color.Black, 0.8);
				Color gradientColor3 = ChartGraphics.GetGradientColor(this.buttonCurrentColor, Color.White, 0.5);
				graph.FillRectangleRel(buttonRect, this.buttonCurrentColor, ChartHatchStyle.None, "", ChartImageWrapMode.Tile, Color.Empty, ChartImageAlign.Center, GradientType.None, Color.Empty, gradientColor, pressedState ? 1 : 0, ChartDashStyle.Solid, Color.Empty, 0, PenAlignment.Outset);
				bool flag = this.Size <= 12.0;
				if (!pressedState)
				{
					SizeF size = new SizeF(1f, 1f);
					size = graph.GetRelativeSize(size);
					graph.DrawLineRel(flag ? gradientColor3 : this.buttonCurrentColor, 1, ChartDashStyle.Solid, new PointF(buttonRect.X, buttonRect.Bottom), new PointF(buttonRect.X, buttonRect.Top));
					graph.DrawLineRel(flag ? gradientColor3 : this.buttonCurrentColor, 1, ChartDashStyle.Solid, new PointF(buttonRect.Left, buttonRect.Y), new PointF(buttonRect.Right, buttonRect.Y));
					graph.DrawLineRel(flag ? gradientColor : gradientColor2, 1, ChartDashStyle.Solid, new PointF(buttonRect.Right, buttonRect.Bottom), new PointF(buttonRect.Right, buttonRect.Top));
					graph.DrawLineRel(flag ? gradientColor : gradientColor2, 1, ChartDashStyle.Solid, new PointF(buttonRect.Left, buttonRect.Bottom), new PointF(buttonRect.Right, buttonRect.Bottom));
					if (!flag)
					{
						graph.DrawLineRel(gradientColor, 1, ChartDashStyle.Solid, new PointF(buttonRect.Right - size.Width, buttonRect.Bottom - size.Height), new PointF(buttonRect.Right - size.Width, buttonRect.Top + size.Height));
						graph.DrawLineRel(gradientColor, 1, ChartDashStyle.Solid, new PointF(buttonRect.Left + size.Width, buttonRect.Bottom - size.Height), new PointF(buttonRect.Right - size.Width, buttonRect.Bottom - size.Height));
						graph.DrawLineRel(gradientColor3, 1, ChartDashStyle.Solid, new PointF(buttonRect.X + size.Width, buttonRect.Bottom - size.Height), new PointF(buttonRect.X + size.Width, buttonRect.Top + size.Height));
						graph.DrawLineRel(gradientColor3, 1, ChartDashStyle.Solid, new PointF(buttonRect.Left + size.Width, buttonRect.Y + size.Height), new PointF(buttonRect.Right - size.Width, buttonRect.Y + size.Height));
					}
				}
				bool flag2 = (byte)((this.axis.AxisPosition == AxisPosition.Left || this.axis.AxisPosition == AxisPosition.Right) ? 1 : 0) != 0;
				float num = (float)(flag ? 0.5 : 1.0);
				if (pressedState)
				{
					graph.TranslateTransform(num, num);
				}
				RectangleF absoluteRectangle = graph.GetAbsoluteRectangle(buttonRect);
				float num2 = (float)(flag ? 2 : 3);
				switch (buttonType)
				{
				case ScrollBarButtonType.SmallDecrement:
				{
					PointF[] array2 = new PointF[3];
					if (flag2)
					{
						array2[0].X = absoluteRectangle.X + num2;
						array2[0].Y = (float)(absoluteRectangle.Y + (num2 + 1.0));
						array2[1].X = (float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0);
						array2[1].Y = absoluteRectangle.Bottom - num2;
						array2[2].X = absoluteRectangle.Right - num2;
						array2[2].Y = (float)(absoluteRectangle.Y + (num2 + 1.0));
					}
					else
					{
						array2[0].X = absoluteRectangle.X + num2;
						array2[0].Y = (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0);
						array2[1].X = (float)(absoluteRectangle.Right - (num2 + 1.0));
						array2[1].Y = absoluteRectangle.Y + num2;
						array2[2].X = (float)(absoluteRectangle.Right - (num2 + 1.0));
						array2[2].Y = absoluteRectangle.Bottom - num2;
					}
					graph.FillPolygon(new SolidBrush(this.lineCurrentColor), array2);
					break;
				}
				case ScrollBarButtonType.SmallIncrement:
				{
					PointF[] array = new PointF[3];
					if (flag2)
					{
						array[0].X = absoluteRectangle.X + num2;
						array[0].Y = (float)(absoluteRectangle.Bottom - (num2 + 1.0));
						array[1].X = (float)(absoluteRectangle.X + absoluteRectangle.Width / 2.0);
						array[1].Y = absoluteRectangle.Y + num2;
						array[2].X = absoluteRectangle.Right - num2;
						array[2].Y = (float)(absoluteRectangle.Bottom - (num2 + 1.0));
					}
					else
					{
						array[0].X = absoluteRectangle.Right - num2;
						array[0].Y = (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0);
						array[1].X = (float)(absoluteRectangle.X + (num2 + 1.0));
						array[1].Y = absoluteRectangle.Y + num2;
						array[2].X = (float)(absoluteRectangle.X + (num2 + 1.0));
						array[2].Y = absoluteRectangle.Bottom - num2;
					}
					graph.FillPolygon(new SolidBrush(this.lineCurrentColor), array);
					break;
				}
				case ScrollBarButtonType.ZoomReset:
				{
					Pen pen = new Pen(this.lineCurrentColor, 1f);
					graph.DrawEllipse(pen, (float)(absoluteRectangle.X + num2 - 0.5), (float)(absoluteRectangle.Y + num2 - 0.5), (float)(absoluteRectangle.Width - 2.0 * num2), (float)(absoluteRectangle.Height - 2.0 * num2));
					graph.DrawLine(pen, (float)(absoluteRectangle.X + num2 + 1.5), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0 - 0.5), (float)(absoluteRectangle.Right - num2 - 2.5), (float)(absoluteRectangle.Y + absoluteRectangle.Height / 2.0 - 0.5));
					break;
				}
				}
				if (pressedState)
				{
					graph.TranslateTransform((float)(0.0 - num), (float)(0.0 - num));
				}
			}
		}

		internal RectangleF GetScrollBarButtonRect(RectangleF scrollBarClientRect, ScrollBarButtonType buttonType)
		{
			RectangleF result = new RectangleF(scrollBarClientRect.Location, scrollBarClientRect.Size);
			bool flag = (byte)((this.axis.AxisPosition == AxisPosition.Left || this.axis.AxisPosition == AxisPosition.Right) ? 1 : 0) != 0;
			SizeF size = new SizeF(1f, 1f);
			size = this.GetRelativeSize(size);
			SizeF sizeF = new SizeF(scrollBarClientRect.Width, scrollBarClientRect.Height);
			sizeF = this.GetAbsoluteSize(sizeF);
			if (flag)
			{
				sizeF.Height = sizeF.Width;
			}
			else
			{
				sizeF.Width = sizeF.Height;
			}
			sizeF = this.GetRelativeSize(sizeF);
			result.Width = sizeF.Width;
			result.Height = sizeF.Height;
			if (flag)
			{
				result.X = scrollBarClientRect.X;
			}
			else
			{
				result.Y = scrollBarClientRect.Y;
			}
			switch (buttonType)
			{
			case ScrollBarButtonType.ThumbTracker:
			case ScrollBarButtonType.LargeDecrement:
			case ScrollBarButtonType.LargeIncrement:
				if (flag)
				{
					double num = (double)(scrollBarClientRect.Height - (float)this.GetButtonsNumberAll() * sizeF.Height);
					result.Height = (float)(this.GetDataViewPercentage() * (num / 100.0));
					if (result.Height < size.Height * 6.0)
					{
						result.Height = (float)(size.Height * 6.0);
					}
					if (!this.axis.Reverse)
					{
						result.Y = scrollBarClientRect.Bottom - (float)this.GetButtonsNumberBottom() * sizeF.Height - result.Height;
						result.Y -= (float)(this.GetDataViewPositionPercentage() * (num / 100.0));
						if (result.Y < scrollBarClientRect.Y + (float)this.GetButtonsNumberTop() * sizeF.Height + ((this.GetButtonsNumberTop() == 0) ? 0.0 : size.Height))
						{
							result.Y = (float)(scrollBarClientRect.Y + (float)this.GetButtonsNumberTop() * sizeF.Height + ((this.GetButtonsNumberTop() == 0) ? 0.0 : size.Height));
						}
					}
					else
					{
						result.Y = scrollBarClientRect.Top + (float)this.GetButtonsNumberTop() * sizeF.Height;
						result.Y += (float)(this.GetDataViewPositionPercentage() * (num / 100.0));
						if (result.Y + result.Height > scrollBarClientRect.Bottom - (float)this.GetButtonsNumberBottom() * sizeF.Height - ((this.GetButtonsNumberBottom() == 0) ? 0.0 : size.Height))
						{
							result.Y = (float)(scrollBarClientRect.Bottom - (float)this.GetButtonsNumberBottom() * sizeF.Height - result.Height - ((this.GetButtonsNumberBottom() == 0) ? 0.0 : size.Height));
						}
					}
				}
				else
				{
					double num2 = (double)(scrollBarClientRect.Width - (float)this.GetButtonsNumberAll() * sizeF.Width);
					result.Width = (float)(this.GetDataViewPercentage() * (num2 / 100.0));
					if (result.Width < size.Width * 6.0)
					{
						result.Width = (float)(size.Width * 6.0);
					}
					if (!this.axis.Reverse)
					{
						result.X = scrollBarClientRect.X + (float)this.GetButtonsNumberTop() * sizeF.Width;
						result.X += (float)(this.GetDataViewPositionPercentage() * (num2 / 100.0));
						if (result.X + result.Width > scrollBarClientRect.Right - (float)this.GetButtonsNumberBottom() * sizeF.Width - ((this.GetButtonsNumberBottom() == 0) ? 0.0 : size.Width))
						{
							result.X = (float)(scrollBarClientRect.Right - result.Width - (float)this.GetButtonsNumberBottom() * sizeF.Width - ((this.GetButtonsNumberBottom() == 0) ? 0.0 : size.Width));
						}
					}
					else
					{
						result.X = (float)(scrollBarClientRect.Right - (float)this.GetButtonsNumberBottom() * sizeF.Width - ((this.GetButtonsNumberBottom() == 0) ? 0.0 : size.Width) - result.Width);
						result.X -= (float)(this.GetDataViewPositionPercentage() * (num2 / 100.0));
						if (result.X < scrollBarClientRect.X + (float)this.GetButtonsNumberTop() * sizeF.Width)
						{
							result.X = scrollBarClientRect.X + (float)this.GetButtonsNumberTop() * sizeF.Width;
						}
					}
				}
				switch (buttonType)
				{
				case ScrollBarButtonType.LargeDecrement:
					if (flag)
					{
						result.Y = result.Bottom + size.Height;
						result.Height = scrollBarClientRect.Bottom - (float)this.GetButtonsNumberBottom() * sizeF.Height - size.Height - result.Y;
					}
					else
					{
						float num4 = scrollBarClientRect.X + (float)this.GetButtonsNumberTop() * sizeF.Width + size.Width;
						result.Width = result.X - num4;
						result.X = num4;
					}
					break;
				case ScrollBarButtonType.LargeIncrement:
					if (flag)
					{
						float num3 = scrollBarClientRect.Y + (float)this.GetButtonsNumberTop() * sizeF.Height + size.Height;
						result.Height = result.Y - num3;
						result.Y = num3;
					}
					else
					{
						result.X = result.Right + size.Width;
						result.Width = scrollBarClientRect.Right - (float)this.GetButtonsNumberBottom() * sizeF.Width - size.Height - result.X;
					}
					break;
				}
				break;
			case ScrollBarButtonType.SmallDecrement:
				if (this.scrollBarButtonStyle != ScrollBarButtonStyles.All && this.scrollBarButtonStyle != ScrollBarButtonStyles.SmallScroll)
				{
					result = RectangleF.Empty;
					return result;
				}
				if (flag)
				{
					result.Y = scrollBarClientRect.Bottom - result.Height;
				}
				else
				{
					result.X = (float)(scrollBarClientRect.X + ((float)this.GetButtonsNumberTop() - 1.0) * sizeF.Width);
					result.X += (float)((this.GetButtonsNumberTop() == 1) ? 0.0 : size.Width);
				}
				break;
			case ScrollBarButtonType.SmallIncrement:
				if (this.scrollBarButtonStyle != ScrollBarButtonStyles.All && this.scrollBarButtonStyle != ScrollBarButtonStyles.SmallScroll)
				{
					result = RectangleF.Empty;
					return result;
				}
				if (flag)
				{
					result.Y = (float)(scrollBarClientRect.Y + ((float)this.GetButtonsNumberTop() - 1.0) * sizeF.Height);
					result.Y += (float)((this.GetButtonsNumberTop() == 1) ? 0.0 : size.Height);
				}
				else
				{
					result.X = scrollBarClientRect.Right - result.Width;
				}
				break;
			case ScrollBarButtonType.ZoomReset:
				if (this.scrollBarButtonStyle != ScrollBarButtonStyles.All && this.scrollBarButtonStyle != ScrollBarButtonStyles.ResetZoom)
				{
					result = RectangleF.Empty;
					return result;
				}
				if (flag)
				{
					result.Y = scrollBarClientRect.Y;
				}
				else
				{
					result.X = scrollBarClientRect.X;
				}
				break;
			}
			return result;
		}

		internal RectangleF GetScrollBarRect()
		{
			float num = (float)this.GetScrollBarRelativeSize();
			RectangleF rectangleF = this.axis.PlotAreaPosition.ToRectangleF();
			if (!this.PositionInside)
			{
				rectangleF = this.axis.chartArea.Position.ToRectangleF();
				Axis[] axes = this.ChartArea.Axes;
				foreach (Axis axis in axes)
				{
					if (axis.ScrollBar.IsVisible() && !axis.ScrollBar.PositionInside)
					{
						float num2 = (float)axis.ScrollBar.GetScrollBarRelativeSize();
						switch (axis.AxisPosition)
						{
						case AxisPosition.Left:
							rectangleF.X += num2;
							rectangleF.Width -= num2;
							break;
						case AxisPosition.Right:
							rectangleF.Width -= num2;
							break;
						case AxisPosition.Bottom:
							rectangleF.Height -= num2;
							break;
						case AxisPosition.Top:
							rectangleF.Y += num2;
							rectangleF.Height -= num2;
							break;
						}
					}
				}
			}
			RectangleF empty = RectangleF.Empty;
			if (this.axis.PlotAreaPosition != null)
			{
				switch (this.axis.AxisPosition)
				{
				case AxisPosition.Left:
					empty.Y = rectangleF.Y;
					empty.Height = rectangleF.Height;
					empty.X = (this.PositionInside ? ((float)this.axis.GetAxisPosition(true)) : rectangleF.X) - num;
					empty.Width = num;
					break;
				case AxisPosition.Right:
					empty.Y = rectangleF.Y;
					empty.Height = rectangleF.Height;
					empty.X = (this.PositionInside ? ((float)this.axis.GetAxisPosition(true)) : rectangleF.Right);
					empty.Width = num;
					break;
				case AxisPosition.Bottom:
					empty.X = rectangleF.X;
					empty.Width = rectangleF.Width;
					empty.Y = (this.PositionInside ? ((float)this.axis.GetAxisPosition(true)) : rectangleF.Bottom);
					empty.Height = num;
					break;
				case AxisPosition.Top:
					empty.X = rectangleF.X;
					empty.Width = rectangleF.Width;
					empty.Y = (this.PositionInside ? ((float)this.axis.GetAxisPosition(true)) : rectangleF.Y) - num;
					empty.Height = num;
					break;
				}
			}
			return empty;
		}

		internal double GetScrollBarRelativeSize()
		{
			if (!this.axis.chartArea.Area3DStyle.Enable3D && !this.axis.chartArea.chartAreaIsCurcular)
			{
				if (this.axis.AxisPosition != 0 && this.axis.AxisPosition != AxisPosition.Right)
				{
					return this.scrollBarSize * 100.0 / (double)(float)(this.axis.Common.Height - 1);
				}
				return this.scrollBarSize * 100.0 / (double)(float)(this.axis.Common.Width - 1);
			}
			return 0.0;
		}

		private double GetDataViewPercentage()
		{
			double result = 100.0;
			if (this.axis != null && !double.IsNaN(this.axis.View.Position) && !double.IsNaN(this.axis.View.Size))
			{
				double intervalSize = this.axis.GetIntervalSize(this.axis.View.Position, this.axis.View.Size, this.axis.View.SizeType);
				double num = this.axis.minimum + this.axis.marginView;
				double num2 = this.axis.maximum - this.axis.marginView;
				if (this.axis.View.Position < num)
				{
					num = this.axis.View.Position;
				}
				if (this.axis.View.Position + intervalSize > num2)
				{
					num2 = this.axis.View.Position + intervalSize;
				}
				double num3 = Math.Abs(num - num2);
				if (intervalSize < num3)
				{
					result = intervalSize / (num3 / 100.0);
				}
			}
			return result;
		}

		private double GetDataViewPositionPercentage()
		{
			double result = 0.0;
			if (this.axis != null && !double.IsNaN(this.axis.View.Position) && !double.IsNaN(this.axis.View.Size))
			{
				double intervalSize = this.axis.GetIntervalSize(this.axis.View.Position, this.axis.View.Size, this.axis.View.SizeType);
				double num = this.axis.minimum + this.axis.marginView;
				double num2 = this.axis.maximum - this.axis.marginView;
				if (this.axis.View.Position < num)
				{
					num = this.axis.View.Position;
				}
				if (this.axis.View.Position + intervalSize > num2)
				{
					num2 = this.axis.View.Position + intervalSize;
				}
				double num3 = Math.Abs(num - num2);
				result = (this.axis.View.Position - num) / (num3 / 100.0);
			}
			return result;
		}

		private int GetButtonsNumberAll()
		{
			int num = 0;
			if ((this.scrollBarButtonStyle & ScrollBarButtonStyles.ResetZoom) == ScrollBarButtonStyles.ResetZoom)
			{
				num++;
			}
			if ((this.scrollBarButtonStyle & ScrollBarButtonStyles.SmallScroll) == ScrollBarButtonStyles.SmallScroll)
			{
				num += 2;
			}
			return num;
		}

		private int GetButtonsNumberTop()
		{
			int num = 0;
			if ((this.scrollBarButtonStyle & ScrollBarButtonStyles.ResetZoom) == ScrollBarButtonStyles.ResetZoom)
			{
				num++;
			}
			if ((this.scrollBarButtonStyle & ScrollBarButtonStyles.SmallScroll) == ScrollBarButtonStyles.SmallScroll)
			{
				num++;
			}
			return num;
		}

		private int GetButtonsNumberBottom()
		{
			int num = 0;
			if ((this.scrollBarButtonStyle & ScrollBarButtonStyles.SmallScroll) == ScrollBarButtonStyles.SmallScroll)
			{
				num++;
			}
			return num;
		}

		internal SizeF GetAbsoluteSize(SizeF relative)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = (float)(relative.Width * (float)(this.axis.Common.Width - 1) / 100.0);
			empty.Height = (float)(relative.Height * (float)(this.axis.Common.Height - 1) / 100.0);
			return empty;
		}

		internal SizeF GetRelativeSize(SizeF size)
		{
			SizeF empty = SizeF.Empty;
			empty.Width = (float)(size.Width * 100.0 / (float)(this.axis.Common.Width - 1));
			empty.Height = (float)(size.Height * 100.0 / (float)(this.axis.Common.Height - 1));
			return empty;
		}
	}
}
