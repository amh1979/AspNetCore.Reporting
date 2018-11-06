using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[DefaultProperty("SkinStyle")]
	[SRDescription("DescriptionAttributeBorderSkinAttributes_BorderSkinAttributes")]
	internal class BorderSkinAttributes
	{
		internal IServiceContainer serviceContainer;

		private Color pageColor = Color.White;

		private BorderSkinStyle skinStyle;

		private GradientType backGradientType;

		private Color backGradientEndColor = Color.Empty;

		private Color backColor = Color.Gray;

		private string backImage = "";

		private ChartImageWrapMode backImageMode;

		private Color backImageTranspColor = Color.Empty;

		private ChartImageAlign backImageAlign;

		private Color borderColor = Color.Black;

		private int borderWidth = 1;

		private ChartDashStyle borderStyle;

		private ChartHatchStyle backHatchStyle;

		internal object ownerElement;

		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "White")]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_PageColor")]
		[SRCategory("CategoryAttributeAppearance")]
		public Color PageColor
		{
			get
			{
				return this.pageColor;
			}
			set
			{
				this.pageColor = value;
				this.Invalidate();
			}
		}

		[ParenthesizePropertyName(true)]
		[Bindable(true)]
		[DefaultValue(BorderSkinStyle.None)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_SkinStyle")]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		public BorderSkinStyle SkinStyle
		{
			get
			{
				return this.skinStyle;
			}
			set
			{
				this.skinStyle = value;
				this.Invalidate();
			}
		}

		[DefaultValue(typeof(Color), "Gray")]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[Browsable(false)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackColor")]
		[Bindable(true)]
		public Color FrameBackColor
		{
			get
			{
				return this.backColor;
			}
			set
			{
				this.backColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "Black")]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBorderColor")]
		[Browsable(false)]
		public Color FrameBorderColor
		{
			get
			{
				return this.borderColor;
			}
			set
			{
				this.borderColor = value;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(ChartHatchStyle.None)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackHatchStyle")]
		[Bindable(true)]
		public ChartHatchStyle FrameBackHatchStyle
		{
			get
			{
				return this.backHatchStyle;
			}
			set
			{
				this.backHatchStyle = value;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue("")]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackImage")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string FrameBackImage
		{
			get
			{
				return this.backImage;
			}
			set
			{
				this.backImage = value;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(ChartImageWrapMode.Tile)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackImageMode")]
		[SRCategory("CategoryAttributeAppearance")]
		public ChartImageWrapMode FrameBackImageMode
		{
			get
			{
				return this.backImageMode;
			}
			set
			{
				this.backImageMode = value;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackImageTransparentColor")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public Color FrameBackImageTransparentColor
		{
			get
			{
				return this.backImageTranspColor;
			}
			set
			{
				this.backImageTranspColor = value;
				this.Invalidate();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(ChartImageAlign.TopLeft)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackImageAlign")]
		[Browsable(false)]
		public ChartImageAlign FrameBackImageAlign
		{
			get
			{
				return this.backImageAlign;
			}
			set
			{
				this.backImageAlign = value;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(GradientType.None)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackGradientType")]
		[SRCategory("CategoryAttributeAppearance")]
		public GradientType FrameBackGradientType
		{
			get
			{
				return this.backGradientType;
			}
			set
			{
				this.backGradientType = value;
				this.Invalidate();
			}
		}

		[Browsable(false)]
		[SRCategory("CategoryAttributeAppearance")]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Color), "")]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBackGradientEndColor")]
		[Bindable(true)]
		public Color FrameBackGradientEndColor
		{
			get
			{
				return this.backGradientEndColor;
			}
			set
			{
				this.backGradientEndColor = value;
				this.Invalidate();
			}
		}

		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(1)]
		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBorderWidth")]
		[Browsable(false)]
		public int FrameBorderWidth
		{
			get
			{
				return this.borderWidth;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", SR.ExceptionBorderWidthIsNotPositive);
				}
				this.borderWidth = value;
				this.Invalidate();
			}
		}

		[SRDescription("DescriptionAttributeBorderSkinAttributes_FrameBorderStyle")]
		[SRCategory("CategoryAttributeAppearance")]
		[Bindable(true)]
		[NotifyParentProperty(true)]
		[DefaultValue(ChartDashStyle.NotSet)]
		[Browsable(false)]
		public ChartDashStyle FrameBorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				this.borderStyle = value;
				this.Invalidate();
			}
		}

		public BorderSkinAttributes()
		{
		}

		internal BorderSkinAttributes(IServiceContainer container)
		{
			this.serviceContainer = container;
		}

		private void Invalidate()
		{
		}
	}
}
