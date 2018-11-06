using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapMarkerRule : MapAppearanceRule
	{
		private MapMarkerCollection m_mapMarkers;

		public MapMarkerCollection MapMarkers
		{
			get
			{
				if (this.m_mapMarkers == null && this.MapMarkerRuleDef.MapMarkers != null)
				{
					this.m_mapMarkers = new MapMarkerCollection(this, base.m_map);
				}
				return this.m_mapMarkers;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerRule MapMarkerRuleDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerRule)base.MapAppearanceRuleDef;
			}
		}

		public new MapMarkerRuleInstance Instance
		{
			get
			{
				return (MapMarkerRuleInstance)this.GetInstance();
			}
		}

		internal MapMarkerRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapMarkerRule defObject, MapVectorLayer mapVectorLayer, Map map)
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
				base.m_instance = new MapMarkerRuleInstance(this);
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
			if (this.m_mapMarkers != null)
			{
				this.m_mapMarkers.SetNewContext();
			}
		}
	}
}
