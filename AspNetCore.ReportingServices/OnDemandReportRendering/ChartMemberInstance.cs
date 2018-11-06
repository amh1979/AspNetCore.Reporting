using AspNetCore.ReportingServices.RdlExpressions;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal class ChartMemberInstance : BaseInstance
	{
		protected Chart m_owner;

		protected ChartMember m_memberDef;

		protected bool m_labelEvaluated;

		protected string m_label;

		protected StyleInstance m_style;

		private VariantResult? m_labelObject = null;

		public object LabelObject
		{
			get
			{
				return this.GetLabelObject().Value;
			}
		}

		internal TypeCode LabelTypeCode
		{
			get
			{
				return this.GetLabelObject().TypeCode;
			}
		}

		public string Label
		{
			get
			{
				if (!this.m_labelEvaluated)
				{
					this.m_labelEvaluated = true;
					if (this.m_owner.IsOldSnapshot)
					{
						object value = this.GetLabelObject().Value;
						if (value != null)
						{
							this.m_label = value.ToString();
						}
					}
					else
					{
						this.m_label = this.m_memberDef.MemberDefinition.GetFormattedLabelValue(this.GetLabelObject(), this.m_owner.RenderingContext.OdpContext);
					}
				}
				return this.m_label;
			}
		}

		internal ChartMemberInstance(Chart owner, ChartMember memberDef)
			: base(memberDef.ReportScope)
		{
			this.m_owner = owner;
			this.m_memberDef = memberDef;
		}

		private VariantResult GetLabelObject()
		{
			if (!this.m_labelObject.HasValue)
			{
				if (this.m_owner.IsOldSnapshot)
				{
					this.m_labelObject = new VariantResult(false, ((ShimChartMember)this.m_memberDef).LabelInstanceValue);
				}
				else
				{
					this.m_labelObject = this.m_memberDef.MemberDefinition.EvaluateLabel(this, this.m_owner.RenderingContext.OdpContext);
				}
			}
			return this.m_labelObject.Value;
		}

		protected override void ResetInstanceCache()
		{
			if (this.m_style != null)
			{
				this.m_style.SetNewContext();
			}
			this.m_labelEvaluated = false;
			this.m_labelObject = null;
		}
	}
}
