using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class StateIndicatorInstance : GaugePanelItemInstance
	{
		private StateIndicator m_defObject;

		private GaugeStateIndicatorStyles? m_indicatorStyle;

		private double? m_scaleFactor;

		private GaugeResizeModes? m_resizeMode;

		private double? m_angle;

		private GaugeTransformationType? m_transformationType;

		public GaugeStateIndicatorStyles IndicatorStyle
		{
			get
			{
				if (!this.m_indicatorStyle.HasValue)
				{
					this.m_indicatorStyle = ((AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator)this.m_defObject.GaugePanelItemDef).EvaluateIndicatorStyle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_indicatorStyle.Value;
			}
		}

		public double ScaleFactor
		{
			get
			{
				if (!this.m_scaleFactor.HasValue)
				{
					this.m_scaleFactor = ((AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator)this.m_defObject.GaugePanelItemDef).EvaluateScaleFactor(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_scaleFactor.Value;
			}
		}

		public GaugeResizeModes ResizeMode
		{
			get
			{
				if (!this.m_resizeMode.HasValue)
				{
					this.m_resizeMode = ((AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator)this.m_defObject.GaugePanelItemDef).EvaluateResizeMode(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_resizeMode.Value;
			}
		}

		public double Angle
		{
			get
			{
				if (!this.m_angle.HasValue)
				{
					this.m_angle = ((AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator)this.m_defObject.GaugePanelItemDef).EvaluateAngle(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_angle.Value;
			}
		}

		public GaugeTransformationType TransformationType
		{
			get
			{
				if (!this.m_transformationType.HasValue)
				{
					this.m_transformationType = ((AspNetCore.ReportingServices.ReportIntermediateFormat.StateIndicator)this.m_defObject.GaugePanelItemDef).EvaluateTransformationType(this.ReportScopeInstance, this.m_defObject.GaugePanelDef.RenderingContext.OdpContext);
				}
				return this.m_transformationType.Value;
			}
		}

		internal StateIndicatorInstance(StateIndicator defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_indicatorStyle = null;
			this.m_scaleFactor = null;
			this.m_resizeMode = null;
			this.m_angle = null;
			this.m_transformationType = null;
		}
	}
}
