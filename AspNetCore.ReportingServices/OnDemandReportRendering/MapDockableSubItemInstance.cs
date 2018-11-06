using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapDockableSubItemInstance : MapSubItemInstance
	{
		private MapDockableSubItem m_defObject;

		private MapPosition? m_position;

		private bool? m_dockOutsideViewport;

		private bool? m_hidden;

		private string m_toolTip;

		private bool m_toolTipEvaluated;

		public MapPosition Position
		{
			get
			{
				if (!this.m_position.HasValue)
				{
					this.m_position = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapDockableSubItem)this.m_defObject.MapSubItemDef).EvaluatePosition(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_position.Value;
			}
		}

		public bool DockOutsideViewport
		{
			get
			{
				if (!this.m_dockOutsideViewport.HasValue)
				{
					this.m_dockOutsideViewport = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapDockableSubItem)this.m_defObject.MapSubItemDef).EvaluateDockOutsideViewport(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_dockOutsideViewport.Value;
			}
		}

		public bool Hidden
		{
			get
			{
				if (!this.m_hidden.HasValue)
				{
					this.m_hidden = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapDockableSubItem)this.m_defObject.MapSubItemDef).EvaluateHidden(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_hidden.Value;
			}
		}

		public string ToolTip
		{
			get
			{
				if (!this.m_toolTipEvaluated)
				{
					this.m_toolTip = ((AspNetCore.ReportingServices.ReportIntermediateFormat.MapDockableSubItem)this.m_defObject.MapSubItemDef).EvaluateToolTip(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
					this.m_toolTipEvaluated = true;
				}
				return this.m_toolTip;
			}
		}

		internal MapDockableSubItemInstance(MapDockableSubItem defObject)
			: base(defObject)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_position = null;
			this.m_dockOutsideViewport = null;
			this.m_hidden = null;
			this.m_toolTip = null;
			this.m_toolTipEvaluated = false;
		}
	}
}
