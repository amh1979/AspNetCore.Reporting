using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartThreeDProperties
	{
		private Chart m_chart;

		private ThreeDProperties m_renderThreeDPropertiesDef;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties m_chartThreeDPropertiesDef;

		private ChartThreeDPropertiesInstance m_instance;

		private ReportBoolProperty m_clustered;

		private ReportIntProperty m_wallThickness;

		private ReportIntProperty m_gapDepth;

		private ReportEnumProperty<ChartThreeDShadingTypes> m_shading;

		private ReportIntProperty m_depthRatio;

		private ReportIntProperty m_rotation;

		private ReportIntProperty m_inclination;

		private ReportIntProperty m_perspective;

		private ReportEnumProperty<ChartThreeDProjectionModes> m_projectionMode;

		private ReportBoolProperty m_enabled;

		public ReportBoolProperty Enabled
		{
			get
			{
				if (this.m_enabled == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_enabled = new ReportBoolProperty(this.m_renderThreeDPropertiesDef.Enabled);
					}
					else if (this.m_chartThreeDPropertiesDef.Enabled != null)
					{
						this.m_enabled = new ReportBoolProperty(this.m_chartThreeDPropertiesDef.Enabled);
					}
				}
				return this.m_enabled;
			}
		}

		public ReportEnumProperty<ChartThreeDProjectionModes> ProjectionMode
		{
			get
			{
				if (this.m_projectionMode == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_projectionMode = new ReportEnumProperty<ChartThreeDProjectionModes>((ChartThreeDProjectionModes)((!this.m_renderThreeDPropertiesDef.PerspectiveProjectionMode) ? 1 : 0));
					}
					else if (this.m_chartThreeDPropertiesDef.ProjectionMode != null)
					{
						this.m_projectionMode = new ReportEnumProperty<ChartThreeDProjectionModes>(this.m_chartThreeDPropertiesDef.ProjectionMode.IsExpression, this.m_chartThreeDPropertiesDef.ProjectionMode.OriginalText, EnumTranslator.TranslateChartThreeDProjectionMode(this.m_chartThreeDPropertiesDef.ProjectionMode.StringValue, null));
					}
				}
				return this.m_projectionMode;
			}
		}

		public ReportIntProperty Perspective
		{
			get
			{
				if (this.m_perspective == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_perspective = new ReportIntProperty(this.m_renderThreeDPropertiesDef.Perspective);
					}
					else if (this.m_chartThreeDPropertiesDef.Perspective != null)
					{
						this.m_perspective = new ReportIntProperty(this.m_chartThreeDPropertiesDef.Perspective);
					}
				}
				return this.m_perspective;
			}
		}

		public ReportIntProperty Rotation
		{
			get
			{
				if (this.m_rotation == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_rotation = new ReportIntProperty(this.m_renderThreeDPropertiesDef.Rotation);
					}
					else if (this.m_chartThreeDPropertiesDef.Rotation != null)
					{
						this.m_rotation = new ReportIntProperty(this.m_chartThreeDPropertiesDef.Rotation);
					}
				}
				return this.m_rotation;
			}
		}

		public ReportIntProperty Inclination
		{
			get
			{
				if (this.m_inclination == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_inclination = new ReportIntProperty(this.m_renderThreeDPropertiesDef.Inclination);
					}
					else if (this.m_chartThreeDPropertiesDef.Inclination != null)
					{
						this.m_inclination = new ReportIntProperty(this.m_chartThreeDPropertiesDef.Inclination);
					}
				}
				return this.m_inclination;
			}
		}

		public ReportIntProperty DepthRatio
		{
			get
			{
				if (this.m_depthRatio == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_depthRatio = new ReportIntProperty(this.m_renderThreeDPropertiesDef.DepthRatio);
					}
					else if (this.m_chartThreeDPropertiesDef.DepthRatio != null)
					{
						this.m_depthRatio = new ReportIntProperty(this.m_chartThreeDPropertiesDef.DepthRatio);
					}
				}
				return this.m_depthRatio;
			}
		}

		public ReportEnumProperty<ChartThreeDShadingTypes> Shading
		{
			get
			{
				if (this.m_shading == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						ChartThreeDShadingTypes value = ChartThreeDShadingTypes.None;
						if (this.m_renderThreeDPropertiesDef.Shading == ThreeDProperties.ShadingTypes.Real)
						{
							value = ChartThreeDShadingTypes.Real;
						}
						else if (this.m_renderThreeDPropertiesDef.Shading == ThreeDProperties.ShadingTypes.Simple)
						{
							value = ChartThreeDShadingTypes.Simple;
						}
						this.m_shading = new ReportEnumProperty<ChartThreeDShadingTypes>(value);
					}
					else if (this.m_chartThreeDPropertiesDef.Shading != null)
					{
						this.m_shading = new ReportEnumProperty<ChartThreeDShadingTypes>(this.m_chartThreeDPropertiesDef.Shading.IsExpression, this.m_chartThreeDPropertiesDef.Shading.OriginalText, EnumTranslator.TranslateChartThreeDShading(this.m_chartThreeDPropertiesDef.Shading.StringValue, null));
					}
				}
				return this.m_shading;
			}
		}

		public ReportIntProperty GapDepth
		{
			get
			{
				if (this.m_gapDepth == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_gapDepth = new ReportIntProperty(this.m_renderThreeDPropertiesDef.GapDepth);
					}
					else if (this.m_chartThreeDPropertiesDef.GapDepth != null)
					{
						this.m_gapDepth = new ReportIntProperty(this.m_chartThreeDPropertiesDef.GapDepth);
					}
				}
				return this.m_gapDepth;
			}
		}

		public ReportIntProperty WallThickness
		{
			get
			{
				if (this.m_wallThickness == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_wallThickness = new ReportIntProperty(this.m_renderThreeDPropertiesDef.WallThickness);
					}
					else if (this.m_chartThreeDPropertiesDef.WallThickness != null)
					{
						this.m_wallThickness = new ReportIntProperty(this.m_chartThreeDPropertiesDef.WallThickness);
					}
				}
				return this.m_wallThickness;
			}
		}

		public ReportBoolProperty Clustered
		{
			get
			{
				if (this.m_clustered == null)
				{
					if (this.m_chart.IsOldSnapshot)
					{
						this.m_clustered = new ReportBoolProperty(this.m_renderThreeDPropertiesDef.Clustered);
					}
					else if (this.m_chartThreeDPropertiesDef.Clustered != null)
					{
						this.m_clustered = new ReportBoolProperty(this.m_chartThreeDPropertiesDef.Clustered);
					}
				}
				return this.m_clustered;
			}
		}

		internal Chart ChartDef
		{
			get
			{
				return this.m_chart;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties ChartThreeDPropertiesDef
		{
			get
			{
				return this.m_chartThreeDPropertiesDef;
			}
		}

		public ChartThreeDPropertiesInstance Instance
		{
			get
			{
				if (this.m_chart.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new ChartThreeDPropertiesInstance(this);
				}
				return this.m_instance;
			}
		}

		internal ChartThreeDProperties(AspNetCore.ReportingServices.ReportIntermediateFormat.ChartThreeDProperties threeDPropertiesDef, Chart chart)
		{
			this.m_chart = chart;
			this.m_chartThreeDPropertiesDef = threeDPropertiesDef;
		}

		internal ChartThreeDProperties(ThreeDProperties renderThreeDPropertiesDef, Chart chart)
		{
			this.m_chart = chart;
			this.m_renderThreeDPropertiesDef = renderThreeDPropertiesDef;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
