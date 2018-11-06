using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	[SkipStaticValidation]
	internal class StyleInstance : StyleBaseInstance, IPersistable
	{
		private Style m_styleDefinition;

		protected ReportColor m_backgroundColor;

		protected ReportColor m_backgroundGradientEndColor;

		protected ReportColor m_color;

		protected FontStyles? m_fontStyle;

		protected string m_fontFamily;

		protected FontWeights? m_fontWeight;

		protected string m_format;

		protected TextDecorations? m_textDecoration;

		protected TextAlignments? m_textAlign;

		protected VerticalAlignments? m_verticalAlign;

		protected Directions? m_direction;

		protected WritingModes? m_writingMode;

		protected string m_language;

		protected UnicodeBiDiTypes? m_unicodeBiDi;

		protected Calendars? m_calendar;

		protected string m_currencyLanguage;

		protected string m_numeralLanguage;

		protected BackgroundGradients? m_backgroundGradientType;

		protected ReportSize m_fontSize;

		protected ReportSize m_paddingLeft;

		protected ReportSize m_paddingRight;

		protected ReportSize m_paddingTop;

		protected ReportSize m_paddingBottom;

		protected ReportSize m_lineHeight;

		protected int m_numeralVariant;

		protected TextEffects? m_textEffect;

		protected BackgroundHatchTypes? m_backgroundHatchType;

		protected ReportColor m_shadowColor;

		protected ReportSize m_shadowOffset;

		protected Dictionary<StyleAttributeNames, bool> m_assignedValues;

		private static readonly Declaration m_Declaration = StyleInstance.GetDeclaration();

		public override List<StyleAttributeNames> StyleAttributes
		{
			get
			{
				return this.m_styleDefinition.NonSharedStyleAttributes;
			}
		}

		public override object this[StyleAttributeNames style]
		{
			get
			{
				Border border = null;
				switch (style)
				{
				case StyleAttributeNames.BorderColor:
					return this.m_styleDefinition.Border.Instance.Color;
				case StyleAttributeNames.BorderColorTop:
					border = this.m_styleDefinition.TopBorder;
					if (border != null)
					{
						return border.Instance.Color;
					}
					return null;
				case StyleAttributeNames.BorderColorLeft:
					border = this.m_styleDefinition.LeftBorder;
					if (border != null)
					{
						return border.Instance.Color;
					}
					return null;
				case StyleAttributeNames.BorderColorRight:
					border = this.m_styleDefinition.RightBorder;
					if (border != null)
					{
						return border.Instance.Color;
					}
					return null;
				case StyleAttributeNames.BorderColorBottom:
					border = this.m_styleDefinition.BottomBorder;
					if (border != null)
					{
						return border.Instance.Color;
					}
					return null;
				case StyleAttributeNames.BorderStyle:
					return this.m_styleDefinition.Border.Instance.Style;
				case StyleAttributeNames.BorderStyleTop:
					border = this.m_styleDefinition.TopBorder;
					if (border != null)
					{
						return border.Instance.Style;
					}
					return null;
				case StyleAttributeNames.BorderStyleLeft:
					border = this.m_styleDefinition.LeftBorder;
					if (border != null)
					{
						return border.Instance.Style;
					}
					return null;
				case StyleAttributeNames.BorderStyleRight:
					border = this.m_styleDefinition.RightBorder;
					if (border != null)
					{
						return border.Instance.Style;
					}
					return null;
				case StyleAttributeNames.BorderStyleBottom:
					border = this.m_styleDefinition.BottomBorder;
					if (border != null)
					{
						return border.Instance.Style;
					}
					return null;
				case StyleAttributeNames.BorderWidth:
					return this.m_styleDefinition.Border.Instance.Width;
				case StyleAttributeNames.BorderWidthTop:
					border = this.m_styleDefinition.TopBorder;
					if (border != null)
					{
						return border.Instance.Width;
					}
					return null;
				case StyleAttributeNames.BorderWidthLeft:
					border = this.m_styleDefinition.LeftBorder;
					if (border != null)
					{
						return border.Instance.Width;
					}
					return null;
				case StyleAttributeNames.BorderWidthRight:
					border = this.m_styleDefinition.RightBorder;
					if (border != null)
					{
						return border.Instance.Width;
					}
					return null;
				case StyleAttributeNames.BorderWidthBottom:
					border = this.m_styleDefinition.BottomBorder;
					if (border != null)
					{
						return border.Instance.Width;
					}
					return null;
				case StyleAttributeNames.BackgroundColor:
					return this.BackgroundColor;
				case StyleAttributeNames.BackgroundGradientEndColor:
					return this.BackgroundGradientEndColor;
				case StyleAttributeNames.BackgroundGradientType:
					return this.BackgroundGradientType;
				case StyleAttributeNames.Calendar:
					return this.Calendar;
				case StyleAttributeNames.Color:
					return this.Color;
				case StyleAttributeNames.CurrencyLanguage:
					return this.CurrencyLanguage;
				case StyleAttributeNames.Direction:
					return this.Direction;
				case StyleAttributeNames.FontFamily:
					return this.FontFamily;
				case StyleAttributeNames.FontSize:
					return this.FontSize;
				case StyleAttributeNames.FontStyle:
					return this.FontStyle;
				case StyleAttributeNames.FontWeight:
					return this.FontWeight;
				case StyleAttributeNames.Format:
					return this.Format;
				case StyleAttributeNames.Language:
					return this.Language;
				case StyleAttributeNames.LineHeight:
					return this.LineHeight;
				case StyleAttributeNames.NumeralLanguage:
					return this.NumeralLanguage;
				case StyleAttributeNames.NumeralVariant:
					return this.NumeralVariant;
				case StyleAttributeNames.PaddingBottom:
					return this.PaddingBottom;
				case StyleAttributeNames.PaddingLeft:
					return this.PaddingLeft;
				case StyleAttributeNames.PaddingRight:
					return this.PaddingRight;
				case StyleAttributeNames.PaddingTop:
					return this.PaddingTop;
				case StyleAttributeNames.TextAlign:
					return this.TextAlign;
				case StyleAttributeNames.TextDecoration:
					return this.TextDecoration;
				case StyleAttributeNames.UnicodeBiDi:
					return this.UnicodeBiDi;
				case StyleAttributeNames.VerticalAlign:
					return this.VerticalAlign;
				case StyleAttributeNames.WritingMode:
					return this.WritingMode;
				case StyleAttributeNames.TextEffect:
					return this.TextEffect;
				case StyleAttributeNames.BackgroundHatchType:
					return this.BackgroundHatchType;
				case StyleAttributeNames.ShadowColor:
					return this.ShadowColor;
				case StyleAttributeNames.ShadowOffset:
					return this.ShadowOffset;
				default:
					return null;
				}
			}
		}

		public override ReportColor BackgroundGradientEndColor
		{
			get
			{
				if (this.m_backgroundGradientEndColor == null)
				{
					this.m_backgroundGradientEndColor = this.m_styleDefinition.EvaluateInstanceReportColor(StyleAttributeNames.BackgroundGradientEndColor);
				}
				return this.m_backgroundGradientEndColor;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.BackgroundGradientEndColor.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.BackgroundGradientEndColor);
					this.m_backgroundGradientEndColor = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsBackgroundGradientEndColorAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.BackgroundGradientEndColor);
			}
		}

		public override ReportColor Color
		{
			get
			{
				if (this.m_color == null)
				{
					this.m_color = this.m_styleDefinition.EvaluateInstanceReportColor(StyleAttributeNames.Color);
				}
				return this.m_color;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.Color.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.Color);
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
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.Color);
			}
		}

		public override ReportColor BackgroundColor
		{
			get
			{
				if (this.m_backgroundColor == null)
				{
					this.m_backgroundColor = this.m_styleDefinition.EvaluateInstanceReportColor(StyleAttributeNames.BackgroundColor);
				}
				return this.m_backgroundColor;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.BackgroundColor.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.BackgroundColor);
					this.m_backgroundColor = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsBackgroundColorAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.BackgroundColor);
			}
		}

		public override FontStyles FontStyle
		{
			get
			{
				if (!this.m_fontStyle.HasValue)
				{
					this.m_fontStyle = (FontStyles)this.m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.FontStyle);
				}
				return this.m_fontStyle.Value;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.FontStyle.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.FontStyle);
					this.m_fontStyle = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsFontStyleAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.FontStyle);
			}
		}

		public override string FontFamily
		{
			get
			{
				if (this.m_fontFamily == null)
				{
					this.m_fontFamily = this.m_styleDefinition.EvaluateInstanceStyleString(StyleAttributeNames.FontFamily);
				}
				return this.m_fontFamily;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.FontFamily.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.FontFamily);
					this.m_fontFamily = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsFontFamilyAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.FontFamily);
			}
		}

		public override FontWeights FontWeight
		{
			get
			{
				if (!this.m_fontWeight.HasValue)
				{
					this.m_fontWeight = (FontWeights)this.m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.FontWeight);
				}
				return this.m_fontWeight.Value;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.FontWeight.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.FontWeight);
					this.m_fontWeight = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsFontWeightAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.FontWeight);
			}
		}

		public override string Format
		{
			get
			{
				if (this.m_format == null)
				{
					this.m_format = this.m_styleDefinition.EvaluateInstanceStyleString(StyleAttributeNames.Format);
				}
				return this.m_format;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.Format.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.Format);
					this.m_format = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsFormatAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.Format);
			}
		}

		public override TextDecorations TextDecoration
		{
			get
			{
				if (!this.m_textDecoration.HasValue)
				{
					this.m_textDecoration = (TextDecorations)this.m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.TextDecoration);
				}
				return this.m_textDecoration.Value;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.TextDecoration.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.TextDecoration);
					this.m_textDecoration = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsTextDecorationAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.TextDecoration);
			}
		}

		public override TextAlignments TextAlign
		{
			get
			{
				if (!this.m_textAlign.HasValue)
				{
					this.m_textAlign = (TextAlignments)this.m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.TextAlign);
				}
				return this.m_textAlign.Value;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.TextAlign.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.TextAlign);
					this.m_textAlign = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsTextAlignAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.TextAlign);
			}
		}

		public override VerticalAlignments VerticalAlign
		{
			get
			{
				if (!this.m_verticalAlign.HasValue)
				{
					this.m_verticalAlign = (VerticalAlignments)this.m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.VerticalAlign);
				}
				return this.m_verticalAlign.Value;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.VerticalAlign.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.VerticalAlign);
					this.m_verticalAlign = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsVerticalAlignAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.VerticalAlign);
			}
		}

		public override Directions Direction
		{
			get
			{
				if (!this.m_direction.HasValue)
				{
					this.m_direction = (Directions)this.m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.Direction);
				}
				return this.m_direction.Value;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.Direction.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.Direction);
					this.m_direction = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsDirectionAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.Direction);
			}
		}

		public override WritingModes WritingMode
		{
			get
			{
				if (!this.m_writingMode.HasValue)
				{
					this.m_writingMode = (WritingModes)this.m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.WritingMode);
				}
				return this.m_writingMode.Value;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.WritingMode.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.WritingMode);
					this.m_writingMode = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsWritingModeAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.WritingMode);
			}
		}

		public override string Language
		{
			get
			{
				if (this.m_language == null)
				{
					this.m_language = this.m_styleDefinition.EvaluateInstanceStyleString(StyleAttributeNames.Language);
				}
				return this.m_language;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.Language.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.Language);
					this.m_language = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsLanguageAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.Language);
			}
		}

		public override UnicodeBiDiTypes UnicodeBiDi
		{
			get
			{
				if (!this.m_unicodeBiDi.HasValue)
				{
					this.m_unicodeBiDi = (UnicodeBiDiTypes)this.m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.UnicodeBiDi);
				}
				return this.m_unicodeBiDi.Value;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.UnicodeBiDi.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.UnicodeBiDi);
					this.m_unicodeBiDi = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsUnicodeBiDiAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.UnicodeBiDi);
			}
		}

		public override Calendars Calendar
		{
			get
			{
				if (!this.m_calendar.HasValue)
				{
					this.m_calendar = (Calendars)this.m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.Calendar);
				}
				return this.m_calendar.Value;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.Calendar.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.Calendar);
					this.m_calendar = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsCalendarAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.Calendar);
			}
		}

		public override string CurrencyLanguage
		{
			get
			{
				if (this.m_currencyLanguage == null)
				{
					this.m_currencyLanguage = this.m_styleDefinition.EvaluateInstanceStyleString(StyleAttributeNames.CurrencyLanguage);
				}
				return this.m_currencyLanguage;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.CurrencyLanguage.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.CurrencyLanguage);
					this.m_currencyLanguage = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsCurrencyLanguageAssigned
		{
			get
			{
				if (this.m_currencyLanguage == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.CurrencyLanguage);
			}
		}

		public override string NumeralLanguage
		{
			get
			{
				if (this.m_numeralLanguage == null)
				{
					this.m_numeralLanguage = this.m_styleDefinition.EvaluateInstanceStyleString(StyleAttributeNames.NumeralLanguage);
				}
				return this.m_numeralLanguage;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.NumeralLanguage.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.NumeralLanguage);
					this.m_numeralLanguage = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsNumeralLanguageAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.NumeralLanguage);
			}
		}

		public override BackgroundGradients BackgroundGradientType
		{
			get
			{
				if (!this.m_backgroundGradientType.HasValue)
				{
					this.m_backgroundGradientType = (BackgroundGradients)this.m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.BackgroundGradientType);
				}
				return this.m_backgroundGradientType.Value;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.BackgroundGradientType.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.BackgroundGradientType);
					this.m_backgroundGradientType = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsBackgroundGradientTypeAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.BackgroundGradientType);
			}
		}

		public override ReportSize FontSize
		{
			get
			{
				if (this.m_fontSize == null)
				{
					this.m_fontSize = this.m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.FontSize);
				}
				return this.m_fontSize;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.FontSize.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.FontSize);
					this.m_fontSize = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsFontSizeAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.FontSize);
			}
		}

		public override ReportSize PaddingLeft
		{
			get
			{
				if (this.m_paddingLeft == null)
				{
					this.m_paddingLeft = this.m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.PaddingLeft);
				}
				return this.m_paddingLeft;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.PaddingLeft.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.PaddingLeft);
					this.m_paddingLeft = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsPaddingLeftAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.PaddingLeft);
			}
		}

		public override ReportSize PaddingRight
		{
			get
			{
				if (this.m_paddingRight == null)
				{
					this.m_paddingRight = this.m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.PaddingRight);
				}
				return this.m_paddingRight;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.PaddingRight.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.PaddingRight);
					this.m_paddingRight = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsPaddingRightAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.PaddingRight);
			}
		}

		public override ReportSize PaddingTop
		{
			get
			{
				if (this.m_paddingTop == null)
				{
					this.m_paddingTop = this.m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.PaddingTop);
				}
				return this.m_paddingTop;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.PaddingTop.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.PaddingTop);
					this.m_paddingTop = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsPaddingTopAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.PaddingTop);
			}
		}

		public override ReportSize PaddingBottom
		{
			get
			{
				if (this.m_paddingBottom == null)
				{
					this.m_paddingBottom = this.m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.PaddingBottom);
				}
				return this.m_paddingBottom;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.PaddingBottom.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.PaddingBottom);
					this.m_paddingBottom = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsPaddingBottomAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.PaddingBottom);
			}
		}

		public override ReportSize LineHeight
		{
			get
			{
				if (this.m_lineHeight == null)
				{
					this.m_lineHeight = this.m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.LineHeight);
				}
				return this.m_lineHeight;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.LineHeight.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.LineHeight);
					this.m_lineHeight = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsLineHeightAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.LineHeight);
			}
		}

		public override int NumeralVariant
		{
			get
			{
				if (this.m_numeralVariant == -1)
				{
					this.m_numeralVariant = this.m_styleDefinition.EvaluateInstanceStyleInt(StyleAttributeNames.NumeralVariant, 1);
				}
				return this.m_numeralVariant;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.NumeralVariant.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.NumeralVariant);
					this.m_numeralVariant = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsNumeralVariantAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.NumeralVariant);
			}
		}

		public override TextEffects TextEffect
		{
			get
			{
				if (!this.m_textEffect.HasValue)
				{
					this.m_textEffect = (TextEffects)this.m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.TextEffect);
				}
				return this.m_textEffect.Value;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.NumeralVariant.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.TextEffect);
					this.m_textEffect = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsTextEffectAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.TextEffect);
			}
		}

		public override BackgroundHatchTypes BackgroundHatchType
		{
			get
			{
				if (!this.m_backgroundHatchType.HasValue)
				{
					this.m_backgroundHatchType = (BackgroundHatchTypes)this.m_styleDefinition.EvaluateInstanceStyleEnum(StyleAttributeNames.BackgroundHatchType);
				}
				return this.m_backgroundHatchType.Value;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.NumeralVariant.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.BackgroundHatchType);
					this.m_backgroundHatchType = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsBackgroundHatchTypeAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.BackgroundHatchType);
			}
		}

		public override ReportColor ShadowColor
		{
			get
			{
				if (this.m_shadowColor == null)
				{
					this.m_shadowColor = this.m_styleDefinition.EvaluateInstanceReportColor(StyleAttributeNames.ShadowColor);
				}
				return this.m_shadowColor;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.NumeralVariant.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.ShadowColor);
					this.m_shadowColor = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsShadowColorAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.ShadowColor);
			}
		}

		public override ReportSize ShadowOffset
		{
			get
			{
				if (this.m_shadowOffset == null)
				{
					this.m_shadowOffset = this.m_styleDefinition.EvaluateInstanceReportSize(StyleAttributeNames.ShadowOffset);
				}
				return this.m_shadowOffset;
			}
			set
			{
				if (this.m_styleDefinition.ReportElement != null && this.m_styleDefinition.ReportElement.CriGenerationPhase != 0 && (this.m_styleDefinition.ReportElement.CriGenerationPhase != ReportElement.CriGenerationPhases.Instance || this.m_styleDefinition.NumeralVariant.IsExpression))
				{
					this.AssignedValueTo(StyleAttributeNames.ShadowColor);
					this.m_shadowOffset = value;
					return;
				}
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal bool IsShadowOffsetAssigned
		{
			get
			{
				if (this.m_assignedValues == null)
				{
					return false;
				}
				return this.m_assignedValues.ContainsKey(StyleAttributeNames.ShadowOffset);
			}
		}

		internal StyleInstance(IROMStyleDefinitionContainer styleDefinitionContainer, IReportScope reportScope, RenderingContext context)
			: base(context, reportScope)
		{
			this.m_styleDefinition = styleDefinitionContainer.Style;
		}

		protected override void ResetInstanceCache()
		{
			this.m_backgroundColor = null;
			this.m_backgroundGradientEndColor = null;
			this.m_color = null;
			this.m_fontStyle = null;
			this.m_fontFamily = null;
			this.m_fontWeight = null;
			this.m_format = null;
			this.m_textDecoration = null;
			this.m_textAlign = null;
			this.m_verticalAlign = null;
			this.m_direction = null;
			this.m_writingMode = null;
			this.m_language = null;
			this.m_unicodeBiDi = null;
			this.m_calendar = null;
			this.m_currencyLanguage = null;
			this.m_numeralLanguage = null;
			this.m_backgroundGradientType = null;
			this.m_fontSize = null;
			this.m_paddingLeft = null;
			this.m_paddingRight = null;
			this.m_paddingTop = null;
			this.m_paddingBottom = null;
			this.m_lineHeight = null;
			this.m_numeralVariant = -1;
			this.m_textEffect = null;
			this.m_backgroundHatchType = null;
			this.m_shadowColor = null;
			this.m_shadowOffset = null;
			this.m_assignedValues = null;
		}

		private void AssignedValueTo(StyleAttributeNames styleName)
		{
			if (this.m_assignedValues == null)
			{
				this.m_assignedValues = new Dictionary<StyleAttributeNames, bool>();
			}
			if (!this.m_assignedValues.ContainsKey(styleName))
			{
				this.m_assignedValues.Add(styleName, true);
			}
		}

		void IPersistable.Serialize(IntermediateFormatWriter writer)
		{
			writer.RegisterDeclaration(StyleInstance.m_Declaration);
			List<int> list = default(List<int>);
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo> rifList = default(List<AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo>);
			this.GetStyleDynamicValues(out list, out rifList);
			while (writer.NextMember())
			{
				switch (writer.CurrentMember.MemberName)
				{
				case MemberName.StyleAttributes:
					writer.WriteListOfPrimitives(list);
					break;
				case MemberName.StyleAttributeValues:
					writer.Write(rifList);
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
		}

		void IPersistable.Deserialize(IntermediateFormatReader reader)
		{
			reader.RegisterDeclaration(StyleInstance.m_Declaration);
			List<int> styles = null;
			List<AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo> values = null;
			while (reader.NextMember())
			{
				switch (reader.CurrentMember.MemberName)
				{
				case MemberName.StyleAttributes:
					styles = reader.ReadListOfPrimitives<int>();
					break;
				case MemberName.StyleAttributeValues:
					values = reader.ReadListOfRIFObjects<List<AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo>>();
					break;
				default:
					Global.Tracer.Assert(false);
					break;
				}
			}
			this.SetStyleDynamicValues(styles, values);
		}

		void IPersistable.ResolveReferences(Dictionary<AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType, List<MemberReference>> memberReferencesCollection, Dictionary<int, IReferenceable> referenceableItems)
		{
			Global.Tracer.Assert(false);
		}

		AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType IPersistable.GetObjectType()
		{
			return AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StyleInstance;
		}

		private static Declaration GetDeclaration()
		{
			List<MemberInfo> list = new List<MemberInfo>();
			list.Add(new MemberInfo(MemberName.StyleAttributes, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.PrimitiveList, Token.Int32));
			list.Add(new MemberInfo(MemberName.StyleAttributeValues, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.RIFObjectList, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.AttributeInfo));
			return new Declaration(AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.StyleInstance, AspNetCore.ReportingServices.ReportIntermediateFormat.Persistence.ObjectType.None, list);
		}

		private void GetStyleDynamicValues(out List<int> styles, out List<AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo> values)
		{
			styles = new List<int>();
			values = new List<AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo>();
			this.m_styleDefinition.Border.Instance.GetAssignedDynamicValues(styles, values);
			this.m_styleDefinition.TopBorder.Instance.GetAssignedDynamicValues(styles, values);
			this.m_styleDefinition.BottomBorder.Instance.GetAssignedDynamicValues(styles, values);
			this.m_styleDefinition.LeftBorder.Instance.GetAssignedDynamicValues(styles, values);
			this.m_styleDefinition.RightBorder.Instance.GetAssignedDynamicValues(styles, values);
			if (this.IsBackgroundColorAssigned && this.m_styleDefinition.BackgroundColor.IsExpression)
			{
				styles.Add(15);
				values.Add(StyleInstance.CreateAttrInfo(this.m_backgroundColor));
			}
			if (this.IsBackgroundGradientEndColorAssigned && this.m_styleDefinition.BackgroundGradientEndColor.IsExpression)
			{
				styles.Add(38);
				values.Add(StyleInstance.CreateAttrInfo(this.m_backgroundGradientEndColor));
			}
			if (this.IsColorAssigned && this.m_styleDefinition.Color.IsExpression)
			{
				styles.Add(24);
				values.Add(StyleInstance.CreateAttrInfo(this.m_color));
			}
			if (this.IsFontStyleAssigned && this.m_styleDefinition.FontStyle.IsExpression)
			{
				styles.Add(16);
				values.Add(StyleInstance.CreateAttrInfo((int)this.m_fontStyle.Value));
			}
			if (this.IsFontFamilyAssigned && this.m_styleDefinition.FontFamily.IsExpression)
			{
				styles.Add(17);
				values.Add(StyleInstance.CreateAttrInfo(this.m_fontFamily));
			}
			if (this.IsFontWeightAssigned && this.m_styleDefinition.FontWeight.IsExpression)
			{
				styles.Add(19);
				values.Add(StyleInstance.CreateAttrInfo((int)this.m_fontWeight.Value));
			}
			if (this.IsFormatAssigned && this.m_styleDefinition.Format.IsExpression)
			{
				styles.Add(20);
				values.Add(StyleInstance.CreateAttrInfo(this.m_format));
			}
			if (this.IsTextDecorationAssigned && this.m_styleDefinition.TextDecoration.IsExpression)
			{
				styles.Add(21);
				values.Add(StyleInstance.CreateAttrInfo((int)this.m_textDecoration.Value));
			}
			if (this.IsTextAlignAssigned && this.m_styleDefinition.TextAlign.IsExpression)
			{
				styles.Add(22);
				values.Add(StyleInstance.CreateAttrInfo((int)this.m_textAlign.Value));
			}
			if (this.IsVerticalAlignAssigned && this.m_styleDefinition.VerticalAlign.IsExpression)
			{
				styles.Add(23);
				values.Add(StyleInstance.CreateAttrInfo((int)this.m_verticalAlign.Value));
			}
			if (this.IsDirectionAssigned && this.m_styleDefinition.Direction.IsExpression)
			{
				styles.Add(30);
				values.Add(StyleInstance.CreateAttrInfo((int)this.m_direction.Value));
			}
			if (this.IsWritingModeAssigned && this.m_styleDefinition.WritingMode.IsExpression)
			{
				styles.Add(31);
				values.Add(StyleInstance.CreateAttrInfo((int)this.m_writingMode.Value));
			}
			if (this.IsLanguageAssigned && this.m_styleDefinition.Language.IsExpression)
			{
				styles.Add(32);
				values.Add(StyleInstance.CreateAttrInfo(this.m_language));
			}
			if (this.IsUnicodeBiDiAssigned && this.m_styleDefinition.UnicodeBiDi.IsExpression)
			{
				styles.Add(33);
				values.Add(StyleInstance.CreateAttrInfo((int)this.m_unicodeBiDi.Value));
			}
			if (this.IsCalendarAssigned && this.m_styleDefinition.Calendar.IsExpression)
			{
				styles.Add(34);
				values.Add(StyleInstance.CreateAttrInfo((int)this.m_calendar.Value));
			}
			if (this.IsCurrencyLanguageAssigned && this.m_styleDefinition.CurrencyLanguage.IsExpression)
			{
				styles.Add(50);
				values.Add(StyleInstance.CreateAttrInfo(this.m_currencyLanguage));
			}
			if (this.IsNumeralLanguageAssigned && this.m_styleDefinition.NumeralLanguage.IsExpression)
			{
				styles.Add(35);
				values.Add(StyleInstance.CreateAttrInfo(this.m_numeralLanguage));
			}
			if (this.IsBackgroundGradientTypeAssigned && this.m_styleDefinition.BackgroundGradientType.IsExpression)
			{
				styles.Add(37);
				values.Add(StyleInstance.CreateAttrInfo((int)this.m_backgroundGradientType.Value));
			}
			if (this.IsFontSizeAssigned && this.m_styleDefinition.FontSize.IsExpression)
			{
				styles.Add(18);
				values.Add(StyleInstance.CreateAttrInfo(this.m_fontSize));
			}
			if (this.IsPaddingLeftAssigned && this.m_styleDefinition.PaddingLeft.IsExpression)
			{
				styles.Add(25);
				values.Add(StyleInstance.CreateAttrInfo(this.m_paddingLeft));
			}
			if (this.IsPaddingRightAssigned && this.m_styleDefinition.PaddingRight.IsExpression)
			{
				styles.Add(26);
				values.Add(StyleInstance.CreateAttrInfo(this.m_paddingRight));
			}
			if (this.IsPaddingTopAssigned && this.m_styleDefinition.PaddingTop.IsExpression)
			{
				styles.Add(27);
				values.Add(StyleInstance.CreateAttrInfo(this.m_paddingTop));
			}
			if (this.IsPaddingBottomAssigned && this.m_styleDefinition.PaddingBottom.IsExpression)
			{
				styles.Add(28);
				values.Add(StyleInstance.CreateAttrInfo(this.m_paddingBottom));
			}
			if (this.IsLineHeightAssigned && this.m_styleDefinition.LineHeight.IsExpression)
			{
				styles.Add(29);
				values.Add(StyleInstance.CreateAttrInfo(this.m_lineHeight));
			}
			if (this.IsNumeralVariantAssigned && this.m_styleDefinition.NumeralVariant.IsExpression)
			{
				styles.Add(36);
				values.Add(StyleInstance.CreateAttrInfo(this.m_numeralVariant));
			}
			Global.Tracer.Assert(styles.Count == values.Count, "styles.Count == values.Count");
		}

		internal static AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo CreateAttrInfo(ReportColor reportColor)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo();
			attributeInfo.IsExpression = true;
			attributeInfo.Value = ((reportColor != null) ? reportColor.ToString() : null);
			return attributeInfo;
		}

		internal static AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo CreateAttrInfo(ReportSize reportSize)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo();
			attributeInfo.IsExpression = true;
			attributeInfo.Value = ((reportSize != null) ? reportSize.ToString() : null);
			return attributeInfo;
		}

		internal static AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo CreateAttrInfo(string strValue)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo();
			attributeInfo.IsExpression = true;
			attributeInfo.Value = ((strValue != null) ? strValue.ToString() : null);
			return attributeInfo;
		}

		internal static AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo CreateAttrInfo(int intValue)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = new AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo();
			attributeInfo.IsExpression = true;
			attributeInfo.IntValue = intValue;
			return attributeInfo;
		}

		private void SetStyleDynamicValues(List<int> styles, List<AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo> values)
		{
			if (styles == null && values == null)
			{
				return;
			}
			Global.Tracer.Assert(styles != null && values != null && styles.Count == values.Count, "styles != null && values != null && styles.Count == values.Count");
			for (int i = 0; i < styles.Count; i++)
			{
				StyleAttributeNames styleAttributeNames = (StyleAttributeNames)styles[i];
				AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = values[i];
				switch (styleAttributeNames)
				{
				case StyleAttributeNames.BorderColor:
					this.m_styleDefinition.Border.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Color, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderColorTop:
					this.m_styleDefinition.TopBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Color, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderColorLeft:
					this.m_styleDefinition.LeftBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Color, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderColorRight:
					this.m_styleDefinition.RightBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Color, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderColorBottom:
					this.m_styleDefinition.BottomBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Color, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderStyle:
					this.m_styleDefinition.Border.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Style, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderStyleTop:
					this.m_styleDefinition.TopBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Style, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderStyleLeft:
					this.m_styleDefinition.LeftBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Style, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderStyleRight:
					this.m_styleDefinition.RightBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Style, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderStyleBottom:
					this.m_styleDefinition.BottomBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Style, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderWidth:
					this.m_styleDefinition.Border.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Width, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderWidthTop:
					this.m_styleDefinition.TopBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Width, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderWidthLeft:
					this.m_styleDefinition.LeftBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Width, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderWidthRight:
					this.m_styleDefinition.RightBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Width, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BorderWidthBottom:
					this.m_styleDefinition.BottomBorder.Instance.SetAssignedDynamicValue(BorderInstance.BorderStyleProperty.Width, attributeInfo, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BackgroundColor:
					this.m_backgroundColor = new ReportColor(attributeInfo.Value, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BackgroundGradientEndColor:
					this.m_backgroundGradientEndColor = new ReportColor(attributeInfo.Value, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.BackgroundGradientType:
					this.m_backgroundGradientType = (BackgroundGradients)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.Calendar:
					this.m_calendar = (Calendars)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.Color:
					this.m_color = new ReportColor(attributeInfo.Value, this.m_styleDefinition.IsDynamicImageStyle);
					break;
				case StyleAttributeNames.Direction:
					this.m_direction = (Directions)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.FontFamily:
					this.m_fontFamily = attributeInfo.Value;
					break;
				case StyleAttributeNames.FontSize:
					this.m_fontSize = new ReportSize(attributeInfo.Value);
					break;
				case StyleAttributeNames.FontStyle:
					this.m_fontStyle = (FontStyles)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.FontWeight:
					this.m_fontWeight = (FontWeights)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.Format:
					this.m_format = attributeInfo.Value;
					break;
				case StyleAttributeNames.Language:
					this.m_language = attributeInfo.Value;
					break;
				case StyleAttributeNames.LineHeight:
					this.m_lineHeight = new ReportSize(attributeInfo.Value);
					break;
				case StyleAttributeNames.CurrencyLanguage:
					this.m_currencyLanguage = attributeInfo.Value;
					break;
				case StyleAttributeNames.NumeralLanguage:
					this.m_numeralLanguage = attributeInfo.Value;
					break;
				case StyleAttributeNames.NumeralVariant:
					this.m_numeralVariant = attributeInfo.IntValue;
					break;
				case StyleAttributeNames.PaddingBottom:
					this.m_paddingBottom = new ReportSize(attributeInfo.Value);
					break;
				case StyleAttributeNames.PaddingLeft:
					this.m_paddingLeft = new ReportSize(attributeInfo.Value);
					break;
				case StyleAttributeNames.PaddingRight:
					this.m_paddingRight = new ReportSize(attributeInfo.Value);
					break;
				case StyleAttributeNames.PaddingTop:
					this.m_paddingTop = new ReportSize(attributeInfo.Value);
					break;
				case StyleAttributeNames.TextAlign:
					this.m_textAlign = (TextAlignments)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.TextDecoration:
					this.m_textDecoration = (TextDecorations)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.UnicodeBiDi:
					this.m_unicodeBiDi = (UnicodeBiDiTypes)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.VerticalAlign:
					this.m_verticalAlign = (VerticalAlignments)attributeInfo.IntValue;
					break;
				case StyleAttributeNames.WritingMode:
					this.m_writingMode = (WritingModes)attributeInfo.IntValue;
					break;
				}
			}
		}
	}
}
