using System;
using System.ComponentModel;

namespace AspNetCore.Reporting.Chart.WebForms
{
	internal class ChartArea3DStyle
	{
		private ChartArea chartArea;

		private bool enable3D;

		private bool rightAngleAxes = true;

		private bool clustered;

		private LightStyle light = LightStyle.Simplistic;

		private int perspective;

		private int xAngle = 30;

		private int yAngle = 30;

		private int wallWidth = 7;

		private int pointDepth = 100;

		private int pointGapDepth = 100;

		[DefaultValue(false)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_Enable3D")]
		[ParenthesizePropertyName(true)]
		[SRCategory("CategoryAttribute3D")]
		public bool Enable3D
		{
			get
			{
				return this.enable3D;
			}
			set
			{
				if (this.enable3D != value)
				{
					this.enable3D = value;
					if (this.chartArea != null)
					{
						this.chartArea.Invalidate(true);
					}
				}
			}
		}

		[Bindable(true)]
		[DefaultValue(true)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_RightAngleAxes")]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute3D")]
		public bool RightAngleAxes
		{
			get
			{
				return this.rightAngleAxes;
			}
			set
			{
				this.rightAngleAxes = value;
				if (this.rightAngleAxes)
				{
					this.perspective = 0;
				}
				if (this.chartArea != null)
				{
					this.chartArea.Invalidate(true);
				}
			}
		}

		[SRDescription("DescriptionAttributeChartArea3DStyle_Clustered")]
		[Bindable(true)]
		[SRCategory("CategoryAttribute3D")]
		[DefaultValue(false)]
		public bool Clustered
		{
			get
			{
				return this.clustered;
			}
			set
			{
				this.clustered = value;
				if (this.chartArea != null)
				{
					this.chartArea.Invalidate(true);
				}
			}
		}

		[Bindable(true)]
		[DefaultValue(typeof(LightStyle), "Simplistic")]
		[SRDescription("DescriptionAttributeChartArea3DStyle_Light")]
		[SRCategory("CategoryAttribute3D")]
		public LightStyle Light
		{
			get
			{
				return this.light;
			}
			set
			{
				this.light = value;
				if (this.chartArea != null)
				{
					this.chartArea.Invalidate(true);
				}
			}
		}

		[DefaultValue(0)]
		[Bindable(true)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_Perspective")]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute3D")]
		public int Perspective
		{
			get
			{
				return this.perspective;
			}
			set
			{
				if (value >= 0 && value <= 100)
				{
					this.perspective = value;
					if (this.perspective != 0)
					{
						this.rightAngleAxes = false;
					}
					if (this.chartArea != null)
					{
						this.chartArea.Invalidate(true);
					}
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionChartArea3DPerspectiveInvalid);
			}
		}

		[Bindable(true)]
		[DefaultValue(30)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_XAngle")]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute3D")]
		public int XAngle
		{
			get
			{
				return this.xAngle;
			}
			set
			{
				if (value >= -90 && value <= 90)
				{
					this.xAngle = value;
					if (this.chartArea != null)
					{
						this.chartArea.Invalidate(true);
					}
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionChartArea3DXAxisRotationInvalid);
			}
		}

		[DefaultValue(30)]
		[SRCategory("CategoryAttribute3D")]
		[SRDescription("DescriptionAttributeChartArea3DStyle_YAngle")]
		[RefreshProperties(RefreshProperties.All)]
		[Bindable(true)]
		public int YAngle
		{
			get
			{
				return this.yAngle;
			}
			set
			{
				if (value >= -180 && value <= 180)
				{
					this.yAngle = value;
					if (this.chartArea != null)
					{
						this.chartArea.Invalidate(true);
					}
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionChartArea3DYAxisRotationInvalid);
			}
		}

		[Bindable(true)]
		[DefaultValue(7)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_WallWidth")]
		[RefreshProperties(RefreshProperties.All)]
		[SRCategory("CategoryAttribute3D")]
		public int WallWidth
		{
			get
			{
				return this.wallWidth;
			}
			set
			{
				if (value >= 0 && value <= 30)
				{
					this.wallWidth = value;
					if (this.chartArea != null)
					{
						this.chartArea.Invalidate(true);
					}
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionChartArea3DWallWidthInvalid);
			}
		}

		[Bindable(true)]
		[SRCategory("CategoryAttribute3D")]
		[SRDescription("DescriptionAttributeChartArea3DStyle_PointDepth")]
		[RefreshProperties(RefreshProperties.All)]
		[DefaultValue(100)]
		public int PointDepth
		{
			get
			{
				return this.pointDepth;
			}
			set
			{
				if (value >= 0 && value <= 1000)
				{
					this.pointDepth = value;
					if (this.chartArea != null)
					{
						this.chartArea.Invalidate(true);
					}
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionChartArea3DPointsDepthInvalid);
			}
		}

		[Bindable(true)]
		[DefaultValue(100)]
		[SRDescription("DescriptionAttributeChartArea3DStyle_PointGapDepth")]
		[SRCategory("CategoryAttribute3D")]
		[RefreshProperties(RefreshProperties.All)]
		public int PointGapDepth
		{
			get
			{
				return this.pointGapDepth;
			}
			set
			{
				if (value >= 0 && value <= 1000)
				{
					this.pointGapDepth = value;
					if (this.chartArea != null)
					{
						this.chartArea.Invalidate(true);
					}
					return;
				}
				throw new ArgumentOutOfRangeException("value", SR.ExceptionChartArea3DPointsGapInvalid);
			}
		}

		public ChartArea3DStyle()
		{
		}

		public ChartArea3DStyle(ChartArea chartArea)
		{
			this.chartArea = chartArea;
		}

		internal void Initialize(ChartArea chartArea)
		{
			this.chartArea = chartArea;
		}
	}
}
