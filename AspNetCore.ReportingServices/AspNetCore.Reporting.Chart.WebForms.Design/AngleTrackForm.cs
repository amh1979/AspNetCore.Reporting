using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Windows.Forms;

namespace AspNetCore.Reporting.Chart.WebForms.Design
{
	internal class AngleTrackForm : Control
	{
		private const int size = 170;

		private const int offset = 22;

		private Container components;

		private int angleValue;

		private bool dragging;

		private EventHandler onValueChanged;

		private Region lastUpdatedRegion = new Region();

		private bool showText = true;

		private bool showLine = true;

		private bool showMarks = true;

		private int markStep = 15;

		private bool showOnly180Degrees = true;

		private int forceRoundingToDegees = 5;

		private int prevAngleValue;

		[SRCategory("CategoryAttributeData")]
		[DefaultValue(0)]
		public int Angle
		{
			get
			{
				if (this.showOnly180Degrees && this.angleValue >= 270)
				{
					this.angleValue -= 360;
				}
				return this.angleValue;
			}
			set
			{
				if (value != this.angleValue)
				{
					this.OnValueChanged(EventArgs.Empty);
					this.angleValue = value;
				}
			}
		}

		[SRDescription("DescriptionAttributeAngleTrackFormEvent_ValueChanged")]
		public event EventHandler ValueChanged
		{
			add
			{
				this.onValueChanged = (EventHandler)Delegate.Combine(this.onValueChanged, value);
			}
			remove
			{
				this.onValueChanged = (EventHandler)Delegate.Remove(this.onValueChanged, value);
			}
		}

		public AngleTrackForm()
		{
			this.InitializeComponent();
			base.SetStyle(ControlStyles.Opaque, true);
			base.SetStyle(ControlStyles.ResizeRedraw, true);
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.components.Dispose();
			if (this.lastUpdatedRegion != null)
			{
				this.lastUpdatedRegion.Dispose();
				this.lastUpdatedRegion = null;
			}
		}

		private void InitializeComponent()
		{
			this.components = new Container();
			base.Size = new Size(170 / ((!this.showOnly180Degrees) ? 1 : 2), 170);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			this.dragging = true;
			this.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			this.dragging = false;
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			base.OnDoubleClick(e);
			this.OnValueChanged(EventArgs.Empty);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (this.dragging)
			{
				PointF pointF = new PointF((float)(e.X - (this.showOnly180Degrees ? 22 : 85)), (float)(e.Y - 85));
				if (pointF.X >= 0.0)
				{
					double num = Math.Atan((double)(pointF.Y / pointF.X));
					this.angleValue = (int)(num * 180.0 / 3.1415926535897931);
				}
				else if (!this.showOnly180Degrees)
				{
					double num = Math.Atan((double)(pointF.Y / pointF.X));
					this.angleValue = (int)(num * 180.0 / 3.1415926535897931);
					this.angleValue = 180 + this.angleValue;
				}
				if (this.angleValue < 0)
				{
					this.angleValue = 360 + this.angleValue;
				}
				if (this.showOnly180Degrees)
				{
					if (this.angleValue > 90 && this.angleValue <= 180)
					{
						this.angleValue = 90;
					}
					else if (this.angleValue > 180 && this.angleValue < 270)
					{
						this.angleValue = 270;
					}
				}
				if (this.forceRoundingToDegees > 1)
				{
					this.angleValue = this.forceRoundingToDegees * (int)Math.Round((double)this.angleValue / (double)this.forceRoundingToDegees);
				}
				if (this.angleValue != this.prevAngleValue)
				{
					//base.Invalidate(this.lastUpdatedRegion);
					//if (this.showOnly180Degrees)
					//{
					//	base.Invalidate(new Rectangle(0, 22, 22, 126));
					//}
				}
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
            /*
			Brush brush = new SolidBrush(Color.White);
			Brush brush2 = new SolidBrush(Color.Black);
			Font font = new Font(ChartPicture.GetDefaultFontFamilyName(), 8f);
			e.Graphics.FillRectangle(brush, base.ClientRectangle);
			e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			e.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
			int num = this.showOnly180Degrees ? 22 : 85;
			PointF point = new PointF((float)num, 85f);
			if (this.showOnly180Degrees)
			{
				e.Graphics.DrawLine(Pens.Black, num, 22, num, 148);
			}
			e.Graphics.DrawArc(Pens.Black, num - 63, 22, 126, 126, -90, this.showOnly180Degrees ? 180 : 360);
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(num - 63 + 1, 23, 124, 124, -90f, (float)(this.showOnly180Degrees ? 180 : 360));
			this.lastUpdatedRegion = new Region(graphicsPath);
			SizeF sizeF = e.Graphics.MeasureString("X", font);
			StringFormat stringFormat = new StringFormat();
			if (this.showOnly180Degrees)
			{
				stringFormat.Alignment = StringAlignment.Center;
				e.Graphics.DrawString("-90", font, brush2, 11f, 3f);
				e.Graphics.DrawString("90", font, brush2, 11f, 151f);
				stringFormat.Alignment = StringAlignment.Near;
				e.Graphics.DrawString("0", font, brush2, new RectangleF(88f, (float)(85.0 - sizeF.Height / 2.0), 170f, (float)(85.0 + sizeF.Height / 2.0)), stringFormat);
			}
			else
			{
				stringFormat.Alignment = StringAlignment.Center;
				e.Graphics.DrawString("-90", font, brush2, new RectangleF(0f, 3f, 170f, 22f), stringFormat);
				e.Graphics.DrawString("90", font, brush2, new RectangleF(0f, 151f, 170f, 170f), stringFormat);
				stringFormat.Alignment = StringAlignment.Near;
				e.Graphics.DrawString("0", font, brush2, new RectangleF(151f, (float)(85.0 - sizeF.Height / 2.0), 170f, (float)(85.0 + sizeF.Height / 2.0)), stringFormat);
				stringFormat.Alignment = StringAlignment.Near;
				e.Graphics.DrawString("180", font, brush2, new RectangleF(0f, (float)(85.0 - sizeF.Height / 2.0), 170f, (float)(85.0 + sizeF.Height / 2.0)), stringFormat);
			}
			Matrix matrix = new Matrix();
			if (this.showMarks)
			{
				if (this.showOnly180Degrees)
				{
					for (int i = 0; i <= 90; i += this.markStep)
					{
						matrix.Dispose();
						matrix = new Matrix();
						matrix.RotateAt((float)i, point);
						e.Graphics.Transform = matrix;
						e.Graphics.DrawLine(Pens.Black, num + 63, 85, num + 63 + 3, 85);
					}
					for (int j = 270; j < 360; j += this.markStep)
					{
						matrix.Dispose();
						matrix = new Matrix();
						matrix.RotateAt((float)j, point);
						e.Graphics.Transform = matrix;
						e.Graphics.DrawLine(Pens.Black, num + 63, 85, num + 63 + 3, 85);
					}
				}
				else
				{
					for (int k = 0; k < 360; k += this.markStep)
					{
						matrix.Dispose();
						matrix = new Matrix();
						matrix.RotateAt((float)k, point);
						e.Graphics.Transform = matrix;
						e.Graphics.DrawLine(Pens.Black, num + 63, 85, num + 63 + 3, 85);
					}
				}
			}
			matrix.Dispose();
			matrix = new Matrix();
			matrix.RotateAt((float)this.angleValue, point);
			e.Graphics.Transform = matrix;
			this.prevAngleValue = this.angleValue;
			if (this.showLine)
			{
				e.Graphics.DrawLine(Pens.Black, num, 85, num + 63, 85);
			}
			if (this.showOnly180Degrees && this.angleValue >= 270)
			{
				this.angleValue -= 360;
			}
			if (this.showText)
			{
				e.Graphics.DrawString(this.angleValue.ToString(CultureInfo.InvariantCulture) + " degrees", font, brush2, point);
			}
			e.Graphics.Flush();
			brush.Dispose();
			brush2.Dispose();
			font.Dispose();
            */
		}

		protected virtual void OnValueChanged(EventArgs e)
		{
			if (this.onValueChanged != null)
			{
				this.onValueChanged(this, e);
			}
		}
	}
}
