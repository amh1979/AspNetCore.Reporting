namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorScaleTitleInstance : BaseInstance
	{
		private MapColorScaleTitle m_defObject;

		private StyleInstance m_style;

		private string m_caption;

		private bool m_captionEvaluated;

		public StyleInstance Style
		{
			get
			{
				if (this.m_style == null)
				{
					this.m_style = new StyleInstance(this.m_defObject, this.m_defObject.MapDef.ReportScope, this.m_defObject.MapDef.RenderingContext);
				}
				return this.m_style;
			}
		}

		public string Caption
		{
			get
			{
				if (!this.m_captionEvaluated)
				{
					this.m_caption = this.m_defObject.MapColorScaleTitleDef.EvaluateCaption(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
					this.m_captionEvaluated = true;
				}
				return this.m_caption;
			}
		}

		internal MapColorScaleTitleInstance(MapColorScaleTitle defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_caption = null;
			this.m_captionEvaluated = false;
		}
	}
}
