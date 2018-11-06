using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class BorderInstance : BaseInstance
	{
		internal enum BorderStyleProperty
		{
			Color,
			Style,
			Width
		}

		private Border m_owner;

		private BorderStyles m_defaultStyleValueIfExpressionNull;

		private ReportColor m_color;

		private BorderStyles m_style;

		private ReportSize m_width;

		private bool m_colorEvaluated;

		private bool m_colorAssigned;

		private bool m_styleEvaluated;

		private bool m_styleAssigned;

		private bool m_widthEvaluated;

		private bool m_widthAssigned;

		public ReportColor Color
		{
			get
			{
				if (!this.m_colorEvaluated)
				{
					this.m_colorEvaluated = true;
					this.m_color = this.m_owner.Owner.EvaluateInstanceReportColor(this.m_owner.ColorAttrName);
				}
				return this.m_color;
			}
			set
			{
				if (this.m_owner.Owner.ReportElement != null && this.m_owner.Owner.ReportElement.CriGenerationPhase != 0 && (this.m_owner.Owner.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_owner.Color.IsExpression))
				{
					this.m_colorEvaluated = true;
					this.m_colorAssigned = true;
					this.m_color = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsColorAssigned
		{
			get
			{
				return this.m_colorAssigned;
			}
		}

		public BorderStyles Style
		{
			get
			{
				if (!this.m_styleEvaluated)
				{
					this.m_styleEvaluated = true;
					this.m_style = (BorderStyles)this.m_owner.Owner.EvaluateInstanceStyleEnum(this.m_owner.StyleAttrName, (int)this.m_defaultStyleValueIfExpressionNull);
					if (this.m_style == BorderStyles.Default && this.m_owner.BorderPosition == Border.Position.Default)
					{
						this.m_style = this.m_defaultStyleValueIfExpressionNull;
					}
				}
				return this.m_style;
			}
			set
			{
				if (this.m_owner.Owner.ReportElement != null && this.m_owner.Owner.ReportElement.CriGenerationPhase != 0 && (this.m_owner.Owner.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_owner.Style.IsExpression))
				{
					string text = value.ToString();
					string text2 = default(string);
					if (!AspNetCore.ReportingServices.ReportPublishing.Validator.ValidateBorderStyle(text, this.m_owner.BorderPosition == Border.Position.Default, this.m_owner.Owner.StyleContainer.ObjectType, AspNetCore.ReportingServices.ReportPublishing.Validator.IsDynamicImageSubElement(this.m_owner.Owner.StyleContainer), out text2))
					{
						throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
					}
					this.m_styleEvaluated = true;
					this.m_styleAssigned = true;
					if (text2 != text)
					{
						this.m_style = StyleTranslator.TranslateBorderStyle(text2, null);
					}
					else
					{
						this.m_style = value;
					}
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsStyleAssigned
		{
			get
			{
				return this.m_styleAssigned;
			}
		}

		public ReportSize Width
		{
			get
			{
				if (!this.m_widthEvaluated)
				{
					this.m_widthEvaluated = true;
					this.m_width = this.m_owner.Owner.EvaluateInstanceReportSize(this.m_owner.WidthAttrName);
				}
				return this.m_width;
			}
			set
			{
				if (this.m_owner.Owner.ReportElement != null && this.m_owner.Owner.ReportElement.CriGenerationPhase != 0 && (this.m_owner.Owner.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_owner.Width.IsExpression))
				{
					this.m_widthEvaluated = true;
					this.m_widthAssigned = true;
					this.m_width = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsWidthAssigned
		{
			get
			{
				return this.m_widthAssigned;
			}
		}

		internal BorderInstance(Border owner, IReportScope reportScope, BorderStyles defaultStyleValueIfExpressionNull)
			: base(reportScope)
		{
			this.m_owner = owner;
			this.m_defaultStyleValueIfExpressionNull = defaultStyleValueIfExpressionNull;
		}

		protected override void ResetInstanceCache()
		{
			this.m_colorEvaluated = false;
			this.m_colorAssigned = false;
			this.m_color = null;
			this.m_styleEvaluated = false;
			this.m_styleAssigned = false;
			this.m_widthEvaluated = false;
			this.m_widthAssigned = false;
			this.m_width = null;
		}

		internal void GetAssignedDynamicValues(List<int> styles, List<AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo> values)
		{
			if (this.m_colorAssigned && this.m_owner.Color.IsExpression)
			{
				styles.Add((int)this.m_owner.ColorAttrName);
				values.Add(StyleInstance.CreateAttrInfo(this.m_color));
			}
			if (this.m_styleAssigned && this.m_owner.Style.IsExpression)
			{
				styles.Add((int)this.m_owner.StyleAttrName);
				values.Add(StyleInstance.CreateAttrInfo((int)this.m_style));
			}
			if (this.m_widthAssigned && this.m_owner.Width.IsExpression)
			{
				styles.Add((int)this.m_owner.WidthAttrName);
				values.Add(StyleInstance.CreateAttrInfo(this.m_width));
			}
		}

		internal void SetAssignedDynamicValue(BorderStyleProperty prop, AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo value, bool allowTransparency)
		{
			switch (prop)
			{
			case BorderStyleProperty.Color:
				this.m_colorEvaluated = true;
				this.m_color = new ReportColor(value.Value, allowTransparency);
				break;
			case BorderStyleProperty.Style:
				this.m_styleEvaluated = true;
				this.m_style = (BorderStyles)value.IntValue;
				break;
			case BorderStyleProperty.Width:
				this.m_widthEvaluated = true;
				this.m_width = new ReportSize(value.Value);
				break;
			}
		}
	}
}
