namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapSubItemInstance : BaseInstance
	{
		private MapSubItem m_defObject;

		private StyleInstance m_style;

		private ReportSize m_leftMargin;

		private ReportSize m_rightMargin;

		private ReportSize m_topMargin;

		private ReportSize m_bottomMargin;

		private int? m_zIndex;

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

		public ReportSize LeftMargin
		{
			get
			{
				if (this.m_leftMargin == null)
				{
					this.m_leftMargin = new ReportSize(this.m_defObject.MapSubItemDef.EvaluateLeftMargin(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_leftMargin;
			}
		}

		public ReportSize RightMargin
		{
			get
			{
				if (this.m_rightMargin == null)
				{
					this.m_rightMargin = new ReportSize(this.m_defObject.MapSubItemDef.EvaluateRightMargin(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_rightMargin;
			}
		}

		public ReportSize TopMargin
		{
			get
			{
				if (this.m_topMargin == null)
				{
					this.m_topMargin = new ReportSize(this.m_defObject.MapSubItemDef.EvaluateTopMargin(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_topMargin;
			}
		}

		public ReportSize BottomMargin
		{
			get
			{
				if (this.m_bottomMargin == null)
				{
					this.m_bottomMargin = new ReportSize(this.m_defObject.MapSubItemDef.EvaluateBottomMargin(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext));
				}
				return this.m_bottomMargin;
			}
		}

		public int ZIndex
		{
			get
			{
				if (!this.m_zIndex.HasValue)
				{
					this.m_zIndex = this.m_defObject.MapSubItemDef.EvaluateZIndex(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_zIndex.Value;
			}
		}

		internal MapSubItemInstance(MapSubItem defObject)
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
			this.m_leftMargin = null;
			this.m_rightMargin = null;
			this.m_topMargin = null;
			this.m_bottomMargin = null;
			this.m_zIndex = null;
		}
	}
}
