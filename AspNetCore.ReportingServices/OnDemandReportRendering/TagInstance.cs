using AspNetCore.ReportingServices.RdlExpressions;
using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class TagInstance
	{
		private readonly Tag m_tagDef;

		private VariantResult? m_tag;

		public TypeCode DataType
		{
			get
			{
				this.EvaluateTagValue();
				return this.m_tag.Value.TypeCode;
			}
		}

		public object Value
		{
			get
			{
				this.EvaluateTagValue();
				return this.m_tag.Value.Value;
			}
		}

		internal TagInstance(Tag tagDef)
		{
			this.m_tagDef = tagDef;
		}

		private void EvaluateTagValue()
		{
			if (!this.m_tag.HasValue)
			{
				ExpressionInfo expression = this.m_tagDef.Expression;
				if (expression != null)
				{
					if (expression.IsExpression)
					{
						Image image = this.m_tagDef.Image;
						this.m_tag = image.ImageDef.EvaluateTagExpression(expression, image.Instance.ReportScopeInstance, image.RenderingContext.OdpContext);
					}
					else
					{
						VariantResult value = new VariantResult(false, expression.Value);
						ReportRuntime.SetVariantType(ref value);
						this.m_tag = value;
					}
				}
				else
				{
					this.m_tag = new VariantResult(false, null);
				}
			}
		}

		internal void ResetInstanceCache()
		{
			this.m_tag = null;
		}
	}
}
