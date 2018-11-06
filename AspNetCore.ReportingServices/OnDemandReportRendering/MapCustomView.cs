using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapCustomView : MapView
	{
		private ReportDoubleProperty m_centerX;

		private ReportDoubleProperty m_centerY;

		public ReportDoubleProperty CenterX
		{
			get
			{
				if (this.m_centerX == null && this.MapCustomViewDef.CenterX != null)
				{
					this.m_centerX = new ReportDoubleProperty(this.MapCustomViewDef.CenterX);
				}
				return this.m_centerX;
			}
		}

		public ReportDoubleProperty CenterY
		{
			get
			{
				if (this.m_centerY == null && this.MapCustomViewDef.CenterY != null)
				{
					this.m_centerY = new ReportDoubleProperty(this.MapCustomViewDef.CenterY);
				}
				return this.m_centerY;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView MapCustomViewDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView)base.MapViewDef;
			}
		}

		public new MapCustomViewInstance Instance
		{
			get
			{
				return (MapCustomViewInstance)this.GetInstance();
			}
		}

		internal MapCustomView(AspNetCore.ReportingServices.ReportIntermediateFormat.MapCustomView defObject, Map map)
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
				base.m_instance = new MapCustomViewInstance(this);
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
