namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ChartThreeDPropertiesInstance : BaseInstance
	{
		private ChartThreeDProperties m_chartThreeDPropertiesDef;

		private bool? m_enabled;

		private ChartThreeDProjectionModes? m_projectionMode;

		private int? m_perspective;

		private int? m_rotation;

		private int? m_inclination;

		private int? m_depthRatio;

		private ChartThreeDShadingTypes? m_shading;

		private int? m_gapDepth;

		private int? m_wallThickness;

		private bool? m_clustered;

		public bool Enabled
		{
			get
			{
				if (!this.m_enabled.HasValue)
				{
					if (this.m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						this.m_enabled = this.m_chartThreeDPropertiesDef.Enabled.Value;
					}
					else
					{
						this.m_enabled = this.m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateEnabled(this.ReportScopeInstance, this.m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_enabled.Value;
			}
		}

		public ChartThreeDProjectionModes ProjectionMode
		{
			get
			{
				if (!this.m_projectionMode.HasValue)
				{
					if (this.m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						this.m_projectionMode = this.m_chartThreeDPropertiesDef.ProjectionMode.Value;
					}
					else
					{
						this.m_projectionMode = this.m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateProjectionMode(this.ReportScopeInstance, this.m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_projectionMode.Value;
			}
		}

		public int Perspective
		{
			get
			{
				if (!this.m_perspective.HasValue)
				{
					if (this.m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						this.m_perspective = this.m_chartThreeDPropertiesDef.Perspective.Value;
					}
					else
					{
						this.m_perspective = this.m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluatePerspective(this.ReportScopeInstance, this.m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_perspective.Value;
			}
		}

		public int Rotation
		{
			get
			{
				if (!this.m_rotation.HasValue)
				{
					if (this.m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						this.m_rotation = this.m_chartThreeDPropertiesDef.Rotation.Value;
					}
					else
					{
						this.m_rotation = this.m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateRotation(this.ReportScopeInstance, this.m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_rotation.Value;
			}
		}

		public int Inclination
		{
			get
			{
				if (!this.m_inclination.HasValue)
				{
					if (this.m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						this.m_inclination = this.m_chartThreeDPropertiesDef.Inclination.Value;
					}
					else
					{
						this.m_inclination = this.m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateInclination(this.ReportScopeInstance, this.m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_inclination.Value;
			}
		}

		public int DepthRatio
		{
			get
			{
				if (!this.m_depthRatio.HasValue)
				{
					if (this.m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						this.m_depthRatio = this.m_chartThreeDPropertiesDef.DepthRatio.Value;
					}
					else
					{
						this.m_depthRatio = this.m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateDepthRatio(this.ReportScopeInstance, this.m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_depthRatio.Value;
			}
		}

		public ChartThreeDShadingTypes Shading
		{
			get
			{
				if (!this.m_shading.HasValue)
				{
					if (this.m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						this.m_shading = this.m_chartThreeDPropertiesDef.Shading.Value;
					}
					else
					{
						this.m_shading = this.m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateShading(this.ReportScopeInstance, this.m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_shading.Value;
			}
		}

		public int GapDepth
		{
			get
			{
				if (!this.m_gapDepth.HasValue)
				{
					if (this.m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						this.m_gapDepth = this.m_chartThreeDPropertiesDef.GapDepth.Value;
					}
					else
					{
						this.m_gapDepth = this.m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateGapDepth(this.ReportScopeInstance, this.m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_gapDepth.Value;
			}
		}

		public int WallThickness
		{
			get
			{
				if (!this.m_wallThickness.HasValue)
				{
					if (this.m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						this.m_wallThickness = this.m_chartThreeDPropertiesDef.WallThickness.Value;
					}
					else
					{
						this.m_wallThickness = this.m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateWallThickness(this.ReportScopeInstance, this.m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_wallThickness.Value;
			}
		}

		public bool Clustered
		{
			get
			{
				if (!this.m_clustered.HasValue)
				{
					if (this.m_chartThreeDPropertiesDef.ChartDef.IsOldSnapshot)
					{
						this.m_clustered = this.m_chartThreeDPropertiesDef.Clustered.Value;
					}
					else
					{
						this.m_clustered = this.m_chartThreeDPropertiesDef.ChartThreeDPropertiesDef.EvaluateClustered(this.ReportScopeInstance, this.m_chartThreeDPropertiesDef.ChartDef.RenderingContext.OdpContext);
					}
				}
				return this.m_clustered.Value;
			}
		}

		internal ChartThreeDPropertiesInstance(ChartThreeDProperties chartThreeDPropertiesDef)
			: base(chartThreeDPropertiesDef.ChartDef)
		{
			this.m_chartThreeDPropertiesDef = chartThreeDPropertiesDef;
		}

		protected override void ResetInstanceCache()
		{
			this.m_enabled = null;
			this.m_projectionMode = null;
			this.m_perspective = null;
			this.m_rotation = null;
			this.m_inclination = null;
			this.m_depthRatio = null;
			this.m_shading = null;
			this.m_gapDepth = null;
			this.m_wallThickness = null;
			this.m_clustered = null;
		}
	}
}
