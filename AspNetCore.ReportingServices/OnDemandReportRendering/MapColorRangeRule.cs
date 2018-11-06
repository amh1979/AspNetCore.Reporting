using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class MapColorRangeRule : MapColorRule
	{
		private ReportColorProperty m_startColor;

		private ReportColorProperty m_middleColor;

		private ReportColorProperty m_endColor;

		public ReportColorProperty StartColor
		{
			get
			{
				if (this.m_startColor == null && this.MapColorRangeRuleDef.StartColor != null)
				{
					ExpressionInfo startColor = this.MapColorRangeRuleDef.StartColor;
					if (startColor != null)
					{
						this.m_startColor = new ReportColorProperty(startColor.IsExpression, this.MapColorRangeRuleDef.StartColor.OriginalText, startColor.IsExpression ? null : new ReportColor(startColor.StringValue.Trim(), true), startColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_startColor;
			}
		}

		public ReportColorProperty MiddleColor
		{
			get
			{
				if (this.m_middleColor == null && this.MapColorRangeRuleDef.MiddleColor != null)
				{
					ExpressionInfo middleColor = this.MapColorRangeRuleDef.MiddleColor;
					if (middleColor != null)
					{
						this.m_middleColor = new ReportColorProperty(middleColor.IsExpression, this.MapColorRangeRuleDef.MiddleColor.OriginalText, middleColor.IsExpression ? null : new ReportColor(middleColor.StringValue.Trim(), true), middleColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_middleColor;
			}
		}

		public ReportColorProperty EndColor
		{
			get
			{
				if (this.m_endColor == null && this.MapColorRangeRuleDef.EndColor != null)
				{
					ExpressionInfo endColor = this.MapColorRangeRuleDef.EndColor;
					if (endColor != null)
					{
						this.m_endColor = new ReportColorProperty(endColor.IsExpression, this.MapColorRangeRuleDef.EndColor.OriginalText, endColor.IsExpression ? null : new ReportColor(endColor.StringValue.Trim(), true), endColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_endColor;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule MapColorRangeRuleDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule)base.MapAppearanceRuleDef;
			}
		}

		public new MapColorRangeRuleInstance Instance
		{
			get
			{
				return (MapColorRangeRuleInstance)this.GetInstance();
			}
		}

		internal MapColorRangeRule(AspNetCore.ReportingServices.ReportIntermediateFormat.MapColorRangeRule defObject, MapVectorLayer mapVectorLayer, Map map)
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
				base.m_instance = new MapColorRangeRuleInstance(this);
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
