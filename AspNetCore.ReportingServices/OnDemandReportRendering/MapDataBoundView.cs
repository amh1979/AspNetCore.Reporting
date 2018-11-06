using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapDataBoundView : MapView
	{
		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataBoundView MapDataBoundViewDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataBoundView)base.MapViewDef;
			}
		}

		public new MapDataBoundViewInstance Instance
		{
			get
			{
				return (MapDataBoundViewInstance)this.GetInstance();
			}
		}

		internal MapDataBoundView(AspNetCore.ReportingServices.ReportIntermediateFormat.MapDataBoundView defObject, Map map)
			: base(defObject, map)
		{
		}

		internal override MapViewInstance GetInstance()
		{
			if (base.m_map.RenderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (base.m_instance == null)
			{
				base.m_instance = new MapDataBoundViewInstance(this);
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
