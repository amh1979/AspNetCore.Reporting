using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CircularRangeConverter))]
	internal sealed class CircularRange : RangeBase, ISelectable
	{
		[Browsable(false)]
		[SRDescription("DescriptionAttributeStartValue")]
		[DefaultValue(70.0)]
		[SRCategory("CategoryLayout")]
		public override double StartValue
		{
			get
			{
				return base.StartValue;
			}
			set
			{
				base.StartValue = value;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeEndValue3")]
		[DefaultValue(100.0)]
		public override double EndValue
		{
			get
			{
				return base.EndValue;
			}
			set
			{
				base.EndValue = value;
				this.Invalidate();
			}
		}

		[ValidateBound(0.0, 100.0)]
		[SRDescription("DescriptionAttributeCircularRange_StartWidth")]
		[SRCategory("CategoryLayout")]
		[DefaultValue(15f)]
		public override float StartWidth
		{
			get
			{
				return base.StartWidth;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					base.StartWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeCircularRange_EndWidth")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(30f)]
		public override float EndWidth
		{
			get
			{
				return base.EndWidth;
			}
			set
			{
				if (!(value < 0.0) && !(value > 1000.0))
				{
					base.EndWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 1000));
			}
		}

		[SRDescription("DescriptionAttributeDistanceFromScale8")]
		[SRCategory("CategoryLayout")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(30f)]
		public override float DistanceFromScale
		{
			get
			{
				return base.DistanceFromScale;
			}
			set
			{
				if (!(value < -100.0) && !(value > 100.0))
				{
					base.DistanceFromScale = value;
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", -100, 100));
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributePlacement7")]
		[DefaultValue(Placement.Inside)]
		public override Placement Placement
		{
			get
			{
				return base.Placement;
			}
			set
			{
				base.Placement = value;
				this.Invalidate();
			}
		}

		public CircularRange()
		{
			base.StartValue = 70.0;
			base.EndValue = 100.0;
			base.StartWidth = 15f;
			base.EndWidth = 30f;
			base.DistanceFromScale = 30f;
			base.Placement = Placement.Inside;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public CircularGauge GetGauge()
		{
			if (this.Collection == null)
			{
				return null;
			}
			return (CircularGauge)this.Collection.parent;
		}

		public CircularScale GetScale()
		{
			if (this.GetGauge() == null)
			{
				return null;
			}
			if (base.ScaleName == string.Empty)
			{
				return null;
			}
			if (base.ScaleName == "Default" && this.GetGauge().Scales.Count == 0)
			{
				return null;
			}
			return this.GetGauge().Scales[base.ScaleName];
		}

		internal override void Render(GaugeGraphics g)
		{
			if (this.Common != null && base.Visible && this.GetScale() != null)
			{
				this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRendering(this.Name));
				g.StartHotRegion(this);
				this.GetScale().SetDrawRegion(g);
				RectangleF rectangleF = this.CalculateRangeRectangle();
				CircularScale scale = this.GetScale();
				double valueLimit = scale.GetValueLimit(this.StartValue);
				double valueLimit2 = scale.GetValueLimit(this.EndValue);
				float positionFromValue = scale.GetPositionFromValue(valueLimit);
				float positionFromValue2 = scale.GetPositionFromValue(valueLimit2);
				float num = positionFromValue2 - positionFromValue;
				if (Math.Round((double)num, 4) == 0.0)
				{
					g.RestoreDrawRegion();
					g.EndHotRegion();
					this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(this.Name));
				}
				else
				{
					Pen pen = null;
					Brush brush = null;
					GraphicsPath graphicsPath = null;
					try
					{
						graphicsPath = g.GetCircularRangePath(rectangleF, (float)(positionFromValue + 90.0), num, this.StartWidth, this.EndWidth, this.Placement);
						if (graphicsPath == null)
						{
							g.RestoreDrawRegion();
							g.EndHotRegion();
							this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(this.Name));
							return;
						}
						RectangleF rect = rectangleF;
						if (this.Placement != 0)
						{
							float num2 = this.StartWidth;
							if (num2 < this.EndWidth)
							{
								num2 = this.EndWidth;
							}
							if (this.Placement == Placement.Outside)
							{
								rect.Inflate(num2, num2);
							}
							else
							{
								rect.Inflate((float)(num2 / 2.0), (float)(num2 / 2.0));
							}
						}
						RangeGradientType fillGradientType = base.FillGradientType;
						brush = g.GetCircularRangeBrush(rect, (float)(positionFromValue + 90.0), num, base.FillColor, base.FillHatchStyle, "", GaugeImageWrapMode.Unscaled, Color.Empty, GaugeImageAlign.TopLeft, fillGradientType, base.FillGradientEndColor);
						pen = new Pen(base.BorderColor, (float)base.BorderWidth);
						pen.DashStyle = g.GetPenStyle(base.BorderStyle);
						g.FillPath(brush, graphicsPath);
						if (base.BorderStyle != 0 && base.BorderWidth > 0)
						{
							g.DrawPath(pen, graphicsPath);
						}
					}
					catch (Exception)
					{
						if (graphicsPath != null)
						{
							graphicsPath.Dispose();
						}
						if (pen != null)
						{
							pen.Dispose();
						}
						if (brush != null)
						{
							brush.Dispose();
						}
						g.RestoreDrawRegion();
						throw;
					}
					this.Common.GaugeCore.HotRegionList.SetHotRegion(this, g.GetAbsolutePoint(this.GetScale().GetPivotPoint()), graphicsPath);
					g.RestoreDrawRegion();
					g.EndHotRegion();
					this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(this.Name));
				}
			}
		}

		internal RectangleF CalculateRangeRectangle()
		{
			CircularScale scale = this.GetScale();
			PointF pivotPoint = this.GetScale().GetPivotPoint();
			float radius = this.GetScale().GetRadius();
			RectangleF result = new RectangleF(pivotPoint.X - radius, pivotPoint.Y - radius, (float)(radius * 2.0), (float)(radius * 2.0));
			if (this.Placement == Placement.Outside)
			{
				result.Inflate(this.DistanceFromScale, this.DistanceFromScale);
				result.Inflate((float)(scale.Width / 2.0), (float)(scale.Width / 2.0));
			}
			else if (this.Placement == Placement.Inside)
			{
				result.Inflate((float)(0.0 - this.DistanceFromScale), (float)(0.0 - this.DistanceFromScale));
				result.Inflate((float)((0.0 - scale.Width) / 2.0), (float)((0.0 - scale.Width) / 2.0));
			}
			else
			{
				result.Inflate((float)(0.0 - this.DistanceFromScale), (float)(0.0 - this.DistanceFromScale));
			}
			return result;
		}

		internal GraphicsPath GetPath(GaugeGraphics g, bool getShadowPath)
		{
			if (getShadowPath && (!base.Visible || base.ShadowOffset == 0.0))
			{
				return null;
			}
			this.GetScale().SetDrawRegion(g);
			CircularScale scale = this.GetScale();
			RectangleF rect = this.CalculateRangeRectangle();
			double valueLimit = scale.GetValueLimit(this.StartValue);
			double valueLimit2 = scale.GetValueLimit(this.EndValue);
			float positionFromValue = scale.GetPositionFromValue(valueLimit);
			float positionFromValue2 = scale.GetPositionFromValue(valueLimit2);
			float num = positionFromValue2 - positionFromValue;
			if (Math.Round((double)num, 4) == 0.0)
			{
				g.RestoreDrawRegion();
				return null;
			}
			GraphicsPath circularRangePath = g.GetCircularRangePath(rect, (float)(positionFromValue + 90.0), positionFromValue2 - positionFromValue, this.StartWidth, this.EndWidth, this.Placement);
			if (circularRangePath != null && getShadowPath)
			{
				using (Matrix matrix = new Matrix())
				{
					matrix.Translate(base.ShadowOffset, base.ShadowOffset);
					circularRangePath.Transform(matrix);
				}
			}
			PointF pointF = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
			g.RestoreDrawRegion();
			PointF pointF2 = new PointF(g.Graphics.Transform.OffsetX, g.Graphics.Transform.OffsetY);
			if (circularRangePath != null)
			{
				using (Matrix matrix2 = new Matrix())
				{
					matrix2.Translate(pointF.X - pointF2.X, pointF.Y - pointF2.Y);
					circularRangePath.Transform(matrix2);
					return circularRangePath;
				}
			}
			return circularRangePath;
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			if (this.GetGauge() != null && this.GetScale() != null)
			{
				Stack stack = new Stack();
				for (NamedElement namedElement = this.GetGauge().ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
				{
					stack.Push(namedElement);
				}
				foreach (IRenderable item in stack)
				{
					g.CreateDrawRegion(item.GetBoundRect(g));
				}
				g.CreateDrawRegion(((IRenderable)this.GetGauge()).GetBoundRect(g));
				using (GraphicsPath graphicsPath = this.GetPath(g, false))
				{
					if (graphicsPath != null)
					{
						RectangleF bounds = graphicsPath.GetBounds();
						g.DrawSelection(bounds, designTimeSelection, this.Common.GaugeCore.SelectionBorderColor, this.Common.GaugeCore.SelectionMarkerColor);
					}
				}
				g.RestoreDrawRegion();
				foreach (IRenderable item2 in stack)
				{
					IRenderable renderable = item2;
					g.RestoreDrawRegion();
				}
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			CircularRange circularRange = new CircularRange();
			binaryFormatSerializer.Deserialize(circularRange, stream);
			return circularRange;
		}
	}
}
