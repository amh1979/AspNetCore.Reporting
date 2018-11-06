namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapBucketInstance : BaseInstance
	{
		private MapBucket m_defObject;

		private object m_startValue;

		private object m_endValue;

		public object StartValue
		{
			get
			{
				if (this.m_startValue == null)
				{
					this.m_startValue = this.m_defObject.MapBucketDef.EvaluateStartValue(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext).Value;
				}
				return this.m_startValue;
			}
		}

		public object EndValue
		{
			get
			{
				if (this.m_endValue == null)
				{
					this.m_endValue = this.m_defObject.MapBucketDef.EvaluateEndValue(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext).Value;
				}
				return this.m_endValue;
			}
		}

		internal MapBucketInstance(MapBucket defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_startValue = null;
			this.m_endValue = null;
		}
	}
}
