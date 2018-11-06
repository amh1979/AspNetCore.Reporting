using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class Viewport : Panel
	{
		private bool autoSize = true;

		private int contentSize;

		private int contentAutoFitMargin = 10;

		private bool enablePanning;

		private int minimumZoom = 20;

		private int maximumZoom = 20000;

		private float zoom = 100f;

		private ViewCenter viewCenter;

		private bool optimizeForPanning;

		private bool loadTilesAsynchronously;

		private bool queryVirtualEarthAsynchronously;

		private string errorMessage = string.Empty;

		public override int BorderWidth
		{
			get
			{
				return base.BorderWidth;
			}
			set
			{
				if (base.BorderWidth != value)
				{
					base.BorderWidth = value;
					this.InvalidateAndLayout();
				}
			}
		}

		[Browsable(false)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[SerializationVisibility(SerializationVisibility.Hidden)]
		public override int ZOrder
		{
			get
			{
				return -2147483648;
			}
			set
			{
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeViewport_AutoSize")]
		[NotifyParentProperty(true)]
		public bool AutoSize
		{
			get
			{
				return this.autoSize;
			}
			set
			{
				if (this.autoSize != value)
				{
					this.autoSize = value;
					this.Location.Docked = this.AutoSize;
					this.Size.AutoSize = this.AutoSize;
					this.InvalidateAndLayout();
				}
			}
		}

		[SRDescription("DescriptionAttributeViewport_ContentSize")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(0)]
		[SRCategory("CategoryAttribute_MapContent")]
		[TypeConverter(typeof(IntAutoFitConverter))]
		[NotifyParentProperty(true)]
		public int ContentSize
		{
			get
			{
				return this.contentSize;
			}
			set
			{
				this.contentSize = value;
				MapCore mapCore = base.GetMapCore();
				if (mapCore != null)
				{
					mapCore.InvalidateCachedPaths();
				}
				this.InvalidateDistanceScalePanel();
				this.InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_MapContent")]
		[DefaultValue(10)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeViewport_ContentAutoFitMargin")]
		public int ContentAutoFitMargin
		{
			get
			{
				return this.contentAutoFitMargin;
			}
			set
			{
				this.contentAutoFitMargin = value;
				MapCore mapCore = base.GetMapCore();
				if (mapCore != null)
				{
					mapCore.InvalidateCachedPaths();
				}
				this.InvalidateDistanceScalePanel();
				this.InvalidateViewport();
			}
		}

		[DefaultValue(0)]
		public override int BackShadowOffset
		{
			get
			{
				return base.BackShadowOffset;
			}
			set
			{
				base.BackShadowOffset = value;
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(false)]
		[Browsable(false)]
		[SRCategory("CategoryAttribute_Interactivity")]
		[SRDescription("DescriptionAttributeViewport_EnablePanning")]
		[NotifyParentProperty(true)]
		public bool EnablePanning
		{
			get
			{
				return this.enablePanning;
			}
			set
			{
				this.enablePanning = value;
				this.Invalidate();
			}
		}

		[DefaultValue(20)]
		[SRCategory("CategoryAttribute_Zooming")]
		[SRDescription("DescriptionAttributeViewport_MinimumZoom")]
		[NotifyParentProperty(true)]
		public int MinimumZoom
		{
			get
			{
				return this.minimumZoom;
			}
			set
			{
				if (this.minimumZoom != value)
				{
					MapCore mapCore = base.GetMapCore();
					this.minimumZoom = value;
					if (mapCore != null && mapCore.ZoomPanel != null)
					{
						mapCore.ZoomPanel.UpdateZoomRange();
					}
					this.Invalidate();
				}
			}
		}

		[SRCategory("CategoryAttribute_Zooming")]
		[NotifyParentProperty(true)]
		[DefaultValue(20000)]
		[SRDescription("DescriptionAttributeViewport_MaximumZoom")]
		public int MaximumZoom
		{
			get
			{
				return this.maximumZoom;
			}
			set
			{
				if (this.maximumZoom != value)
				{
					if (value > 5000000)
					{
						throw new ArgumentException(SR.ExceptionMaximumZoomtooLarge(5000000), "MaximumZoom");
					}
					MapCore mapCore = base.GetMapCore();
					this.maximumZoom = value;
					if (mapCore != null && mapCore.ZoomPanel != null)
					{
						mapCore.ZoomPanel.UpdateZoomRange();
					}
					this.Invalidate();
				}
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(100f)]
		[Browsable(false)]
		[SRCategory("CategoryAttribute_Zooming")]
		[SRDescription("DescriptionAttributeViewport_Zoom")]
		public float Zoom
		{
			get
			{
				return this.zoom;
			}
			set
			{
				this.zoom = value;
				if (this.zoom > (float)this.MaximumZoom)
				{
					this.zoom = (float)this.MaximumZoom;
				}
				else if (this.zoom < (float)this.MinimumZoom)
				{
					this.zoom = (float)this.MinimumZoom;
				}
				MapCore mapCore = base.GetMapCore();
				if (mapCore != null && mapCore.ZoomPanel != null)
				{
					mapCore.ZoomPanel.ZoomLevel = (double)value;
				}
				MapCore mapCore2 = base.GetMapCore();
				if (mapCore2 != null)
				{
					mapCore2.EnsureContentIsVisible();
					mapCore2.InvalidateCachedPaths();
				}
				this.InvalidateDistanceScalePanel();
				this.InvalidateViewport();
			}
		}

		[SRCategory("CategoryAttribute_View")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeViewport_ViewCenter")]
		[TypeConverter(typeof(ViewCenterConverter))]
		public ViewCenter ViewCenter
		{
			get
			{
				return this.viewCenter;
			}
			set
			{
				this.viewCenter = value;
				this.viewCenter.Parent = this;
				this.InvalidateDistanceScalePanel();
				this.InvalidateViewport(false);
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(false)]
		[SRCategory("CategoryAttribute_Interactivity")]
		[SRDescription("DescriptionAttributeViewport_OptimizeForPanning")]
		[Browsable(false)]
		public bool OptimizeForPanning
		{
			get
			{
				return this.optimizeForPanning;
			}
			set
			{
				this.optimizeForPanning = value;
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeViewport_LoadTilesAsynchronously")]
		[SRCategory("CategoryAttribute_Interactivity")]
		[DefaultValue(false)]
		public bool LoadTilesAsynchronously
		{
			get
			{
				return this.loadTilesAsynchronously;
			}
			set
			{
				this.loadTilesAsynchronously = value;
			}
		}

		[SRCategory("CategoryAttribute_Interactivity")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeViewport_QueryVirtualEarthAsynchronously")]
		[DefaultValue(false)]
		public bool QueryVirtualEarthAsynchronously
		{
			get
			{
				return this.queryVirtualEarthAsynchronously;
			}
			set
			{
				this.queryVirtualEarthAsynchronously = value;
			}
		}

		[DefaultValue("")]
		[SRCategory("CategoryAttribute_MapContent")]
		[SRDescription("DescriptionAttributeViewport_ErrorMessage")]
		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
			set
			{
				this.errorMessage = value;
				this.InvalidateViewport(false);
			}
		}

		public Viewport()
			: this(null)
		{
		}

		internal Viewport(CommonElements common)
			: base(common)
		{
			this.Name = "Viewport";
			this.ViewCenter = new ViewCenter(this, 50f, 50f);
			this.BackShadowOffset = 0;
			this.Visible = true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected bool ShouldSerializeViewCenter()
		{
			if (this.ViewCenter.X == 50.0)
			{
				return this.ViewCenter.Y != 50.0;
			}
			return true;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		protected void ResetViewCenter()
		{
			this.ViewCenter.X = 50f;
			this.ViewCenter.Y = 50f;
		}

		public PointF GetViewOrigin()
		{
			return new PointF((float)(this.ViewCenter.X - 50.0), (float)(this.ViewCenter.Y - 50.0));
		}

		internal SizeF GetContentSizeInPixels()
		{
			SizeF result = default(SizeF);
			float num = (float)base.GetMapCore().CalculateAspectRatio();
			if (this.ContentSize == 0)
			{
				SizeF absoluteSize = base.GetAbsoluteSize();
				absoluteSize.Width -= (float)(this.BorderWidth * 2 + this.ContentAutoFitMargin * 2);
				absoluteSize.Height -= (float)(this.BorderWidth * 2 + this.ContentAutoFitMargin * 2);
				result.Width = absoluteSize.Width;
				result.Height = result.Width / num;
				if (absoluteSize.Height < result.Height)
				{
					result.Height = absoluteSize.Height;
					result.Width = result.Height * num;
				}
				result.Width = Math.Max(result.Width, 0f);
				result.Height = Math.Max(result.Height, 0f);
			}
			else
			{
				result.Width = (float)this.ContentSize;
				result.Height = result.Width / num;
			}
			return result;
		}

		internal PointF GetContentOffsetInPixels()
		{
			SizeF contentSizeInPixels = this.GetContentSizeInPixels();
			PointF pointF = new PointF(this.ViewCenter.X, this.ViewCenter.Y);
			pointF.X *= (float)(contentSizeInPixels.Width / 100.0);
			pointF.Y *= (float)(contentSizeInPixels.Height / 100.0);
			pointF.X *= (float)(this.Zoom / 100.0);
			pointF.Y *= (float)(this.Zoom / 100.0);
			PointF locationInPixels = base.GetLocationInPixels();
			SizeF absoluteSize = base.GetAbsoluteSize();
			PointF pointF2 = new PointF((float)(locationInPixels.X + absoluteSize.Width / 2.0), (float)(locationInPixels.Y + absoluteSize.Height / 2.0));
			return new PointF((float)(int)(pointF2.X - pointF.X), (float)(int)(pointF2.Y - pointF.Y));
		}

		internal void SetContentOffsetInPixels(PointF contentOffset)
		{
			PointF locationInPixels = base.GetLocationInPixels();
			SizeF absoluteSize = base.GetAbsoluteSize();
			PointF pointF = new PointF((float)(locationInPixels.X + absoluteSize.Width / 2.0), (float)(locationInPixels.Y + absoluteSize.Height / 2.0));
			PointF pointF2 = new PointF(pointF.X - contentOffset.X, pointF.Y - contentOffset.Y);
			SizeF contentSizeInPixels = this.GetContentSizeInPixels();
			pointF2.X /= (float)(this.Zoom / 100.0);
			pointF2.Y /= (float)(this.Zoom / 100.0);
			pointF2.X /= (float)(contentSizeInPixels.Width / 100.0);
			pointF2.Y /= (float)(contentSizeInPixels.Height / 100.0);
			this.ViewCenter.X = pointF2.X;
			this.ViewCenter.Y = pointF2.Y;
		}

		public double GetGroundResolutionAtEquator()
		{
			double num = (base.GetMapCore().MaximumPoint.X - base.GetMapCore().MinimumPoint.X) / 360.0 * 2.0 * 3.1415926535897931 * 6378137.0;
			float num2 = (float)(this.GetContentSizeInPixels().Width * this.Zoom / 100.0);
			return num / (double)num2;
		}

		internal double GetGeographicResolutionAtEquator()
		{
			double num = base.GetMapCore().MaximumPoint.X - base.GetMapCore().MinimumPoint.X;
			float num2 = (float)(this.GetContentSizeInPixels().Width * this.Zoom / 100.0);
			Projection projection = this.Common.MapCore.Projection;
			if (projection == Projection.Orthographic)
			{
				return num / (double)num2 / 2.0;
			}
			return num / (double)num2;
		}

		internal override void RenderBorder(MapGraphics g)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			g.AntiAliasing = AntiAliasing.None;
			RectangleF rectangleF = new RectangleF(base.GetAbsoluteLocation(), base.GetAbsoluteSize());
			rectangleF.X = (float)Math.Round((double)rectangleF.X);
			rectangleF.Y = (float)Math.Round((double)rectangleF.Y);
			rectangleF.Width = (float)Math.Round((double)rectangleF.Width);
			rectangleF.Height = (float)Math.Round((double)rectangleF.Height);
			if (rectangleF.Width > 0.0 && rectangleF.Height > 0.0)
			{
				try
				{
					if (this.BorderWidth > 0 && !this.BorderColor.IsEmpty && this.BorderStyle != 0)
					{
						using (Pen pen = new Pen(this.BorderColor, (float)this.BorderWidth))
						{
							pen.DashStyle = MapGraphics.GetPenStyle(this.BorderStyle);
							pen.Alignment = PenAlignment.Inset;
							if (this.BorderWidth == 1)
							{
								rectangleF.Width -= 1f;
								rectangleF.Height -= 1f;
							}
							g.DrawRectangle(pen, rectangleF.X, rectangleF.Y, rectangleF.Width, rectangleF.Height);
						}
					}
				}
				finally
				{
					g.AntiAliasing = antiAliasing;
				}
			}
		}

		internal override void Render(MapGraphics g)
		{
			AntiAliasing antiAliasing = g.AntiAliasing;
			g.AntiAliasing = AntiAliasing.None;
			RectangleF rectangleF = new RectangleF(base.GetAbsoluteLocation(), base.GetAbsoluteSize());
			rectangleF.X = (float)Math.Round((double)rectangleF.X);
			rectangleF.Y = (float)Math.Round((double)rectangleF.Y);
			rectangleF.Width = (float)Math.Round((double)rectangleF.Width);
			rectangleF.Height = (float)Math.Round((double)rectangleF.Height);
			if (rectangleF.Width > 0.0 && rectangleF.Height > 0.0)
			{
				try
				{
					if (this.BackShadowOffset != 0)
					{
						RectangleF rect = rectangleF;
						rect.Offset((float)this.BackShadowOffset, (float)this.BackShadowOffset);
						g.FillRectangle(g.GetShadowBrush(), rect);
					}
					using (Brush brush = g.CreateBrush(rectangleF, this.BackColor, this.BackHatchStyle, "", MapImageWrapMode.Unscaled, Color.Empty, MapImageAlign.Center, this.BackGradientType, this.BackSecondaryColor))
					{
						g.FillRectangle(brush, rectangleF);
					}
				}
				finally
				{
					g.AntiAliasing = antiAliasing;
				}
			}
		}

		internal override void SizeLocationChanged(SizeLocationChangeInfo info)
		{
			base.SizeLocationChanged(info);
			switch (info)
			{
			case SizeLocationChangeInfo.Location:
				this.Location.Docked = this.AutoSize;
				break;
			case SizeLocationChangeInfo.Size:
				this.Size.AutoSize = this.AutoSize;
				break;
			}
			MapCore mapCore = base.GetMapCore();
			if (mapCore != null)
			{
				mapCore.InvalidateCachedPaths();
			}
			this.InvalidateAndLayout();
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Margins":
				return new PanelMargins(0, 0, 0, 0);
			case "Location":
				return new MapLocation(null, 0f, 0f);
			case "Size":
				return new MapSize(null, 100f, 100f);
			case "SizeUnit":
				return CoordinateUnit.Percent;
			case "BackColor":
				return Color.White;
			case "BorderColor":
				return Color.DarkGray;
			case "BorderWidth":
				return 0;
			case "BackGradientType":
				return GradientType.None;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}
	}
}
