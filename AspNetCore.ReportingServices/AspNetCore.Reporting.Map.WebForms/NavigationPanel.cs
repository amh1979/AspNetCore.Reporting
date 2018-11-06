using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	[TypeConverter(typeof(DockablePanelConverter))]
	internal class NavigationPanel : DockablePanel, IToolTipProvider
	{
		private const int GirdPadding = 4;

		private const int CenterButtonCorrection = 2;

		internal PanelButton buttonNorth;

		internal PanelButton buttonSouth;

		internal PanelButton buttonEast;

		internal PanelButton buttonWest;

		private PanelButton buttonCenter;

		private Color symbolColor = Color.LightGray;

		private Color symbolBorderColor = Color.DimGray;

		private Color buttonColor = Color.White;

		private Color buttonBorderColor = Color.DarkGray;

		private NavigationPanelStyle style;

		private double scrollStep = 10.0;

		[DefaultValue(typeof(Color), "LightGray")]
		[SRCategory("CategoryAttribute_Appearance")]
		[SRDescription("DescriptionAttributeNavigationPanel_SymbolColor")]
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

		[SRDescription("DescriptionAttributeNavigationPanel_SymbolBorderColor")]
		[DefaultValue(typeof(Color), "DimGray")]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[DefaultValue(typeof(Color), "White")]
		[NotifyParentProperty(true)]
		[SRDescription("DescriptionAttributeNavigationPanel_ButtonColor")]
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

		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "DarkGray")]
		[SRDescription("DescriptionAttributeNavigationPanel_ButtonBorderColor")]
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

		[SRDescription("DescriptionAttributeNavigationPanel_PanelStyle")]
		[DefaultValue(NavigationPanelStyle.RectangularButtons)]
		[SRCategory("CategoryAttribute_Appearance")]
		[NotifyParentProperty(true)]
		public NavigationPanelStyle PanelStyle
		{
			get
			{
				return this.style;
			}
			set
			{
				if (this.style != value)
				{
					this.style = value;
					this.ApplyStyle();
					this.Invalidate();
				}
			}
		}

		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Behavior")]
		[SRDescription("DescriptionAttributeNavigationPanel_ScrollStep")]
		[DefaultValue(10.0)]
		public double ScrollStep
		{
			get
			{
				return this.scrollStep;
			}
			set
			{
				if (this.scrollStep != value)
				{
					this.scrollStep = value;
					this.Invalidate();
				}
			}
		}

		internal bool CenterButtonVisible
		{
			get
			{
				return false;
			}
		}

		public NavigationPanel()
			: this(null)
		{
		}

		internal NavigationPanel(CommonElements common)
			: base(common)
		{
			this.Name = "NavigationPanel";
			this.buttonNorth = new PanelButton(this, PanelButtonType.NavigationButton, this.NavigationButtonClickHandler);
			this.buttonSouth = new PanelButton(this, PanelButtonType.NaviagateSouth, this.NavigationButtonClickHandler);
			this.buttonEast = new PanelButton(this, PanelButtonType.NaviagateEast, this.NavigationButtonClickHandler);
			this.buttonWest = new PanelButton(this, PanelButtonType.NaviagateWest, this.NavigationButtonClickHandler);
			this.buttonCenter = new PanelButton(this, PanelButtonType.NaviagateCenter, this.NavigationButtonClickHandler);
			this.ApplyStyle();
			this.ApplyColors();
		}

		internal override void Render(MapGraphics g)
		{
			base.Render(g);
			this.RenderButton(g, this.buttonNorth);
			this.RenderButton(g, this.buttonSouth);
			this.RenderButton(g, this.buttonWest);
			this.RenderButton(g, this.buttonEast);
		}

		internal override object GetDefaultPropertyValue(string prop, object currentValue)
		{
			object obj = null;
			switch (prop)
			{
			case "Margins":
				return new PanelMargins(5, 5, 5, 5);
			case "Size":
				return new MapSize(null, 90f, 90f);
			case "Dock":
				return PanelDockStyle.Left;
			default:
				return base.GetDefaultPropertyValue(prop, currentValue);
			}
		}

		private void RenderButton(MapGraphics g, PanelButton button)
		{
			float num = (float)(g.GetAbsoluteDimension(100f) - 1.0);
			SizeF absoluteSize = g.GetAbsoluteSize(new SizeF(100f, 100f));
			absoluteSize.Width -= 1f;
			absoluteSize.Height -= 1f;
			SizeF sizeF = new SizeF((float)((absoluteSize.Width - num) / 2.0), (float)((absoluteSize.Height - num) / 2.0));
			float num2 = (float)(num / 3.0);
			PointF pointF = PointF.Empty;
			switch (button.Type)
			{
			case PanelButtonType.NavigationButton:
				pointF = new PointF((float)(num / 2.0), (float)(num2 / 2.0));
				if (!this.CenterButtonVisible)
				{
					pointF.Y += 2f;
				}
				break;
			case PanelButtonType.NaviagateSouth:
				pointF = new PointF((float)(num / 2.0), (float)(num - num2 / 2.0));
				if (!this.CenterButtonVisible)
				{
					pointF.Y -= 2f;
				}
				break;
			case PanelButtonType.NaviagateEast:
				pointF = new PointF((float)(num - num2 / 2.0), (float)(num / 2.0));
				if (!this.CenterButtonVisible)
				{
					pointF.X -= 2f;
				}
				break;
			case PanelButtonType.NaviagateWest:
				pointF = new PointF((float)(num2 / 2.0), (float)(num / 2.0));
				if (!this.CenterButtonVisible)
				{
					pointF.X += 2f;
				}
				break;
			case PanelButtonType.NaviagateCenter:
				pointF = new PointF((float)(num / 2.0), (float)(num / 2.0));
				break;
			default:
				throw new ArgumentException(SR.invalid_button_type);
			}
			num2 = (float)(num2 - 4.0);
			RectangleF absolute = new RectangleF((float)(sizeF.Width + pointF.X - num2 / 2.0), (float)(sizeF.Height + pointF.Y - num2 / 2.0), num2, num2);
			button.Bounds = g.GetRelativeRectangle(absolute);
			button.Render(g);
		}

		private void NavigationButtonClickHandler(object sender, EventArgs e)
		{
			PanelButton panelButton = (PanelButton)sender;
			MapCore mapCore = base.GetMapCore();
			if (mapCore != null)
			{
				switch (panelButton.Type)
				{
				case (PanelButtonType)35:
					break;
				case PanelButtonType.NavigationButton:
					mapCore.Scroll(ScrollDirection.North, this.ScrollStep);
					break;
				case PanelButtonType.NaviagateSouth:
					mapCore.Scroll(ScrollDirection.South, this.ScrollStep);
					break;
				case PanelButtonType.NaviagateEast:
					mapCore.Scroll(ScrollDirection.East, this.ScrollStep);
					break;
				case PanelButtonType.NaviagateWest:
					mapCore.Scroll(ScrollDirection.West, this.ScrollStep);
					break;
				}
			}
		}

		private void ApplyStyle()
		{
			switch (this.PanelStyle)
			{
			case NavigationPanelStyle.RectangularButtons:
				this.buttonNorth.Style = PanelButtonStyle.RoundedRectangle;
				this.buttonSouth.Style = PanelButtonStyle.RoundedRectangle;
				this.buttonEast.Style = PanelButtonStyle.RoundedRectangle;
				this.buttonWest.Style = PanelButtonStyle.RoundedRectangle;
				this.buttonCenter.Style = PanelButtonStyle.RoundedRectangle;
				break;
			case NavigationPanelStyle.TriangularButtons:
				this.buttonNorth.Style = PanelButtonStyle.Triangle;
				this.buttonSouth.Style = PanelButtonStyle.Triangle;
				this.buttonEast.Style = PanelButtonStyle.Triangle;
				this.buttonWest.Style = PanelButtonStyle.Triangle;
				this.buttonCenter.Style = PanelButtonStyle.Circle;
				break;
			}
		}

		private void ApplyColors()
		{
			this.buttonNorth.BackColor = this.ButtonColor;
			this.buttonNorth.SymbolColor = this.SymbolColor;
			this.buttonNorth.SymbolBorderColor = this.SymbolBorderColor;
			this.buttonNorth.BorderColor = this.ButtonBorderColor;
			this.buttonSouth.BackColor = this.ButtonColor;
			this.buttonSouth.SymbolColor = this.SymbolColor;
			this.buttonSouth.SymbolBorderColor = this.SymbolBorderColor;
			this.buttonSouth.BorderColor = this.ButtonBorderColor;
			this.buttonEast.BackColor = this.ButtonColor;
			this.buttonEast.SymbolColor = this.SymbolColor;
			this.buttonEast.SymbolBorderColor = this.SymbolBorderColor;
			this.buttonEast.BorderColor = this.ButtonBorderColor;
			this.buttonWest.BackColor = this.ButtonColor;
			this.buttonWest.SymbolColor = this.SymbolColor;
			this.buttonWest.SymbolBorderColor = this.SymbolBorderColor;
			this.buttonWest.BorderColor = this.ButtonBorderColor;
			this.buttonCenter.BackColor = this.ButtonColor;
			this.buttonCenter.SymbolColor = this.SymbolColor;
			this.buttonCenter.SymbolBorderColor = this.SymbolBorderColor;
			this.buttonCenter.BorderColor = this.ButtonBorderColor;
		}
	}
}
