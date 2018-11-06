using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapSizeRule : MapAppearanceRule
	{
		private ReportSizeProperty m_startSize;

		private ReportSizeProperty m_endSize;

		public ReportSizeProperty StartSize
		{
			get
			{
				if (this.m_startSize == null && this.MapSizeRuleDef.StartSize != null)
				{
					this.m_startSize = new ReportSizeProperty(this.MapSizeRuleDef.StartSize);
				}
				return this.m_startSize;
			}
		}

		public ReportSizeProperty EndSize
		{
			get
			{
				if (this.m_endSize == null && this.MapSizeRuleDef.EndSize != null)
				{
					this.m_endSize = new ReportSizeProperty(this.MapSizeRuleDef.EndSize);
				}
				return this.m_endSize;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapSizeRule MapSizeRuleDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapSizeRule)base.MapAppearanceRuleDef;
			}
		}

		public new MapSizeRuleInstance Instance
		{
			get
			{
				return (MapSizeRuleInstance)this.GetInstance();
			}
		}

		internal MapSizeRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapSizeRule defObject, MapVectorLayer mapVectorLayer, Map map)
			: base(defObject, mapVectorLayer, map)
		{
		}

		internal override MapAppearanceRuleInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapSizeRuleInstance(this);
			}
			return base.m_instance;
		}

		internal override void SetNewContext()
		{
			base.SetNewContext();
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}
	}
}
