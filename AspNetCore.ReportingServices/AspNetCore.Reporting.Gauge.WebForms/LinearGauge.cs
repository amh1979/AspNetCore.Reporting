using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace AspNetCore.Reporting.Gauge.WebForms
{
	[TypeConverter(typeof(LinearGaugeConverter))]
	internal class LinearGauge : GaugeBase, ISelectable
	{
		private LinearScaleCollection scales;

		private LinearRangeCollection ranges;

		private LinearPointerCollection pointers;

		private GaugeOrientation orientation = GaugeOrientation.Auto;

		private SizeF absoluteSize = SizeF.Empty;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearScaleCollection Scales
		{
			get
			{
				return this.scales;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearRangeCollection Ranges
		{
			get
			{
				return this.ranges;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public LinearPointerCollection Pointers
		{
			get
			{
				return this.pointers;
			}
		}

		[SRCategory("CategoryLayout")]
		[SRDescription("DescriptionAttributeLinearGauge_Orientation")]
		[DefaultValue(GaugeOrientation.Auto)]
		public GaugeOrientation Orientation
		{
			get
			{
				return this.orientation;
			}
			set
			{
				this.orientation = value;
				this.Invalidate();
			}
		}

		internal SizeF AbsoluteSize
		{
			get
			{
				return this.absoluteSize;
			}
			set
			{
				this.absoluteSize = value;
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
			}
		}

		public LinearGauge()
		{
			base.Frame = new BackFrame(this, BackFrameStyle.Edged, BackFrameShape.Rectangular);
			base.Frame.Parent = this;
			this.scales = new LinearScaleCollection(this, base.common);
			this.ranges = new LinearRangeCollection(this, base.common);
			this.pointers = new LinearPointerCollection(this, base.common);
		}

		public override string ToString()
		{
			return this.Name;
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			this.Scales.BeginInit();
			this.Ranges.BeginInit();
			this.Pointers.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			this.Scales.EndInit();
			this.Ranges.EndInit();
			this.Pointers.EndInit();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			this.Scales.Dispose();
			this.Ranges.Dispose();
			this.Pointers.Dispose();
		}

		internal override void ReconnectData(bool exact)
		{
			base.ReconnectData(exact);
			this.Pointers.ReconnectData(exact);
		}

		internal override void Notify(MessageType msg, NamedElement element, object param)
		{
			base.Notify(msg, element, param);
			this.Scales.Notify(msg, element, param);
			this.Ranges.Notify(msg, element, param);
			this.Pointers.Notify(msg, element, param);
		}

		internal override IEnumerable GetRanges()
		{
			return this.ranges;
		}

		internal GaugeOrientation GetOrientation()
		{
			if (this.Orientation == GaugeOrientation.Auto)
			{
				if (this.AbsoluteSize.Width < this.AbsoluteSize.Height)
				{
					return GaugeOrientation.Vertical;
				}
				return GaugeOrientation.Horizontal;
			}
			return this.Orientation;
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
			if (rectangle.Width > rectangle.Height * base.AspectRatio)
			{
				float width = rectangle.Width;
				rectangle.Width = rectangle.Height * base.AspectRatio;
				rectangle.X += (float)((width - rectangle.Width) / 2.0);
			}
			else
			{
				float height = rectangle.Height;
				rectangle.Height = rectangle.Width / base.AspectRatio;
				rectangle.Y += (float)((height - rectangle.Height) / 2.0);
			}
			rectangle.X /= num;
			rectangle.Y /= num2;
			rectangle.Width /= num;
			rectangle.Height /= num2;
			return rectangle;
		}

		internal override RectangleF GetBoundRect(GaugeGraphics g)
		{
			if (this.Common != null && !float.IsNaN(base.AspectRatio))
			{
				RectangleF absoluteRectangle = g.GetAbsoluteRectangle(base.Position.Rectangle);
				if (absoluteRectangle.Width > absoluteRectangle.Height * base.AspectRatio)
				{
					float width = absoluteRectangle.Width;
					absoluteRectangle.Width = absoluteRectangle.Height * base.AspectRatio;
					absoluteRectangle.X += (float)((width - absoluteRectangle.Width) / 2.0);
					return g.GetRelativeRectangle(absoluteRectangle);
				}
				float height = absoluteRectangle.Height;
				absoluteRectangle.Height = absoluteRectangle.Width / base.AspectRatio;
				absoluteRectangle.Y += (float)((height - absoluteRectangle.Height) / 2.0);
				return g.GetRelativeRectangle(absoluteRectangle);
			}
			return base.Position.Rectangle;
		}

		internal override void RenderStaticElements(GaugeGraphics g)
		{
			if (base.Visible)
			{
				this.AbsoluteSize = g.GetAbsoluteSize(base.Size);
				g.StartHotRegion(this);
				base.BackFrame.RenderFrame(g);
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
				this.Common.GaugeCore.HotRegionList.SetHotRegion(this, PointF.Empty, base.BackFrame.GetFramePath(g, 0f));
				g.EndHotRegion();
				this.RenderStaticShadows(g);
				foreach (LinearRange range in this.Ranges)
				{
					range.Render(g);
				}
				foreach (LinearScale scale in this.Scales)
				{
					scale.RenderStaticElements(g);
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
				if (this.Common != null && this.Common.GaugeCore.renderContent == RenderContent.Dynamic)
				{
					this.AbsoluteSize = g.GetAbsoluteSize(base.Size);
				}
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
				foreach (LinearPointer pointer in this.Pointers)
				{
					pointer.Render(g);
				}
				foreach (LinearScale scale in this.Scales)
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
				foreach (LinearPointer pointer in this.Pointers)
				{
					GraphicsPath shadowPath = pointer.GetShadowPath(g);
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

		internal void RenderStaticShadows(GaugeGraphics g)
		{
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				foreach (LinearRange range in this.Ranges)
				{
					GraphicsPath path = range.GetPath(g, true);
					if (path != null)
					{
						graphicsPath.AddPath(path, false);
					}
				}
				foreach (LinearScale scale in this.Scales)
				{
					GraphicsPath shadowPath = scale.GetShadowPath();
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
			g.DrawSelection(g.GetAbsoluteRectangle(new RectangleF(0f, 0f, 100f, 100f)), (float)(-3.0 / g.Graphics.PageScale), designTimeSelection, this.Common.GaugeCore.SelectionBorderColor, this.Common.GaugeCore.SelectionMarkerColor);
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
			LinearGauge linearGauge = new LinearGauge();
			binaryFormatSerializer.Deserialize(linearGauge, stream);
			return linearGauge;
		}
	}
}
