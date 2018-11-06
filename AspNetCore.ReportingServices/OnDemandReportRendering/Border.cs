using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Border
	{
		internal enum Position
		{
			Default,
			Top,
			Left,
			Right,
			Bottom
		}

		private Style m_owner;

		private Position m_position;

		private bool m_defaultSolidBorderStyle;

		private BorderInstance m_instance;

		public ReportColorProperty Color
		{
			get
			{
				return ((StyleBase)this.m_owner)[this.ColorAttrName] as ReportColorProperty;
			}
		}

		public ReportEnumProperty<BorderStyles> Style
		{
			get
			{
				return ((StyleBase)this.m_owner)[this.StyleAttrName] as ReportEnumProperty<BorderStyles>;
			}
		}

		public ReportSizeProperty Width
		{
			get
			{
				return ((StyleBase)this.m_owner)[this.WidthAttrName] as ReportSizeProperty;
			}
		}

		public BorderInstance Instance
		{
			get
			{
				this.GetInstance();
				ReportItem reportItem = this.Owner.ReportElement as ReportItem;
				if (reportItem != null)
				{
					reportItem.CriEvaluateInstance();
				}
				return this.m_instance;
			}
		}

		internal Style Owner
		{
			get
			{
				return this.m_owner;
			}
		}

		internal Position BorderPosition
		{
			get
			{
				return this.m_position;
			}
		}

		internal StyleAttributeNames ColorAttrName
		{
			get
			{
				switch (this.m_position)
				{
				case Position.Default:
					return StyleAttributeNames.BorderColor;
				case Position.Top:
					return StyleAttributeNames.BorderColorTop;
				case Position.Right:
					return StyleAttributeNames.BorderColorRight;
				case Position.Bottom:
					return StyleAttributeNames.BorderColorBottom;
				case Position.Left:
					return StyleAttributeNames.BorderColorLeft;
				default:
					Global.Tracer.Assert(false);
					return StyleAttributeNames.BorderColor;
				}
			}
		}

		internal StyleAttributeNames StyleAttrName
		{
			get
			{
				switch (this.m_position)
				{
				case Position.Default:
					return StyleAttributeNames.BorderStyle;
				case Position.Top:
					return StyleAttributeNames.BorderStyleTop;
				case Position.Right:
					return StyleAttributeNames.BorderStyleRight;
				case Position.Bottom:
					return StyleAttributeNames.BorderStyleBottom;
				case Position.Left:
					return StyleAttributeNames.BorderStyleLeft;
				default:
					Global.Tracer.Assert(false);
					return StyleAttributeNames.BorderColor;
				}
			}
		}

		internal StyleAttributeNames WidthAttrName
		{
			get
			{
				switch (this.m_position)
				{
				case Position.Default:
					return StyleAttributeNames.BorderWidth;
				case Position.Top:
					return StyleAttributeNames.BorderWidthTop;
				case Position.Right:
					return StyleAttributeNames.BorderWidthRight;
				case Position.Bottom:
					return StyleAttributeNames.BorderWidthBottom;
				case Position.Left:
					return StyleAttributeNames.BorderWidthLeft;
				default:
					Global.Tracer.Assert(false);
					return StyleAttributeNames.BorderColor;
				}
			}
		}

		internal Border(Style owner, Position position, bool defaultSolidBorderStyle)
		{
			this.m_owner = owner;
			this.m_position = position;
			this.m_defaultSolidBorderStyle = defaultSolidBorderStyle;
		}

		internal BorderInstance GetInstance()
		{
			if (this.m_owner.m_renderingContext.InstanceAccessDisallowed)
			{
				return null;
			}
			if (this.m_instance == null)
			{
				BorderStyles defaultStyleValueIfExpressionNull = (BorderStyles)((!this.m_defaultSolidBorderStyle) ? 1 : 4);
				if (this.m_owner.IsOldSnapshot)
				{
					this.m_instance = new BorderInstance(this, null, defaultStyleValueIfExpressionNull);
				}
				else
				{
					this.m_instance = new BorderInstance(this, this.m_owner.ReportScope, defaultStyleValueIfExpressionNull);
				}
			}
			return this.m_instance;
		}

		internal void ConstructBorderDefinition()
		{
			Global.Tracer.Assert(this.m_owner.ReportElement != null, "(m_owner.ReportElement != null)");
			Global.Tracer.Assert(this.m_owner.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition, "(m_owner.ReportElement.CriGenerationPhase == ReportElement.CriGenerationPhases.Definition)");
			Global.Tracer.Assert(this.m_owner.StyleContainer.StyleClass != null, "(m_owner.StyleContainer.StyleClass != null)");
			Global.Tracer.Assert(!this.Color.IsExpression, "(!this.Color.IsExpression)");
			if (this.Instance.IsColorAssigned)
			{
				string value = (this.Instance.Color != null) ? this.Instance.Color.ToString() : null;
				this.m_owner.StyleContainer.StyleClass.AddAttribute(this.m_owner.GetStyleStringFromEnum(this.ColorAttrName), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value));
			}
			else
			{
				this.m_owner.StyleContainer.StyleClass.AddAttribute(this.m_owner.GetStyleStringFromEnum(this.ColorAttrName), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(this.Style == null || !this.Style.IsExpression, "(this.Style == null || !this.Style.IsExpression)");
			if (this.Instance.IsStyleAssigned)
			{
				this.m_owner.StyleContainer.StyleClass.AddAttribute(this.m_owner.GetStyleStringFromEnum(this.StyleAttrName), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(this.Instance.Style.ToString()));
			}
			else
			{
				this.m_owner.StyleContainer.StyleClass.AddAttribute(this.m_owner.GetStyleStringFromEnum(this.StyleAttrName), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
			Global.Tracer.Assert(this.Width == null || !this.Width.IsExpression, "(this.Width == null || !this.Width.IsExpression)");
			if (this.Instance.IsWidthAssigned)
			{
				string value2 = (this.Instance.Width != null) ? this.Instance.Width.ToString() : null;
				this.m_owner.StyleContainer.StyleClass.AddAttribute(this.m_owner.GetStyleStringFromEnum(this.WidthAttrName), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(value2));
			}
			else
			{
				this.m_owner.StyleContainer.StyleClass.AddAttribute(this.m_owner.GetStyleStringFromEnum(this.WidthAttrName), AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression());
			}
		}
	}
}
