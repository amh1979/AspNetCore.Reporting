using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class BackgroundImage : ReportProperty, IImage, IBaseImage
	{
		private bool m_isOldSnapshot;

		private AspNetCore.ReportingServices.ReportRendering.Style m_renderStyle;

		private Style m_styleDef;

		private ReportStringProperty m_value;

		private ReportStringProperty m_mimeType;

		private ReportEnumProperty<BackgroundRepeatTypes> m_repeat;

		private BackgroundImageInstance m_instance;

		private Image.SourceType? m_imageSource = null;

		private ReportEnumProperty<Positions> m_position;

		private ReportColorProperty m_transparentColor;

		public Image.SourceType Source
		{
			get
			{
				if (!this.m_imageSource.HasValue)
				{
					if (this.m_isOldSnapshot)
					{
						AspNetCore.ReportingServices.ReportRendering.Image.SourceType value = default(AspNetCore.ReportingServices.ReportRendering.Image.SourceType);
						if (((AspNetCore.ReportingServices.ReportRendering.StyleBase)this.m_renderStyle).GetBackgroundImageSource(this.m_renderStyle.GetStyleDefinition("BackgroundImageSource"), out value))
						{
							this.m_imageSource = (Image.SourceType)value;
						}
					}
					else
					{
						int? nullable = this.m_styleDef.EvaluateInstanceStyleEnum(AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId.BackgroundImageSource);
						this.m_imageSource = (Image.SourceType)(nullable.HasValue ? nullable.Value : 0);
					}
				}
				return this.m_imageSource.Value;
			}
		}

		public ReportStringProperty Value
		{
			get
			{
				if (this.m_value == null)
				{
					if (this.m_isOldSnapshot)
					{
						object obj = default(object);
						bool isExpression = default(bool);
						if (((AspNetCore.ReportingServices.ReportRendering.StyleBase)this.m_renderStyle).GetBackgroundImageValue(this.m_renderStyle.GetStyleDefinition("BackgroundImageValue"), out obj, out isExpression))
						{
							this.m_value = new ReportStringProperty(isExpression, null, (obj is string) ? ((string)obj) : null);
						}
						else
						{
							this.m_value = new ReportStringProperty();
						}
					}
					else
					{
						string expressionString = default(string);
						AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = this.m_styleDef.GetAttributeInfo("BackgroundImageValue", out expressionString);
						if (attributeInfo == null)
						{
							this.m_value = new ReportStringProperty();
						}
						else
						{
							this.m_value = new ReportStringProperty(attributeInfo.IsExpression, expressionString, attributeInfo.Value);
						}
					}
				}
				return this.m_value;
			}
		}

		public ReportStringProperty MIMEType
		{
			get
			{
				if (this.m_mimeType == null)
				{
					if (this.m_isOldSnapshot)
					{
						AspNetCore.ReportingServices.ReportProcessing.AttributeInfo styleDefinition = this.m_renderStyle.GetStyleDefinition("BackgroundImageMIMEType");
						if (styleDefinition == null)
						{
							this.m_mimeType = new ReportStringProperty();
						}
						else
						{
							this.m_mimeType = new ReportStringProperty(styleDefinition.IsExpression, null, styleDefinition.Value);
						}
					}
					else
					{
						string expressionString = default(string);
						AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = this.m_styleDef.GetAttributeInfo("BackgroundImageMIMEType", out expressionString);
						if (attributeInfo == null)
						{
							this.m_mimeType = new ReportStringProperty();
						}
						else
						{
							this.m_mimeType = new ReportStringProperty(attributeInfo.IsExpression, expressionString, attributeInfo.Value);
						}
					}
				}
				return this.m_mimeType;
			}
		}

		public ReportEnumProperty<BackgroundRepeatTypes> BackgroundRepeat
		{
			get
			{
				if (this.m_repeat == null)
				{
					if (this.m_isOldSnapshot)
					{
						AspNetCore.ReportingServices.ReportProcessing.AttributeInfo styleDefinition = this.m_renderStyle.GetStyleDefinition("BackgroundRepeat");
						if (styleDefinition == null)
						{
							this.m_repeat = new ReportEnumProperty<BackgroundRepeatTypes>(Style.DefaultEnumBackgroundRepeatType);
						}
						else
						{
							this.m_repeat = new ReportEnumProperty<BackgroundRepeatTypes>(styleDefinition.IsExpression, null, StyleTranslator.TranslateBackgroundRepeat(styleDefinition.Value, null, this.m_styleDef.IsDynamicImageStyle), Style.DefaultEnumBackgroundRepeatType);
						}
					}
					else
					{
						string expressionString = default(string);
						AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = this.m_styleDef.GetAttributeInfo("BackgroundRepeat", out expressionString);
						if (attributeInfo == null)
						{
							this.m_repeat = new ReportEnumProperty<BackgroundRepeatTypes>(Style.DefaultEnumBackgroundRepeatType);
						}
						else
						{
							this.m_repeat = new ReportEnumProperty<BackgroundRepeatTypes>(attributeInfo.IsExpression, expressionString, (BackgroundRepeatTypes)StyleTranslator.TranslateStyle(StyleAttributeNames.BackgroundImageRepeat, attributeInfo.Value, null, this.m_styleDef.IsDynamicImageStyle), Style.DefaultEnumBackgroundRepeatType);
						}
					}
				}
				return this.m_repeat;
			}
		}

		public ReportEnumProperty<Positions> Position
		{
			get
			{
				if (this.m_position == null)
				{
					if (this.m_isOldSnapshot)
					{
						this.m_position = new ReportEnumProperty<Positions>();
					}
					else
					{
						string expressionString = default(string);
						AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = this.m_styleDef.GetAttributeInfo("Position", out expressionString);
						bool isExpression = false;
						string styleString = null;
						if (attributeInfo != null)
						{
							styleString = attributeInfo.Value;
							isExpression = attributeInfo.IsExpression;
						}
						this.m_position = new ReportEnumProperty<Positions>(isExpression, expressionString, StyleTranslator.TranslatePosition(styleString, null, this.m_styleDef.IsDynamicImageStyle));
					}
				}
				return this.m_position;
			}
		}

		public ReportColorProperty TransparentColor
		{
			get
			{
				if (this.m_transparentColor == null)
				{
					ReportColor defaultValue = new ReportColor("Transparent", Color.Transparent, true);
					if (this.m_isOldSnapshot)
					{
						this.m_transparentColor = new ReportColorProperty(false, null, null, defaultValue);
					}
					else
					{
						string expressionString = default(string);
						AspNetCore.ReportingServices.ReportIntermediateFormat.AttributeInfo attributeInfo = this.m_styleDef.GetAttributeInfo("TransparentColor", out expressionString);
						bool flag = false;
						ReportColor value = null;
						if (attributeInfo != null)
						{
							flag = attributeInfo.IsExpression;
							if (!flag)
							{
								value = new ReportColor(attributeInfo.Value, this.m_styleDef.IsDynamicImageStyle);
							}
						}
						this.m_transparentColor = new ReportColorProperty(flag, expressionString, value, defaultValue);
					}
				}
				return this.m_transparentColor;
			}
		}

		public BackgroundImageInstance Instance
		{
			get
			{
				if (!this.m_isOldSnapshot && this.m_styleDef.m_renderingContext.InstanceAccessDisallowed)
				{
					return null;
				}
				if (this.m_instance == null)
				{
					if (this.m_isOldSnapshot)
					{
						this.m_instance = new ShimBackgroundImageInstance(this, ((AspNetCore.ReportingServices.ReportRendering.StyleBase)this.m_renderStyle)["BackgroundImage"] as AspNetCore.ReportingServices.ReportRendering.BackgroundImage, ((AspNetCore.ReportingServices.ReportRendering.StyleBase)this.m_renderStyle)["BackgroundRepeat"] as string);
					}
					else
					{
						this.m_instance = new InternalBackgroundImageInstance(this);
					}
				}
				return this.m_instance;
			}
		}

		internal Style StyleDef
		{
			get
			{
				return this.m_styleDef;
			}
		}

		ObjectType IBaseImage.ObjectType
		{
			get
			{
				return this.m_styleDef.StyleContainer.ObjectType;
			}
		}

		string IBaseImage.ObjectName
		{
			get
			{
				return this.m_styleDef.StyleContainer.Name;
			}
		}

		ReportProperty IBaseImage.Value
		{
			get
			{
				return this.Value;
			}
		}

		string IBaseImage.ImageDataPropertyName
		{
			get
			{
				return "BackgroundImageValue";
			}
		}

		string IBaseImage.ImageValuePropertyName
		{
			get
			{
				return "BackgroundImageValue";
			}
		}

		string IBaseImage.MIMETypePropertyName
		{
			get
			{
				return "BackgroundImageMIMEType";
			}
		}

		Image.EmbeddingModes IBaseImage.EmbeddingMode
		{
			get
			{
				return Image.EmbeddingModes.Inline;
			}
		}

		internal BackgroundImage(bool isExpression, string expressionString, Style styleDef)
			: base(isExpression, expressionString)
		{
			this.m_styleDef = styleDef;
			this.m_isOldSnapshot = false;
		}

		internal BackgroundImage(bool isExpression, string expressionString, AspNetCore.ReportingServices.ReportRendering.Style renderStyle, Style styleDef)
			: base(isExpression, expressionString)
		{
			this.m_styleDef = styleDef;
			this.m_renderStyle = renderStyle;
			this.m_isOldSnapshot = true;
		}

		byte[] IBaseImage.GetImageData(out List<string> fieldsUsedInValue, out bool errorOccurred)
		{
			fieldsUsedInValue = null;
			errorOccurred = false;
			object obj = this.m_styleDef.EvaluateInstanceStyleVariant(AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId.BackgroundImageValue);
			return obj as byte[];
		}

		string IBaseImage.GetMIMETypeValue()
		{
			return this.m_styleDef.EvaluateInstanceStyleString(AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId.BackgroundImageMimeType);
		}

		string IBaseImage.GetValueAsString(out List<string> fieldsUsedInValue, out bool errOccurred)
		{
			errOccurred = false;
			fieldsUsedInValue = null;
			return this.m_styleDef.EvaluateInstanceStyleString(AspNetCore.ReportingServices.ReportIntermediateFormat.Style.StyleId.BackgroundImageValue);
		}

		string IBaseImage.GetTransparentImageProperties(out string mimeType, out byte[] imageData)
		{
			mimeType = null;
			imageData = null;
			return null;
		}
	}
}
