namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapLimitsInstance : BaseInstance
	{
		private MapLimits m_defObject;

		private double? m_minimumX;

		private double? m_minimumY;

		private double? m_maximumX;

		private double? m_maximumY;

		private bool? m_limitToData;

		public double MinimumX
		{
			get
			{
				if (!this.m_minimumX.HasValue)
				{
					this.m_minimumX = this.m_defObject.MapLimitsDef.EvaluateMinimumX(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_minimumX.Value;
			}
		}

		public double MinimumY
		{
			get
			{
				if (!this.m_minimumY.HasValue)
				{
					this.m_minimumY = this.m_defObject.MapLimitsDef.EvaluateMinimumY(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_minimumY.Value;
			}
		}

		public double MaximumX
		{
			get
			{
				if (!this.m_maximumX.HasValue)
				{
					this.m_maximumX = this.m_defObject.MapLimitsDef.EvaluateMaximumX(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_maximumX.Value;
			}
		}

		public double MaximumY
		{
			get
			{
				if (!this.m_maximumY.HasValue)
				{
					this.m_maximumY = this.m_defObject.MapLimitsDef.EvaluateMaximumY(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_maximumY.Value;
			}
		}

		public bool LimitToData
		{
			get
			{
				if (!this.m_limitToData.HasValue)
				{
					this.m_limitToData = this.m_defObject.MapLimitsDef.EvaluateLimitToData(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_limitToData.Value;
			}
		}

		internal MapLimitsInstance(MapLimits defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_minimumX = null;
			this.m_minimumY = null;
			this.m_maximumX = null;
			this.m_maximumY = null;
			this.m_limitToData = null;
		}
	}
}
