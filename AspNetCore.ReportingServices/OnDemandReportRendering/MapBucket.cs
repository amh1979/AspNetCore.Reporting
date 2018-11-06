using AspNetCore.ReportingServices.ReportIntermediateFormat;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBucket : MapObjectCollectionItem
	{
		private Map m_map;

		private AspNetCore.ReportingServices.ReportIntermediateFormat.MapBucket m_defObject;

		private ReportVariantProperty m_startValue;

		private ReportVariantProperty m_endValue;

		public ReportVariantProperty StartValue
		{
			get
			{
				if (this.m_startValue == null && this.m_defObject.StartValue != null)
				{
					this.m_startValue = new ReportVariantProperty(this.m_defObject.StartValue);
				}
				return this.m_startValue;
			}
		}

		public ReportVariantProperty EndValue
		{
			get
			{
				if (this.m_endValue == null && this.m_defObject.EndValue != null)
				{
					this.m_endValue = new ReportVariantProperty(this.m_defObject.EndValue);
				}
				return this.m_endValue;
			}
		}

		internal Map MapDef
		{
			get
			{
				return this.m_map;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapBucket MapBucketDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		public MapBucketInstance Instance
		{
			get
			{
				if (this.m_map.RenderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (base.m_instance == null)
				{
					base.m_instance = new MapBucketInstance(this);
				}
				return (MapBucketInstance)base.m_instance;
			}
		}

		internal MapBucket(AspNetCore.ReportingServices.ReportIntermediateFormat.MapBucket defObject, Map map)
		{
			this.m_defObject = defObject;
			this.m_map = map;
		}

		internal override void SetNewContext()
		{
			if (base.m_instance != null)
			{
				base.m_instance.SetNewContext();
			}
		}
	}
}
