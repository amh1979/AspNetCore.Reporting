using System;
using System.ComponentModel;
using System.Drawing;

namespace AspNetCore.Reporting.Map.WebForms
{
	internal abstract class AutoSizePanel : DockablePanel
	{
		private bool autoSize = true;

		private float maximumPanelAutoSize = 100f;

		[RefreshProperties(RefreshProperties.All)]
		[SRDescription("DescriptionAttributeAutoSizePanel_AutoSize")]
		[DefaultValue(true)]
		[NotifyParentProperty(true)]
		[SRCategory("CategoryAttribute_Layout")]
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
					this.Size.AutoSize = value;
					this.SizeLocationChanged(SizeLocationChangeInfo.Size);
				}
			}
		}

		public override MapSize Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				if (!object.ReferenceEquals(this.Size, value))
				{
					value.AutoSize = this.AutoSize;
					base.Size = value;
				}
			}
		}

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
					if (this.AutoSize)
					{
						this.InvalidateAndLayout();
					}
				}
			}
		}

		internal abstract bool IsEmpty
		{
			get;
		}

		[SRCategory("CategoryAttribute_Layout")]
		[SRDescription("DescriptionAttributeAutoSizePanel_MaxAutoSize")]
		[DefaultValue(100f)]
		[NotifyParentProperty(true)]
		public virtual float MaxAutoSize
		{
			get
			{
				return this.maximumPanelAutoSize;
			}
			set
			{
				if (value < 0.0)
				{
					throw new ArgumentOutOfRangeException("MaxAutoSize", SR.ExceptionMaximumLegendAutoSize);
				}
				this.maximumPanelAutoSize = value;
				this.SizeLocationChanged(SizeLocationChangeInfo.Size);
			}
		}

		public AutoSizePanel()
			: this(null)
		{
		}

		internal AutoSizePanel(CommonElements common)
			: base(common)
		{
		}

		internal abstract SizeF GetOptimalSize(MapGraphics g, SizeF maxSizeAbs);

		internal void AdjustAutoSize(MapGraphics g)
		{
			if (this.AutoSize && this.Common != null)
			{
				SizeF sizeF = SizeF.Empty;
				if (base.DockedInsideViewport)
				{
					sizeF = this.Common.MapCore.Viewport.GetSizeInPixels();
					sizeF.Width -= (float)(this.Common.MapCore.Viewport.Margins.Left + this.Common.MapCore.Viewport.Margins.Right);
					sizeF.Height -= (float)(this.Common.MapCore.Viewport.Margins.Top + this.Common.MapCore.Viewport.Margins.Bottom);
				}
				else
				{
					sizeF = new SizeF((float)this.Common.MapCore.Width, (float)this.Common.MapCore.Height);
				}
				sizeF.Width -= (float)(base.Margins.Right + base.Margins.Left);
				sizeF.Height -= (float)(base.Margins.Top + base.Margins.Bottom);
				if (this.MaxAutoSize > 0.0 && this.MaxAutoSize < 100.0)
				{
					switch (this.Dock)
					{
					case PanelDockStyle.Top:
					case PanelDockStyle.Bottom:
						sizeF.Height = (float)(sizeF.Height * this.MaxAutoSize / 100.0);
						break;
					case PanelDockStyle.Left:
					case PanelDockStyle.Right:
						sizeF.Width = (float)(sizeF.Width * this.MaxAutoSize / 100.0);
						break;
					case PanelDockStyle.None:
						sizeF = this.CalculateUndockedAutoSize(sizeF);
						break;
					}
				}
				if ((double)sizeF.Width <= 0.1 || (double)sizeF.Height <= 0.1)
				{
					base.SetSizeInPixels(new SizeF(0.1f, 0.1f));
				}
				else
				{
					SizeF optimalSize = this.GetOptimalSize(g, sizeF);
					if (!float.IsNaN(optimalSize.Height) && !float.IsNaN(optimalSize.Width))
					{
						SizeF sizeInPixels = base.GetSizeInPixels();
						optimalSize.Width += (float)(base.Margins.Left + base.Margins.Right);
						optimalSize.Height += (float)(base.Margins.Top + base.Margins.Bottom);
						if (sizeInPixels.Width == optimalSize.Width && sizeInPixels.Height == optimalSize.Height)
						{
							return;
						}
						base.SetSizeInPixels(optimalSize);
					}
				}
			}
		}

		protected virtual SizeF CalculateUndockedAutoSize(SizeF size)
		{
			if (size.Width < size.Height)
			{
				size.Height = (float)(size.Height * this.MaxAutoSize / 100.0);
			}
			else
			{
				size.Width = (float)(size.Width * this.MaxAutoSize / 100.0);
			}
			return size;
		}

		internal override bool IsVisible()
		{
			bool flag = base.Visible;
			if (this.Common != null && !this.Common.MapControl.IsDesignMode())
			{
				flag &= !this.IsEmpty;
			}
			return flag;
		}

		internal override void Invalidate()
		{
			this.Invalidate(false);
		}

		protected void Invalidate(bool layout)
		{
			if (layout && this.AutoSize)
			{
				base.InvalidateAndLayout();
			}
			else
			{
				base.Invalidate();
			}
		}

		internal override bool IsRenderVisible(MapGraphics g, RectangleF clipRect)
		{
			if (base.IsRenderVisible(g, clipRect))
			{
				return !this.IsEmpty;
			}
			return false;
		}
	}
}
