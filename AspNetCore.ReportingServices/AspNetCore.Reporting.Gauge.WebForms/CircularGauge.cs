using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(CircularGaugeConverter))]
	internal sealed class CircularGauge : GaugeBase, ISelectable
	{
		internal RectangleF absoluteRect = RectangleF.Empty;

		private CircularScaleCollection scales;

		private CircularRangeCollection ranges;

		private CircularPointerCollection pointers;

		private KnobCollection knobs;

		private GaugeLocation pivotPoint;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CircularScaleCollection Scales
		{
			get
			{
				return this.scales;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CircularRangeCollection Ranges
		{
			get
			{
				return this.ranges;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CircularPointerCollection Pointers
		{
			get
			{
				return this.pointers;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public KnobCollection Knobs
		{
			get
			{
				return this.knobs;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[TypeConverter(typeof(LocationConverter))]
		[SRCategory("CategoryLayout")]
		[DefaultValue(typeof(GaugeLocation), "50F, 50F")]
		[ValidateBound(100.0, 100.0)]
		[SRDescription("DescriptionAttributeCircularGauge_PivotPoint")]
		public GaugeLocation PivotPoint
		{
			get
			{
				if (this.pivotPoint == null)
				{
					this.pivotPoint = new GaugeLocation(this, 50f, 50f);
				}
				return this.pivotPoint;
			}
			set
			{
				this.pivotPoint = value;
				this.pivotPoint.Parent = this;
				this.Invalidate();
			}
		}

		internal override CommonElements Common
		{
			get
			{
				return base.Common;
			}
			set
			{
				base.Common = value;
				this.Scales.Common = value;
				this.Ranges.Common = value;
				this.Pointers.Common = value;
				this.Knobs.Common = value;
			}
		}

		public CircularGauge()
		{
			base.BackFrame = new BackFrame(this, BackFrameStyle.Edged, BackFrameShape.Circular);
			this.scales = new CircularScaleCollection(this, this.Common);
			this.ranges = new CircularRangeCollection(this, this.Common);
			this.pointers = new CircularPointerCollection(this, this.Common);
			this.knobs = new KnobCollection(this, this.Common);
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal void RenderTopImage(GaugeGraphics g)
		{
			if (base.TopImage != "")
			{
				ImageAttributes imageAttributes = new ImageAttributes();
				if (base.TopImageTransColor != Color.Empty)
				{
					imageAttributes.SetColorKey(base.TopImageTransColor, base.TopImageTransColor, ColorAdjustType.Default);
				}
				Image image = this.Common.ImageLoader.LoadImage(base.TopImage);
				Rectangle destRect = Rectangle.Round(g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)));
				if (!base.TopImageHueColor.IsEmpty)
				{
					Color color = g.TransformHueColor(base.TopImageHueColor);
					ColorMatrix colorMatrix = new ColorMatrix();
					colorMatrix.Matrix00 = (float)((float)(int)color.R / 255.0);
					colorMatrix.Matrix11 = (float)((float)(int)color.G / 255.0);
					colorMatrix.Matrix22 = (float)((float)(int)color.B / 255.0);
					imageAttributes.SetColorMatrix(colorMatrix);
				}
				ImageSmoothingState imageSmoothingState = new ImageSmoothingState(g);
				imageSmoothingState.Set();
				g.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, imageAttributes);
				imageSmoothingState.Restore();
			}
		}

		internal override RectangleF GetAspectRatioBounds()
		{
			if (double.IsNaN((double)base.AspectRatio))
			{
				return base.Position.Rectangle;
			}
			RectangleF rectangle = base.Position.Rectangle;
			float num = (float)((float)this.Common.GaugeCore.GetWidth() / 100.0);
			float num2 = (float)((float)this.Common.GaugeCore.GetHeight() / 100.0);
			rectangle.X *= num;
			rectangle.Y *= num2;
			rectangle.Width *= num;
			rectangle.Height *= num2;
			if (!float.IsNaN(base.AspectRatio))
			{
				if (base.AspectRatio >= 1.0)
				{
					float width = rectangle.Width;
					rectangle.Width /= base.AspectRatio;
					rectangle.X += (float)((width - rectangle.Width) / 2.0);
				}
				else
				{
					float height = rectangle.Height;
					rectangle.Height *= base.AspectRatio;
					rectangle.Y += (float)((height - rectangle.Height) / 2.0);
				}
			}
			if (rectangle.Width != rectangle.Height)
			{
				if (rectangle.Width > rectangle.Height)
				{
					rectangle.Offset((float)((rectangle.Width - rectangle.Height) / 2.0), 0f);
					rectangle.Width = rectangle.Height;
				}
				else if (rectangle.Width < rectangle.Height)
				{
					rectangle.Offset(0f, (float)((rectangle.Height - rectangle.Width) / 2.0));
					rectangle.Height = rectangle.Width;
				}
			}
			rectangle.X /= num;
			rectangle.Y /= num2;
			rectangle.Width /= num;
			rectangle.Height /= num2;
			return rectangle;
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			this.Scales.BeginInit();
			this.Ranges.BeginInit();
			this.Pointers.BeginInit();
			this.Knobs.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			this.Scales.EndInit();
			this.Ranges.EndInit();
			this.Pointers.EndInit();
			this.Knobs.EndInit();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			this.Scales.Dispose();
			this.Ranges.Dispose();
			this.Pointers.Dispose();
			this.Knobs.Dispose();
		}

		internal override void ReconnectData(bool exact)
		{
			base.ReconnectData(exact);
			this.Pointers.ReconnectData(exact);
			this.Knobs.ReconnectData(exact);
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			this.Scales.Notify(msg, element, param);
			this.Ranges.Notify(msg, element, param);
			this.Pointers.Notify(msg, element, param);
			this.Knobs.Notify(msg, element, param);
		}

		internal override IEnumerable GetRanges()
		{
			return this.ranges;
		}

		internal override void PointerValueChanged(PointerBase sender)
		{
			ScaleBase scaleBase = sender.GetScaleBase();
			if (scaleBase != null)
			{
				foreach (RangeBase range in this.Ranges)
				{
					if (range.ScaleName == scaleBase.Name)
					{
						range.PointerValueChanged(sender.Data);
					}
				}
				scaleBase.PointerValueChanged(sender);
			}
		}

		internal override RectangleF GetBoundRect(GaugeGraphics g)
		{
			if (this.Common != null)
			{
				RectangleF absolute = this.absoluteRect = g.GetAbsoluteRectangle(base.Position.Rectangle);
				if (!float.IsNaN(base.AspectRatio))
				{
					if (base.AspectRatio >= 1.0)
					{
						float width = absolute.Width;
						absolute.Width /= base.AspectRatio;
						absolute.X += (float)((width - absolute.Width) / 2.0);
					}
					else
					{
						float height = absolute.Height;
						absolute.Height *= base.AspectRatio;
						absolute.Y += (float)((height - absolute.Height) / 2.0);
					}
				}
				if (absolute.Width != absolute.Height)
				{
					if (absolute.Width > absolute.Height)
					{
						absolute.Offset((float)((absolute.Width - absolute.Height) / 2.0), 0f);
						absolute.Width = absolute.Height;
					}
					else if (absolute.Width < absolute.Height)
					{
						absolute.Offset(0f, (float)((absolute.Height - absolute.Width) / 2.0));
						absolute.Height = absolute.Width;
					}
				}
				return g.GetRelativeRectangle(absolute);
			}
			return base.Position.Rectangle;
		}

		internal override void RenderStaticElements(GaugeGraphics g)
		{
			if (base.Visible)
			{
				g.StartHotRegion(this);
				base.BackFrame.RenderFrame(g);
				GraphicsState gstate = g.Save();
				if (base.ClipContent && base.BackFrame.FrameStyle != 0 && base.BackFrame.Image == string.Empty)
				{
					GraphicsPath graphicsPath = null;
					try
					{
						graphicsPath = base.BackFrame.GetBackPath(g);
						g.SetClip(graphicsPath, CombineMode.Intersect);
					}
					finally
					{
						if (graphicsPath != null)
						{
							graphicsPath.Dispose();
						}
					}
				}
				this.Common.GaugeCore.HotRegionList.SetHotRegion(this, g.GetAbsolutePoint(this.PivotPoint.ToPoint()), base.BackFrame.GetFramePath(g, 0f));
				g.EndHotRegion();
				g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f));
				this.RenderStaticShadows(g);
				foreach (CircularRange range in this.Ranges)
				{
					range.Render(g);
				}
				foreach (CircularScale scale in this.Scales)
				{
					scale.RenderStaticElements(g);
				}
				foreach (CircularPointer pointer in this.Pointers)
				{
					pointer.ResetCachedXamlRenderer();
				}
				if (base.ClipContent && base.BackFrame.FrameStyle != 0 && base.BackFrame.Image == string.Empty)
				{
					g.Restore(gstate);
				}
			}
		}

		internal override void RenderDynamicElements(GaugeGraphics g)
		{
			if (base.Visible)
			{
				GraphicsState gstate = g.Save();
				if (base.ClipContent && base.BackFrame.FrameStyle != 0 && base.BackFrame.Image == string.Empty)
				{
					GraphicsPath graphicsPath = null;
					try
					{
						graphicsPath = ((base.BackFrame.FrameStyle != 0) ? base.BackFrame.GetBackPath(g) : base.BackFrame.GetFramePath(g, 0f));
						g.SetClip(graphicsPath, CombineMode.Intersect);
					}
					finally
					{
						if (graphicsPath != null)
						{
							graphicsPath.Dispose();
						}
					}
				}
				this.RenderDynamicShadows(g);
				foreach (Knob knob in this.Knobs)
				{
					knob.Render(g);
				}
				foreach (CircularPointer pointer in this.Pointers)
				{
					pointer.Render(g);
				}
				foreach (CircularScale scale in this.Scales)
				{
					scale.RenderDynamicElements(g);
				}
				if (base.ClipContent && base.BackFrame.FrameStyle != 0 && base.BackFrame.Image == string.Empty)
				{
					g.Restore(gstate);
				}
				base.BackFrame.RenderGlassEffect(g);
				this.RenderTopImage(g);
			}
		}

		internal void RenderDynamicShadows(GaugeGraphics g)
		{
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				foreach (Knob knob in this.Knobs)
				{
					GraphicsPath shadowPath = knob.GetShadowPath(g);
					if (shadowPath != null)
					{
						graphicsPath.AddPath(shadowPath, false);
					}
				}
				foreach (CircularPointer pointer in this.Pointers)
				{
					GraphicsPath shadowPath2 = pointer.GetShadowPath(g);
					if (shadowPath2 != null)
					{
						graphicsPath.AddPath(shadowPath2, false);
					}
				}
				graphicsPath.FillMode = FillMode.Winding;
				using (Brush brush = g.GetShadowBrush())
				{
					g.FillPath(brush, graphicsPath);
				}
			}
		}

		internal void RenderStaticShadows(GaugeGraphics g)
		{
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				foreach (CircularRange range in this.Ranges)
				{
					GraphicsPath path = range.GetPath(g, true);
					if (path != null)
					{
						graphicsPath.AddPath(path, false);
					}
				}
				foreach (CircularScale scale in this.Scales)
				{
					GraphicsPath shadowPath = scale.GetShadowPath(g);
					if (shadowPath != null)
					{
						graphicsPath.AddPath(shadowPath, false);
					}
				}
				graphicsPath.FillMode = FillMode.Winding;
				using (Brush brush = g.GetShadowBrush())
				{
					g.FillPath(brush, graphicsPath);
				}
			}
		}

		void ISelectable.DrawSelection(GaugeGraphics g, bool designTimeSelection)
		{
			Stack stack = new Stack();
			for (NamedElement namedElement = base.ParentObject; namedElement != null; namedElement = (NamedElement)((IRenderable)namedElement).GetParentRenderable())
			{
				stack.Push(namedElement);
			}
			foreach (IRenderable item in stack)
			{
				g.CreateDrawRegion(item.GetBoundRect(g));
			}
			g.CreateDrawRegion(((IRenderable)this).GetBoundRect(g));
			RectangleF frameRectangle = base.Frame.GetFrameRectangle(g);
			g.DrawSelection(frameRectangle, (float)(-3.0 / g.Graphics.PageScale), designTimeSelection, this.Common.GaugeCore.SelectionBorderColor, this.Common.GaugeCore.SelectionMarkerColor);
			g.RestoreDrawRegion();
			foreach (IRenderable item2 in stack)
			{
				IRenderable renderable = item2;
				g.RestoreDrawRegion();
			}
		}

		public override object Clone()
		{
			MemoryStream stream = new MemoryStream();
			BinaryFormatSerializer binaryFormatSerializer = new BinaryFormatSerializer();
			binaryFormatSerializer.Serialize(this, stream);
			CircularGauge circularGauge = new CircularGauge();
			binaryFormatSerializer.Deserialize(circularGauge, stream);
			return circularGauge;
		}
	}
}
