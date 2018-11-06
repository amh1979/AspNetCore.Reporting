using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class Image : ReportItem, IImage, IROMActionOwner, IBaseImage
	{
		internal enum Sizings
		{
			AutoSize,
			Fit,
			FitProportional,
			Clip
		}

		internal enum SourceType
		{
			External,
			Embedded,
			Database
		}

		internal enum EmbeddingModes
		{
			Inline,
			Package
		}

		private AspNetCore.ReportingServices.ReportRendering.Image m_renderImage;

		private ReportStringProperty m_value;

		private ReportStringProperty m_mimeType;

		private TagCollection m_tags;

		private ActionInfo m_actionInfo;

		public SourceType Source
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return (SourceType)((AspNetCore.ReportingServices.ReportProcessing.Image)base.m_renderReportItem.ReportItemDef).Source;
				}
				return this.ImageDef.Source;
			}
		}

		public ReportStringProperty Value
		{
			get
			{
				if (this.m_value == null)
				{
					if (base.m_isOldSnapshot)
					{
						this.m_value = new ReportStringProperty(((AspNetCore.ReportingServices.ReportProcessing.Image)base.m_renderReportItem.ReportItemDef).Value);
					}
					else
					{
						this.m_value = new ReportStringProperty(this.ImageDef.Value);
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
					if (base.m_isOldSnapshot)
					{
						this.m_mimeType = new ReportStringProperty(((AspNetCore.ReportingServices.ReportProcessing.Image)base.m_renderReportItem.ReportItemDef).MIMEType);
					}
					else
					{
						this.m_mimeType = new ReportStringProperty(this.ImageDef.MIMEType);
					}
				}
				return this.m_mimeType;
			}
		}

		public ReportVariantProperty Tag
		{
			get
			{
				TagCollection tags = this.Tags;
				if (tags == null)
				{
					return new ReportVariantProperty(false);
				}
				return ((ReportElementCollectionBase<Tag>)tags)[0].Value;
			}
		}

		internal TagCollection Tags
		{
			get
			{
				if (this.m_tags == null && !base.m_isOldSnapshot && this.ImageDef.Tags != null)
				{
					this.m_tags = new TagCollection(this);
				}
				return this.m_tags;
			}
		}

		internal bool IsNullImage
		{
			get
			{
				if (this.Value.IsExpression)
				{
					return false;
				}
				return string.IsNullOrEmpty(this.Value.Value);
			}
		}

		public Sizings Sizing
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return (Sizings)((AspNetCore.ReportingServices.ReportRendering.Image)base.m_renderReportItem).Sizing;
				}
				return this.ImageDef.Sizing;
			}
			set
			{
				if (base.CriGenerationPhase != CriGenerationPhases.Definition)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.ImageDef.Sizing = value;
			}
		}

		string IROMActionOwner.UniqueName
		{
			get
			{
				return base.m_reportItemDef.UniqueName;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				if (this.m_actionInfo == null)
				{
					if (base.m_isOldSnapshot)
					{
						if (((AspNetCore.ReportingServices.ReportRendering.Image)base.m_renderReportItem).ActionInfo != null)
						{
							this.m_actionInfo = new ActionInfo(base.RenderingContext, ((AspNetCore.ReportingServices.ReportRendering.Image)base.m_renderReportItem).ActionInfo);
						}
					}
					else
					{
						AspNetCore.ReportingServices.ReportIntermediateFormat.Action action = this.ImageDef.Action;
						if (action != null)
						{
							this.m_actionInfo = new ActionInfo(base.RenderingContext, this.ReportScope, action, base.m_reportItemDef, this, base.m_reportItemDef.ObjectType, base.m_reportItemDef.Name, this);
						}
					}
				}
				return this.m_actionInfo;
			}
		}

		public ImageInstance ImageInstance
		{
			get
			{
				return (ImageInstance)base.Instance;
			}
		}

		internal AspNetCore.ReportingServices.ReportIntermediateFormat.Image ImageDef
		{
			get
			{
				return (AspNetCore.ReportingServices.ReportIntermediateFormat.Image)base.m_reportItemDef;
			}
		}

		List<string> IROMActionOwner.FieldsUsedInValueExpression
		{
			get
			{
				return ((ImageInstance)this.GetOrCreateInstance()).GetFieldsUsedInValueExpression();
			}
		}

		ObjectType IBaseImage.ObjectType
		{
			get
			{
				return ObjectType.Image;
			}
		}

		string IBaseImage.ObjectName
		{
			get
			{
				return this.Name;
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

		EmbeddingModes IBaseImage.EmbeddingMode
		{
			get
			{
				if (base.m_isOldSnapshot)
				{
					return EmbeddingModes.Inline;
				}
				return this.ImageDef.EmbeddingMode;
			}
		}

		internal Image(IReportScope reportScope, IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, AspNetCore.ReportingServices.ReportIntermediateFormat.Image reportItemDef, RenderingContext renderingContext)
			: base(reportScope, parentDefinitionPath, indexIntoParentCollectionDef, reportItemDef, renderingContext)
		{
		}

		internal Image(IDefinitionPath parentDefinitionPath, int indexIntoParentCollectionDef, bool inSubtotal, AspNetCore.ReportingServices.ReportRendering.Image renderImage, RenderingContext renderingContext)
			: base(parentDefinitionPath, indexIntoParentCollectionDef, inSubtotal, renderImage, renderingContext)
		{
			this.m_renderImage = renderImage;
		}

		internal override ReportItemInstance GetOrCreateInstance()
		{
			if (base.m_instance == null)
			{
				if (base.CriOwner != null)
				{
					base.m_instance = new CriImageInstance(this);
				}
				else if (base.IsOldSnapshot)
				{
					base.m_instance = new ShimImageInstance(this);
				}
				else
				{
					base.m_instance = new InternalImageInstance(this);
				}
			}
			return base.m_instance;
		}

		byte[] IBaseImage.GetImageData(out List<string> fieldsUsedInValue, out bool errorOccurred)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Image image = (AspNetCore.ReportingServices.ReportIntermediateFormat.Image)base.ReportItemDef;
			bool flag = image.ShouldTrackFieldsUsedInValue();
			fieldsUsedInValue = null;
			if (flag)
			{
				base.RenderingContext.OdpContext.ReportObjectModel.ResetFieldsUsedInExpression();
			}
			byte[] result = image.EvaluateBinaryValueExpression(base.Instance.ReportScopeInstance, base.RenderingContext.OdpContext, out errorOccurred);
			if (errorOccurred)
			{
				return null;
			}
			if (flag)
			{
				fieldsUsedInValue = new List<string>();
				base.RenderingContext.OdpContext.ReportObjectModel.AddFieldsUsedInExpression(fieldsUsedInValue);
			}
			return result;
		}

		string IBaseImage.GetMIMETypeValue()
		{
			return ((AspNetCore.ReportingServices.ReportIntermediateFormat.Image)base.ReportItemDef).EvaluateMimeTypeExpression(base.Instance.ReportScopeInstance, base.RenderingContext.OdpContext);
		}

		string IBaseImage.GetValueAsString(out List<string> fieldsUsedInValue, out bool errOccurred)
		{
			AspNetCore.ReportingServices.ReportIntermediateFormat.Image image = (AspNetCore.ReportingServices.ReportIntermediateFormat.Image)base.ReportItemDef;
			bool flag = image.ShouldTrackFieldsUsedInValue();
			fieldsUsedInValue = null;
			if (flag)
			{
				base.RenderingContext.OdpContext.ReportObjectModel.ResetFieldsUsedInExpression();
			}
			string result = image.EvaluateStringValueExpression(base.Instance.ReportScopeInstance, base.RenderingContext.OdpContext, out errOccurred);
			if (errOccurred)
			{
				return null;
			}
			if (flag)
			{
				fieldsUsedInValue = new List<string>();
				base.RenderingContext.OdpContext.ReportObjectModel.AddFieldsUsedInExpression(fieldsUsedInValue);
			}
			return result;
		}

		string IBaseImage.GetTransparentImageProperties(out string mimeType, out byte[] imageData)
		{
			InternalImageInstance internalImageInstance = this.ImageInstance as InternalImageInstance;
			Global.Tracer.Assert(internalImageInstance != null, "GetTransparentImageProperties may only be called from the ODP engine.");
			return internalImageInstance.LoadAndCacheTransparentImage(out mimeType, out imageData);
		}

		internal override void UpdateRenderReportItem(AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem)
		{
			base.UpdateRenderReportItem(renderReportItem);
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.Update(((AspNetCore.ReportingServices.ReportRendering.Image)base.m_renderReportItem).ActionInfo);
			}
		}

		internal override void SetNewContextChildren()
		{
			base.SetNewContextChildren();
			if (this.m_actionInfo != null)
			{
				this.m_actionInfo.SetNewContext();
			}
			if (this.m_tags != null)
			{
				this.m_tags.SetNewContext();
			}
		}

		internal override void ConstructReportItemDefinition()
		{
			base.ConstructReportItemDefinitionImpl();
			ImageInstance imageInstance = this.ImageInstance;
			Global.Tracer.Assert(imageInstance != null, "(instance != null)");
			if (imageInstance.MIMEType != null)
			{
				this.ImageDef.MIMEType = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateConstExpression(imageInstance.MIMEType);
			}
			else
			{
				this.ImageDef.MIMEType = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			this.m_mimeType = null;
			if (imageInstance.ImageData != null || imageInstance.StreamName != null)
			{
				Global.Tracer.Assert(false, "Runtime construction of images with constant Image.Value is not supported.");
			}
			else
			{
				this.ImageDef.Value = AspNetCore.ReportingServices.ReportIntermediateFormat.ExpressionInfo.CreateEmptyExpression();
			}
			this.m_value = null;
			if (!this.ActionInfo.ConstructActionDefinition())
			{
				this.ImageDef.Action = null;
				this.m_actionInfo = null;
			}
		}

		internal override void CompleteCriGeneratedInstanceEvaluation()
		{
			Global.Tracer.Assert(base.CriGenerationPhase == CriGenerationPhases.Instance, "(CriGenerationPhase == CriGenerationPhases.Instance)");
			ImageInstance imageInstance = this.ImageInstance;
			Global.Tracer.Assert(imageInstance != null, "(instance != null)");
			if (imageInstance.ActionInfoWithDynamicImageMapAreas != null)
			{
				base.CriGenerationPhase = CriGenerationPhases.Definition;
				imageInstance.ActionInfoWithDynamicImageMapAreas.ConstructDefinitions();
				base.CriGenerationPhase = CriGenerationPhases.Instance;
			}
		}
	}
}
