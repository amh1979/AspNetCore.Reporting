using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapTitleInstance : MapDockableSubItemInstance
	{
		private MapTitle m_defObject;

		private string m_text;

		private bool m_textEvaluated;

		private double? m_angle;

		private ReportSize m_textShadowOffset;

		public string Text
		{
			get
			{
				if (!this.m_textEvaluated)
				{
					this.m_text = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle)this.m_defObject.MapDockableSubItemDef).EvaluateText(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
					this.m_textEvaluated = true;
				}
				return this.m_text;
			}
		}

		public double Angle
		{
			get
			{
				if (!this.m_angle.HasValue)
				{
					this.m_angle = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle)this.m_defObject.MapDockableSubItemDef).EvaluateAngle(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_angle.Value;
			}
		}

		public ReportSize TextShadowOffset
		{
			get
			{
				if (this.m_textShadowOffset == null)
				{
					this.m_textShadowOffset = new ReportSize(((AspNetCore.ReportingServices.ReportIntermediateFormat.MapTitle)this.m_defObject.MapDockableSubItemDef).EvaluateTextShadowOffset(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_textShadowOffset;
			}
		}

		internal MapTitleInstance(MapTitle defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_text = null;
			this.m_textEvaluated = false;
			this.m_angle = null;
			this.m_textShadowOffset = null;
		}
	}
}
