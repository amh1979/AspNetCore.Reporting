namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class MapAppearanceRuleInstance : BaseInstance
	{
		private MapAppearanceRule m_defObject;

		private object m_dataValue;

		private MapRuleDistributionType? m_distributionType;

		private int? m_bucketCount;

		private object m_startValue;

		private object m_endValue;

		private string m_legendText;

		private bool m_legendTextEvaluated;

		public object DataValue
		{
			get
			{
				if (this.m_dataValue == null)
				{
					this.m_dataValue = this.m_defObject.MapAppearanceRuleDef.EvaluateDataValue(this.m_defObject.ReportScope.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext).Value;
				}
				return this.m_dataValue;
			}
		}

		public MapRuleDistributionType DistributionType
		{
			get
			{
				if (!this.m_distributionType.HasValue)
				{
					this.m_distributionType = this.m_defObject.MapAppearanceRuleDef.EvaluateDistributionType(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_distributionType.Value;
			}
		}

		public int BucketCount
		{
			get
			{
				if (!this.m_bucketCount.HasValue)
				{
					this.m_bucketCount = this.m_defObject.MapAppearanceRuleDef.EvaluateBucketCount(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
				}
				return this.m_bucketCount.Value;
			}
		}

		public object StartValue
		{
			get
			{
				if (this.m_startValue == null)
				{
					this.m_startValue = this.m_defObject.MapAppearanceRuleDef.EvaluateStartValue(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext).Value;
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
					this.m_endValue = this.m_defObject.MapAppearanceRuleDef.EvaluateEndValue(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext).Value;
				}
				return this.m_endValue;
			}
		}

		public string LegendText
		{
			get
			{
				if (!this.m_legendTextEvaluated)
				{
					this.m_legendText = this.m_defObject.MapAppearanceRuleDef.EvaluateLegendText(this.ReportScopeInstance, this.m_defObject.MapDef.RenderingContext.OdpContext);
					this.m_legendTextEvaluated = true;
				}
				return this.m_legendText;
			}
		}

		internal MapAppearanceRuleInstance(MapAppearanceRule defObject)
			: base(defObject.MapDef.ReportScope)
		{
			this.m_defObject = defObject;
		}

		protected override void ResetInstanceCache()
		{
			this.m_dataValue = null;
			this.m_distributionType = null;
			this.m_bucketCount = null;
			this.m_startValue = null;
			this.m_endValue = null;
			this.m_legendText = null;
			this.m_legendTextEvaluated = false;
		}
	}
}
