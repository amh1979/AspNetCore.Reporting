using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(LinearRangeConverter))]
	internal sealed class LinearRange : RangeBase, ISelectable
	{
		[Browsable(false)]
		[SRDescription("DescriptionAttributeStartValue")]
		[DefaultValue(60.0)]
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
		[DefaultValue(100.0)]
		[SRDescription("DescriptionAttributeEndValue3")]
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

		[SRDescription("DescriptionAttributeLinearRange_StartWidth")]
		[ValidateBound(0.0, 100.0)]
		[DefaultValue(10f)]
		[SRCategory("CategoryLayout")]
		public override float StartWidth
		{
			get
			{
				return base.StartWidth;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					base.StartWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[ValidateBound(0.0, 100.0)]
		[SRDescription("DescriptionAttributeLinearRange_EndWidth")]
		[SRCategory("CategoryLayout")]
		[DefaultValue(10f)]
		public override float EndWidth
		{
			get
			{
				return base.EndWidth;
			}
			set
			{
				if (!(value < 0.0) && !(value > 100.0))
				{
					base.EndWidth = value;
					this.Invalidate();
					return;
				}
				throw new ArgumentException(Utils.SRGetStr("ExceptionMustInRange", 0, 100));
			}
		}

		[SRCategory("CategoryLayout")]
		[DefaultValue(Placement.Outside)]
		[SRDescription("DescriptionAttributePlacement7")]
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

		[SRDescription("DescriptionAttributeDistanceFromScale8")]
		[DefaultValue(10f)]
		[SRCategory("CategoryLayout")]
		[ValidateBound(0.0, 100.0)]
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

		public LinearRange()
		{
			base.StartValue = 60.0;
			base.EndValue = 100.0;
			base.StartWidth = 10f;
			base.EndWidth = 10f;
			base.DistanceFromScale = 10f;
			base.Placement = Placement.Outside;
		}

		public override string ToString()
		{
			return this.Name;
		}

		public LinearGauge GetGauge()
		{
			if (this.Collection == null)
			{
				return null;
			}
			return (LinearGauge)this.Collection.parent;
		}

		public LinearScale GetScale()
		{
			if (this.GetGauge() == null)
			{
				return null;
			}
			LinearScale linearScale = null;
			try
			{
				return this.GetGauge().Scales[base.ScaleName];
			}
			catch
			{
				return null;
			}
		}

		internal override void Render(GaugeGraphics g)
		{
			if (this.Common != null && base.Visible && this.GetScale() != null && !double.IsNaN(this.StartValue) && !double.IsNaN(this.EndValue))
			{
				this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceStartRendering(this.Name));
				g.StartHotRegion(this);
				LinearScale scale = this.GetScale();
				Pen pen = null;
				Brush brush = null;
				GraphicsPath graphicsPath = null;
				try
				{
					graphicsPath = g.GetLinearRangePath(scale.GetPositionFromValue(this.StartValue), scale.GetPositionFromValue(this.EndValue), this.StartWidth, this.EndWidth, scale.Position, this.GetGauge().GetOrientation(), this.DistanceFromScale, this.Placement, scale.Width);
					if (graphicsPath != null && g.Graphics.VisibleClipBounds.IntersectsWith(graphicsPath.GetBounds()))
					{
						brush = g.GetLinearRangeBrush(graphicsPath.GetBounds(), base.FillColor, base.FillHatchStyle, base.FillGradientType, base.FillGradientEndColor, this.GetGauge().GetOrientation(), this.GetScale().GetReversed(), this.StartValue, this.EndValue);
						pen = new Pen(base.BorderColor, (float)base.BorderWidth);
						pen.DashStyle = g.GetPenStyle(base.BorderStyle);
						g.FillPath(brush, graphicsPath);
						if (base.BorderStyle != 0 && base.BorderWidth > 0)
						{
							g.DrawPath(pen, graphicsPath);
						}
						goto end_IL_0069;
					}
					g.EndHotRegion();
					this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(this.Name));
					return;
					end_IL_0069:;
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
					throw;
				}
				this.Common.GaugeCore.HotRegionList.SetHotRegion(this, PointF.Empty, graphicsPath);
				g.EndHotRegion();
				this.Common.GaugeCore.TraceWrite("GaugePaint", SR.TraceRenderingComplete(this.Name));
			}
		}

		internal GraphicsPath GetPath(GaugeGraphics g, bool getShadowPath)
		{
			if (getShadowPath && (!base.Visible || base.ShadowOffset == 0.0))
			{
				return null;
			}
			if (!double.IsNaN(this.StartValue) && !double.IsNaN(this.EndValue))
			{
				LinearScale scale = this.GetScale();
				GraphicsPath linearRangePath = g.GetLinearRangePath(scale.GetPositionFromValue(this.StartValue), scale.GetPositionFromValue(this.EndValue), this.StartWidth, this.EndWidth, scale.Position, this.GetGauge().GetOrientation(), this.DistanceFromScale, this.Placement, scale.Width);
				if (getShadowPath)
				{
					using (Matrix matrix = new Matrix())
					{
						matrix.Translate(base.ShadowOffset, base.ShadowOffset);
						linearRangePath.Transform(matrix);
						return linearRangePath;
					}
				}
				return linearRangePath;
			}
			return null;
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
			LinearRange linearRange = new LinearRange();
			binaryFormatSerializer.Deserialize(linearRange, stream);
			return linearRange;
		}
	}
}
