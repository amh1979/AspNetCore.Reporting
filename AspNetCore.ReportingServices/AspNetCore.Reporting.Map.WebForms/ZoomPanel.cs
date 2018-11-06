using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal class ZoomPanel : DockablePanel, IToolTipProvider
	{
		private const double MaxScaleValue = 100.0;

		private PanelButton zoomInButton;

		private PanelButton zoomOutButton;

		private bool internalZoomChange;

		private bool fixThumbPoition;

		private ZoomPanelStyle panelStyle = ZoomPanelStyle.CircularButtons;

		private Orientation orientation = Orientation.Auto;

		private ZoomType zoomType = ZoomType.Exponential;

		private bool zoomButtonsVisible = true;

		private Color symbolColor = Color.LightGray;

		private Color symbolBorderColor = Color.DimGray;

		private Color buttonBorderColor = Color.DarkGray;

		private Color buttonColor = Color.White;

		private Color thumbBorderColor = Color.Gray;

		private Color thumbColor = Color.White;

		private Color sliderBarBorderColor = Color.Silver;

		private Color sliderBarColor = Color.White;

		private Color tickBorderColor = Color.DarkGray;

		private Color tickColor = Color.White;

		private int tickCount = 10;

		private bool snapToTickMarks;

		private LinearScale scale;

		private LinearPointer pointer;

		private SizeF absoluteSize = SizeF.Empty;

		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeZoomPanel_PanelStyle")]
		[DefaultValue(ZoomPanelStyle.CircularButtons)]
		public ZoomPanelStyle PanelStyle
		{
			get
			{
				return this.panelStyle;
			}
			set
			{
				if (this.panelStyle != value)
				{
					this.panelStyle = value;
					this.ApplyStyle();
					this.Invalidate();
				}
			}
		}

		[SRDescription("DescriptionAttributeZoomPanel_Orientation")]
		[NotifyParentProperty(true)]
		[DefaultValue(Orientation.Auto)]
		[SRCategory("CategoryAttribute_Layout")]
		public Orientation Orientation
		{
			get
			{
				return this.orientation;
			}
			set
			{
				if (this.GetOrientation() != value && value != Orientation.Auto)
				{
					MapSize mapSize2 = this.Size = new MapSize(this, this.Size.Height, this.Size.Width);
				}
				this.orientation = value;
				this.AdjustAutoOrientationForDocking(this.Dock);
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeZoomPanel_Reversed")]
		[DefaultValue(false)]
		public bool Reversed
		{
			get
			{
				return this.Scale.Reversed;
			}
			set
			{
				this.Scale.Reversed = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeZoomPanel_ZoomType")]
		[NotifyParentProperty(true)]
		[DefaultValue(ZoomType.Exponential)]
		[SRCategory("CategoryAttribute_Behavior")]
		public ZoomType ZoomType
		{
			get
			{
				return this.zoomType;
			}
			set
			{
				if (this.zoomType != value)
				{
					this.zoomType = value;
					if (this.Common != null && this.Common.MapCore != null)
					{
						this.Common.MapCore.Viewport.Zoom = (float)this.GetZoomLevelFromPointerPosition(this.Pointer.Value);
					}
				}
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeZoomPanel_Dock")]
		[SRCategory("CategoryAttribute_Layout")]
		[DefaultValue(PanelDockStyle.Left)]
		[NotifyParentProperty(true)]
		public override PanelDockStyle Dock
		{
			get
			{
				return base.Dock;
			}
			set
			{
				this.AdjustAutoOrientationForDocking(value);
				base.Dock = value;
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_ZoomButtonsVisible")]
		[ParenthesizePropertyName(true)]
		[DefaultValue(true)]
		[NotifyParentProperty(true)]
		public bool ZoomButtonsVisible
		{
			get
			{
				return this.zoomButtonsVisible;
			}
			set
			{
				this.zoomButtonsVisible = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "LightGray")]
		[SRDescription("DescriptionAttributeZoomPanel_SymbolColor")]
		[NotifyParentProperty(true)]
		public Color SymbolColor
		{
			get
			{
				return this.symbolColor;
			}
			set
			{
				this.symbolColor = value;
				this.ApplyColors();
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DimGray")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_SymbolBorderColor")]
		public Color SymbolBorderColor
		{
			get
			{
				return this.symbolBorderColor;
			}
			set
			{
				this.symbolBorderColor = value;
				this.ApplyColors();
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "DarkGray")]
		[SRDescription("DescriptionAttributeZoomPanel_ButtonBorderColor")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		public Color ButtonBorderColor
		{
			get
			{
				return this.buttonBorderColor;
			}
			set
			{
				this.buttonBorderColor = value;
				this.ApplyColors();
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_ButtonColor")]
		[NotifyParentProperty(true)]
		public Color ButtonColor
		{
			get
			{
				return this.buttonColor;
			}
			set
			{
				this.buttonColor = value;
				this.ApplyColors();
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeZoomPanel_ThumbBorderColor")]
		[DefaultValue(typeof(Color), "Gray")]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		public Color ThumbBorderColor
		{
			get
			{
				return this.thumbBorderColor;
			}
			set
			{
				this.thumbBorderColor = value;
				this.ApplyColors();
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "White")]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_ThumbColor")]
		public Color ThumbColor
		{
			get
			{
				return this.thumbColor;
			}
			set
			{
				this.thumbColor = value;
				this.ApplyColors();
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeZoomPanel_SliderBarBorderColor")]
		[DefaultValue(typeof(Color), "Silver")]
		public Color SliderBarBorderColor
		{
			get
			{
				return this.sliderBarBorderColor;
			}
			set
			{
				this.sliderBarBorderColor = value;
				this.ApplyColors();
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "White")]
		[SRDescription("DescriptionAttributeZoomPanel_SliderBarColor")]
		public Color SliderBarColor
		{
			get
			{
				return this.sliderBarColor;
			}
			set
			{
				this.sliderBarColor = value;
				this.ApplyColors();
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkGray")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_TickBorderColor")]
		public Color TickBorderColor
		{
			get
			{
				return this.tickBorderColor;
			}
			set
			{
				this.tickBorderColor = value;
				this.ApplyColors();
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeZoomPanel_TickColor")]
		[DefaultValue(typeof(Color), "White")]
		public Color TickColor
		{
			get
			{
				return this.tickColor;
			}
			set
			{
				this.tickColor = value;
				this.ApplyColors();
				this.Invalidate();
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeZoomPanel_TickCount")]
		[DefaultValue(10)]
		public int TickCount
		{
			get
			{
				return this.tickCount;
			}
			set
			{
				if (value < 2)
				{
					throw new ArgumentOutOfRangeException(SR.ticknumber_out_of_range);
				}
				if (this.tickCount != value)
				{
					this.tickCount = value;
					double tickMarksInterval = this.GetTickMarksInterval(value);
					this.Scale.MinorTickMark.Interval = tickMarksInterval;
					this.Pointer.SnappingInterval = tickMarksInterval;
				}
			}
		}

		[DefaultValue(false)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeZoomPanel_SnapToTickMarks")]
		public bool SnapToTickMarks
		{
			get
			{
				return this.snapToTickMarks;
			}
			set
			{
				this.snapToTickMarks = value;
				this.Pointer.SnappingEnabled = value;
			}
		}

		public override string ToolTip
		{
			get
			{
				return base.ToolTip;
			}
			set
			{
				base.ToolTip = value;
				this.Scale.ToolTip = value;
				this.Pointer.ToolTip = value;
			}
		}

		internal LinearScale Scale
		{
			get
			{
				return this.scale;
			}
		}

		internal LinearPointer Pointer
		{
			get
			{
				return this.pointer;
			}
			set
			{
				this.pointer = value;
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

		internal Position Position
		{
			get
			{
				return new Position(this.Location, this.Size, ContentAlignment.TopLeft);
			}
		}

		internal double MinimumZoom
		{
			get
			{
				if (this.Common != null && this.Common.MapCore != null)
				{
					return (double)this.Common.MapCore.Viewport.MinimumZoom;
				}
				return 50.0;
			}
		}

		internal double MaximumZoom
		{
			get
			{
				if (this.Common != null && this.Common.MapCore != null)
				{
					return (double)this.Common.MapCore.Viewport.MaximumZoom;
				}
				return 1000.0;
			}
		}

		internal double ZoomLevel
		{
			get
			{
				if (this.Common != null && this.Common.MapCore != null)
				{
					return (double)this.Common.MapCore.Viewport.Zoom;
				}
				return 100.0;
			}
			set
			{
				if (!this.internalZoomChange)
				{
					try
					{
						this.fixThumbPoition = true;
						double pointerPositionFromZoomLevel = this.GetPointerPositionFromZoomLevel(value);
						this.Pointer.Value = pointerPositionFromZoomLevel;
					}
					finally
					{
						this.fixThumbPoition = false;
					}
				}
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
				if (this.Scale != null)
				{
					this.Scale.Common = value;
				}
				if (this.Pointer != null)
				{
					this.Pointer.Common = value;
				}
			}
		}

		public ZoomPanel()
			: this(null)
		{
		}

		internal ZoomPanel(CommonElements common)
			: base(common)
		{
			this.Name = "ZoomPanel";
			this.scale = new LinearScale(this);
			this.pointer = new LinearPointer(this);
			MapCore mapCore = base.GetMapCore();
			this.Scale.LabelStyle.Visible = false;
			this.Scale.MajorTickMark.Visible = false;
			this.Scale.MinorTickMark.Length = 30f;
			this.Scale.MinorTickMark.Width = 10f;
			this.Scale.MinorTickMark.EnableGradient = false;
			this.Scale.FillGradientType = GradientType.None;
			this.Scale.FillHatchStyle = MapHatchStyle.None;
			this.Scale.ShadowOffset = 0f;
			this.Scale.BorderWidth = 1;
			this.Scale.Width = 15f;
			this.Scale.Minimum = 0.0;
			this.Scale.Maximum = 100.00000000001;
			double tickMarksInterval = this.GetTickMarksInterval(this.TickCount);
			this.Scale.MinorTickMark.Interval = tickMarksInterval;
			this.Pointer.Placement = Placement.Cross;
			if (mapCore != null && mapCore.Viewport != null)
			{
				this.Pointer.Position = (double)mapCore.Viewport.Zoom;
			}
			this.Pointer.SnappingEnabled = true;
			this.Pointer.SnappingInterval = tickMarksInterval;
			this.Pointer.FillGradientType = GradientType.None;
			this.Pointer.FillHatchStyle = MapHatchStyle.None;
			this.Pointer.ShadowOffset = 0f;
			if (mapCore != null && mapCore.Viewport != null)
			{
				this.ZoomLevel = (double)mapCore.Viewport.Zoom;
			}
			this.zoomInButton = new PanelButton(this, PanelButtonType.ZoomButton, PanelButtonStyle.RoundedRectangle, this.zoomButtonClickHandler);
			this.zoomOutButton = new PanelButton(this, PanelButtonType.ZoomOut, PanelButtonStyle.RoundedRectangle, this.zoomButtonClickHandler);
			this.ApplyStyle();
			this.ApplyColors();
		}

		public float GetThumbPosition()
		{
			return (float)this.Pointer.Value;
		}

		public void SetThumbPosition(float thumbPosition)
		{
			thumbPosition = Math.Min(100f, thumbPosition);
			thumbPosition = Math.Max(0f, thumbPosition);
			this.Pointer.Value = (double)thumbPosition;
		}

		internal override void BeginInit()
		{
			base.BeginInit();
			this.Scale.BeginInit();
			this.Pointer.BeginInit();
		}

		internal override void EndInit()
		{
			base.EndInit();
			this.Scale.EndInit();
			this.Pointer.EndInit();
		}

		protected override void OnDispose()
		{
			base.OnDispose();
			this.Scale.Dispose();
			this.Pointer.Dispose();
		}

		internal Orientation GetOrientation()
		{
			if (this.Orientation == Orientation.Auto)
			{
				RectangleF boundsInPixels = base.GetBoundsInPixels();
				if (boundsInPixels.Width < boundsInPixels.Height)
				{
					return Orientation.Vertical;
				}
				return Orientation.Horizontal;
			}
			return this.Orientation;
		}

		internal bool GetReversed()
		{
			return this.Reversed;
		}

		internal void InternalZoomLevelChanged()
		{
			try
			{
				this.internalZoomChange = true;
				MapCore mapCore = base.GetMapCore();
				if (mapCore != null && mapCore.Viewport != null && !this.fixThumbPoition)
				{
					mapCore.Viewport.Zoom = (float)this.GetZoomLevelFromPointerPosition(this.Pointer.Value);
				}
			}
			finally
			{
				this.internalZoomChange = false;
			}
		}

		internal void UpdateZoomRange()
		{
			if (this.Common != null && this.Common.MapCore != null)
			{
				try
				{
					this.fixThumbPoition = true;
					this.Pointer.Value = this.GetPointerPositionFromZoomLevel((double)this.Common.MapCore.Viewport.Zoom);
				}
				finally
				{
					this.fixThumbPoition = false;
				}
			}
		}

		internal double GetSnappingInterval()
		{
			if (this.Pointer.SnappingEnabled)
			{
				return this.Pointer.SnappingInterval;
			}
			return 0.25;
		}

		internal double GetNextZoomLevel(double currentZoom, int zoomLevels, bool zoomIn)
		{
			double pointerPositionFromZoomLevel = this.GetPointerPositionFromZoomLevel(currentZoom);
			pointerPositionFromZoomLevel += (zoomIn ? ((double)zoomLevels * this.GetSnappingInterval()) : ((double)(-zoomLevels) * this.GetSnappingInterval()));
			pointerPositionFromZoomLevel = this.Scale.GetValueLimit(pointerPositionFromZoomLevel, this.Pointer.SnappingEnabled, this.Pointer.SnappingInterval);
			return this.GetZoomLevelFromPointerPosition(pointerPositionFromZoomLevel);
		}

		private void ApplyStyle()
		{
			switch (this.PanelStyle)
			{
			case ZoomPanelStyle.RectangularButtons:
				this.zoomInButton.Style = PanelButtonStyle.RoundedRectangle;
				this.zoomOutButton.Style = PanelButtonStyle.RoundedRectangle;
				this.Pointer.MarkerStyle = MarkerStyle.Rectangle;
				this.Pointer.MarkerLength = 60f;
				this.Pointer.Width = 30f;
				break;
			case ZoomPanelStyle.CircularButtons:
				this.zoomInButton.Style = PanelButtonStyle.Circle;
				this.zoomOutButton.Style = PanelButtonStyle.Circle;
				this.Pointer.MarkerStyle = MarkerStyle.Circle;
				this.Pointer.MarkerLength = 60f;
				this.Pointer.Width = 30f;
				break;
			}
		}

		private void ApplyColors()
		{
			this.zoomInButton.BackColor = this.ButtonColor;
			this.zoomInButton.BorderColor = this.ButtonBorderColor;
			this.zoomInButton.SymbolColor = this.SymbolColor;
			this.zoomInButton.SymbolBorderColor = this.SymbolBorderColor;
			this.zoomOutButton.BackColor = this.ButtonColor;
			this.zoomOutButton.BorderColor = this.ButtonBorderColor;
			this.zoomOutButton.SymbolColor = this.SymbolColor;
			this.zoomOutButton.SymbolBorderColor = this.SymbolBorderColor;
			this.Pointer.FillColor = this.ThumbColor;
			this.Pointer.BorderColor = this.ThumbBorderColor;
			this.Scale.BorderColor = this.SliderBarBorderColor;
			this.Scale.FillColor = this.SliderBarColor;
			this.Scale.MinorTickMark.BorderColor = this.TickBorderColor;
			this.Scale.MinorTickMark.FillColor = this.TickColor;
		}

		private void zoomButtonClickHandler(object sender, EventArgs e)
		{
			if (this.zoomInButton == sender)
			{
				double value = this.Pointer.Value + this.GetSnappingInterval();
				value = this.Scale.GetValueLimit(value, this.Pointer.SnappingEnabled, this.Pointer.SnappingInterval);
				this.Pointer.Value = value;
			}
			else if (this.zoomOutButton == sender)
			{
				double value2 = this.Pointer.Value - this.GetSnappingInterval();
				value2 = this.Scale.GetValueLimit(value2, this.Pointer.SnappingEnabled, this.Pointer.SnappingInterval);
				this.Pointer.Value = value2;
			}
		}

		private void AdjustAutoOrientationForDocking(PanelDockStyle dockStyle)
		{
			if (this.Orientation == Orientation.Auto)
			{
				if (this.GetOrientation() != Orientation.Vertical || (dockStyle != PanelDockStyle.Bottom && dockStyle != PanelDockStyle.Top))
				{
					if (this.GetOrientation() != 0)
					{
						return;
					}
					if (dockStyle != PanelDockStyle.Left && dockStyle != PanelDockStyle.Right)
					{
						return;
					}
				}
				MapSize size = new MapSize(this, this.Size.Height, this.Size.Width);
				PanelMargins panelMargins2 = base.Margins = new PanelMargins(base.Margins.Bottom, base.Margins.Right, base.Margins.Top, base.Margins.Left);
				this.Size = size;
			}
		}

		internal double GetZoomLevelFromPointerPosition(double pos)
		{
			double minimumZoom = this.MinimumZoom;
			double num = 0.0;
			if (this.ZoomType == ZoomType.Quadratic)
			{
				double num2 = (this.MaximumZoom - minimumZoom) / 10000.0;
				return num2 * pos * pos + minimumZoom;
			}
			if (this.ZoomType == ZoomType.Exponential)
			{
				double num4 = (this.MaximumZoom - minimumZoom) / 10000.0;
				return minimumZoom * Math.Pow(this.MaximumZoom / minimumZoom, pos / 100.0);
			}
			double num3 = (this.MaximumZoom - minimumZoom) / 100.0;
			return num3 * pos + minimumZoom;
		}

		internal double GetPointerPositionFromZoomLevel(double zoom)
		{
			double minimumZoom = this.MinimumZoom;
			double num = 0.0;
			if (this.ZoomType == ZoomType.Quadratic)
			{
				double num2 = (this.MaximumZoom - minimumZoom) / 10000.0;
				return Math.Sqrt((zoom - minimumZoom) / num2);
			}
			if (this.ZoomType == ZoomType.Exponential)
			{
				double num3 = Math.Log(this.MaximumZoom / minimumZoom) / 100.0;
				return Math.Log(zoom / minimumZoom) / num3;
			}
			double num4 = (this.MaximumZoom - minimumZoom) / 100.0;
			return (zoom - minimumZoom) / num4;
		}

		private double GetTickMarksInterval(int tickNumber)
		{
			return 100.0 / (double)(tickNumber - 1) - 4.94065645841247E-324;
		}

		internal float[] GetPossibleZoomLevels(float currentZoom)
		{
			if (!this.SnapToTickMarks)
			{
				return null;
			}
			currentZoom = (float)(currentZoom * 100.0);
			currentZoom = (float)Math.Round((double)currentZoom);
			currentZoom = (float)(currentZoom / 100.0);
			bool flag = true;
			ArrayList arrayList = new ArrayList();
			double num = 0.0;
			double num2 = 100.0 / (double)(this.TickCount - 1);
			for (int i = 0; i < this.TickCount; i++)
			{
				float num3 = (float)this.GetZoomLevelFromPointerPosition(num);
				num += num2;
				num3 = (float)(num3 * 100.0);
				num3 = (float)Math.Round((double)num3);
				num3 = (float)(num3 / 100.0);
				if (num3 == currentZoom)
				{
					flag = false;
				}
				arrayList.Add(num3);
			}
			if (flag)
			{
				arrayList.Insert(0, currentZoom);
			}
			return (float[])arrayList.ToArray(typeof(float));
		}

		internal void RenderStaticElements(MapGraphics g)
		{
			g.StartHotRegion(this);
			g.EndHotRegion();
			this.RenderStaticShadows(g);
			this.Scale.RenderStaticElements(g);
			if (this.ZoomButtonsVisible)
			{
				this.RenderButton(g, this.zoomInButton);
				this.RenderButton(g, this.zoomOutButton);
			}
		}

		private void AdjustScaleSize(MapGraphics g)
		{
			float markerLength = this.Pointer.MarkerLength;
			float width = this.Pointer.Width;
			float num = 0f;
			if (this.GetOrientation() == Orientation.Horizontal)
			{
				markerLength = g.GetAbsoluteY(markerLength);
				width = g.GetAbsoluteY(width);
				num = (float)((g.GetAbsoluteY(100f) - markerLength) / 2.0);
			}
			else
			{
				markerLength = g.GetAbsoluteX(markerLength);
				width = g.GetAbsoluteX(width);
				num = (float)((g.GetAbsoluteX(100f) - markerLength) / 2.0);
			}
			float num2 = (float)(2.0 + width / 2.0);
			if (this.ZoomButtonsVisible)
			{
				num2 += markerLength + num;
			}
			num2 = ((this.GetOrientation() != 0) ? g.GetRelativeY(num2) : g.GetRelativeX(num2));
			num2 = Math.Min(Math.Max(0f, num2), 100f);
			this.Scale.StartMargin = num2;
			this.Scale.EndMargin = num2;
		}

		internal void RenderDynamicElements(MapGraphics g)
		{
			this.RenderDynamicShadows(g);
			this.Pointer.Render(g);
		}

		internal void RenderDynamicShadows(MapGraphics g)
		{
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				GraphicsPath shadowPath = this.Pointer.GetShadowPath(g);
				if (shadowPath != null)
				{
					graphicsPath.AddPath(shadowPath, false);
				}
				graphicsPath.FillMode = FillMode.Winding;
				using (Brush brush = g.GetShadowBrush())
				{
					g.FillPath(brush, graphicsPath);
				}
			}
		}

		internal void RenderStaticShadows(MapGraphics g)
		{
			using (GraphicsPath graphicsPath = new GraphicsPath())
			{
				GraphicsPath shadowPath = this.Scale.GetShadowPath();
				if (shadowPath != null)
				{
					graphicsPath.AddPath(shadowPath, false);
				}
				graphicsPath.FillMode = FillMode.Winding;
				using (Brush brush = g.GetShadowBrush())
				{
					g.FillPath(brush, graphicsPath);
				}
			}
		}

		private void RenderButton(MapGraphics g, PanelButton button)
		{
			float markerLength = this.Pointer.MarkerLength;
			markerLength = (float)(markerLength + 5.0);
			float num = 0f;
			float num2 = 0f;
			float relativeY;
			float num3;
			PointF relative;
			float relativeX;
			if (this.GetOrientation() == Orientation.Horizontal)
			{
				relativeY = (float)((100.0 - markerLength) / 2.0);
				relativeY = g.GetRelativeX(g.GetAbsoluteY(relativeY));
				markerLength = g.GetRelativeX(g.GetAbsoluteY(markerLength));
				num3 = g.GetAbsoluteX(markerLength);
				relative = new PointF((float)(markerLength / 2.0 + relativeY), 50f);
				if (button.Type == PanelButtonType.ZoomOut && this.GetReversed())
				{
					goto IL_00a1;
				}
				if (button.Type == PanelButtonType.ZoomButton && !this.GetReversed())
				{
					goto IL_00a1;
				}
			}
			else
			{
				relativeX = (float)((100.0 - markerLength) / 2.0);
				relativeX = g.GetRelativeY(g.GetAbsoluteX(relativeX));
				markerLength = g.GetRelativeY(g.GetAbsoluteX(markerLength));
				num3 = g.GetAbsoluteY(markerLength);
				relative = new PointF(50f, (float)(markerLength / 2.0 + relativeX));
				if (button.Type == PanelButtonType.ZoomOut && !this.GetReversed())
				{
					goto IL_0126;
				}
				if (button.Type == PanelButtonType.ZoomButton && this.GetReversed())
				{
					goto IL_0126;
				}
			}
			goto IL_0138;
			IL_00a1:
			num = (float)(100.0 - markerLength - 2.0 * relativeY);
			goto IL_0138;
			IL_0126:
			num2 = (float)(100.0 - markerLength - 2.0 * relativeX);
			goto IL_0138;
			IL_0138:
			relative.X += num;
			relative.Y += num2;
			relative = g.GetAbsolutePoint(relative);
			RectangleF absolute = new RectangleF(relative.X, relative.Y, 0f, 0f);
			absolute.Inflate((float)(num3 / 2.0), (float)(num3 / 2.0));
			button.Bounds = g.GetRelativeRectangle(absolute);
			button.Render(g);
		}

		internal override void Render(MapGraphics g)
		{
			this.AbsoluteSize = g.GetAbsoluteSize(this.Size);
			this.AdjustScaleSize(g);
			switch (base.GetMapCore().RenderingMode)
			{
			case RenderingMode.ZoomThumb:
				this.RenderDynamicElements(g);
				break;
			case RenderingMode.SinglePanel:
				base.Render(g);
				this.RenderStaticElements(g);
				break;
			default:
				base.Render(g);
				this.RenderStaticElements(g);
				this.RenderDynamicElements(g);
				break;
			}
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Size":
				return new MapSize(null, 40f, 200f);
			case "Dock":
				return PanelDockStyle.Left;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}
	}
}
