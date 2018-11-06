using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Generic;
using System.Drawing;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class BaseGaugeImage : IBaseImage
	{
		internal GaugePanel m_gaugePanel;

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.BaseGaugeImage m_defObject;

		internal BaseGaugeImageInstance m_instance;

		private ReportEnumProperty<Image.SourceType> m_source;

		private ReportVariantProperty m_value;

		private ReportStringProperty m_MIMEType;

		private ReportColorProperty m_transparentColor;

		ObjectType IBaseImage.ObjectType
		{
			get
			{
				return this.m_gaugePanel.ReportItemDef.ObjectType;
			}
		}

		string IBaseImage.ObjectName
		{
			get
			{
				return this.m_gaugePanel.Name;
			}
		}

		Image.SourceType IBaseImage.Source
		{
			get
			{
				ReportEnumProperty<Image.SourceType> source = this.Source;
				if (!source.IsExpression)
				{
					return source.Value;
				}
				return this.Instance.Source;
			}
		}

		ReportProperty IBaseImage.Value
		{
			get
			{
				return this.Value;
			}
		}

		ReportStringProperty IBaseImage.MIMEType
		{
			get
			{
				return this.MIMEType;
			}
		}

		string IBaseImage.ImageDataPropertyName
		{
			get
			{
				return "ImageData";
			}
		}

		string IBaseImage.ImageValuePropertyName
		{
			get
			{
				return "Value";
			}
		}

		string IBaseImage.MIMETypePropertyName
		{
			get
			{
				return "MIMEType";
			}
		}

		Image.EmbeddingModes IBaseImage.EmbeddingMode
		{
			get
			{
				return Image.EmbeddingModes.Inline;
			}
		}

		public ReportEnumProperty<Image.SourceType> Source
		{
			get
			{
				if (this.m_source == null && this.m_defObject.Source != null)
				{
					this.m_source = new ReportEnumProperty<Image.SourceType>(this.m_defObject.Source.IsExpression, this.m_defObject.Source.OriginalText, EnumTranslator.TranslateImageSourceType(this.m_defObject.Source.StringValue, null));
				}
				return this.m_source;
			}
		}

		public ReportVariantProperty Value
		{
			get
			{
				if (this.m_value == null && this.m_defObject.Value != null)
				{
					this.m_value = new ReportVariantProperty(this.m_defObject.Value);
				}
				return this.m_value;
			}
		}

		public ReportStringProperty MIMEType
		{
			get
			{
				if (this.m_MIMEType == null && this.m_defObject.MIMEType != null)
				{
					this.m_MIMEType = new ReportStringProperty(this.m_defObject.MIMEType);
				}
				return this.m_MIMEType;
			}
		}

		public ReportColorProperty TransparentColor
		{
			get
			{
				if (this.m_transparentColor == null && this.m_defObject.TransparentColor != null)
				{
					AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo transparentColor = this.m_defObject.TransparentColor;
					if (transparentColor != null)
					{
						this.m_transparentColor = new ReportColorProperty(transparentColor.IsExpression, transparentColor.OriginalText, transparentColor.IsExpression ? null : new ReportColor(transparentColor.StringValue.Trim(), true), transparentColor.IsExpression ? new ReportColor("", Color.Empty, true) : null);
					}
				}
				return this.m_transparentColor;
			}
		}

		internal GaugePanel GaugePanelDef
		{
			get
			{
				return this.m_gaugePanel;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.BaseGaugeImage BaseGaugeImageDef
		{
			get
			{
				return this.m_defObject;
			}
		}

		internal BaseGaugeImageInstance Instance
		{
			get
			{
				return this.GetInstance();
			}
		}

		internal BaseGaugeImage(AspNetCore.ReportingServices.ReportIntermediateFormat.BaseGaugeImage defObject, GaugePanel gaugePanel)
		{
			this.m_defObject = defObject;
			this.m_gaugePanel = gaugePanel;
		}

		byte[] IBaseImage.GetImageData(out List<string> fieldsUsedInValue, out bool errorOccurred)
		{
			fieldsUsedInValue = null;
			return this.m_defObject.EvaluateBinaryValue(this.Instance.ReportScopeInstance, this.m_gaugePanel.RenderingContext.OdpContext, out errorOccurred);
		}

		string IBaseImage.GetMIMETypeValue()
		{
			ReportStringProperty mIMEType = this.MIMEType;
			if (mIMEType == null)
			{
				return null;
			}
			if (!mIMEType.IsExpression)
			{
				return mIMEType.Value;
			}
			return this.BaseGaugeImageDef.EvaluateMIMEType(this.Instance.ReportScopeInstance, this.GaugePanelDef.RenderingContext.OdpContext);
		}

		string IBaseImage.GetValueAsString(out List<string> fieldsUsedInValue, out bool errOccurred)
		{
			fieldsUsedInValue = null;
			ReportVariantProperty value = this.Value;
			errOccurred = false;
			if (!value.IsExpression)
			{
				object value2 = value.Value;
				if (value2 is string)
				{
					return (string)value2;
				}
				return null;
			}
			return this.m_defObject.EvaluateStringValue(this.Instance.ReportScopeInstance, this.m_gaugePanel.RenderingContext.OdpContext, out errOccurred);
		}

		string IBaseImage.GetTransparentImageProperties(out string mimeType, out byte[] imageData)
		{
			mimeType = null;
			imageData = null;
			return null;
		}

		internal abstract BaseGaugeImageInstance GetInstance();

		internal virtual void SetNewContext()
		{
			if (this.m_instance != null)
			{
				this.m_instance.SetNewContext();
			}
		}
	}
}
