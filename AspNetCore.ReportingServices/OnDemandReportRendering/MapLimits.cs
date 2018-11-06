using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLimits
	{
		private Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapLimits m_defObject;

		private MapLimitsInstance m_instance;

		private ReportDoubleProperty m_minimumX;

		private ReportDoubleProperty m_minimumY;

		private ReportDoubleProperty m_maximumX;

		private ReportDoubleProperty m_maximumY;

		private ReportBoolProperty m_limitToData;

		public ReportDoubleProperty MinimumX
		{
			get
			{
				if (this.m_minimumX == null && this.m_defObject.MinimumX != null)
				{
					this.m_minimumX = new ReportDoubleProperty(this.m_defObject.MinimumX);
				}
				return this.m_minimumX;
			}
		}

		public ReportDoubleProperty MinimumY
		{
			get
			{
				if (this.m_minimumY == null && this.m_defObject.MinimumY != null)
				{
					this.m_minimumY = new ReportDoubleProperty(this.m_defObject.MinimumY);
				}
				return this.m_minimumY;
			}
		}

		public ReportDoubleProperty MaximumX
		{
			get
			{
				if (this.m_maximumX == null && this.m_defObject.MaximumX != null)
				{
					this.m_maximumX = new ReportDoubleProperty(this.m_defObject.MaximumX);
				}
				return this.m_maximumX;
			}
		}

		public ReportDoubleProperty MaximumY
		{
			get
			{
				if (this.m_maximumY == null && this.m_defObject.MaximumY != null)
				{
					this.m_maximumY = new ReportDoubleProperty(this.m_defObject.MaximumY);
				}
				return this.m_maximumY;
			}
		}

		public ReportBoolProperty LimitToData
		{
			get
			{
				if (this.m_limitToData == null && this.m_defObject.LimitToData != null)
				{
					this.m_limitToData = new ReportBoolProperty(this.m_defObject.LimitToData);
				}
				return this.m_limitToData;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapLimits MapLimitsDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapLimitsInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					this.m_instance = new MapLimitsInstance(this);
				}
				return this.m_instance;
			}
		}

		internal MapLimits(AspNetCore.ReportingServices.ReportIntermediateFormat.MapLimits defObject, Map map)
		{
			this.m_defObject = defObject;
			this.m_map = map;
		}

		internal void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
