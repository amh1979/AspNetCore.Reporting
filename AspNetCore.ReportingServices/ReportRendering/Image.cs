using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportProcessing;
using System.Collections.Specialized;
using System.Globalization;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class Image : ReportItem, IImage, IDeepCloneable
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

		private ImageBase m_internalImage;

		private ActionInfo m_actionInfo;

		private ImageMapAreasCollection m_imageMap;

		public byte[] ImageData
		{
			get
			{
				if (base.IsCustomControl)
				{
					return this.Processing.m_imageData;
				}
				return this.Rendering.ImageData;
			}
			set
			{
				if (!base.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.m_imageData = value;
			}
		}

		public string MIMEType
		{
			get
			{
				if (base.IsCustomControl)
				{
					return this.Processing.m_mimeType;
				}
				return this.Rendering.MIMEType;
			}
			set
			{
				if (!base.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				if (value == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "MimeType");
				}
				if (!Validator.ValidateMimeType(value))
				{
					throw new ReportRenderingException(ErrorCode.rrInvalidMimeType, value);
				}
				this.Processing.m_mimeType = value;
			}
		}

		public string StreamName
		{
			get
			{
				if (base.IsCustomControl)
				{
					return null;
				}
				return this.Rendering.StreamName;
			}
		}

		public ReportUrl HyperLinkURL
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = this.ActionInfo;
				}
				if (actionInfo != null)
				{
					return actionInfo.Actions[0].HyperLinkURL;
				}
				return null;
			}
		}

		public ReportUrl DrillthroughReport
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = this.ActionInfo;
				}
				if (actionInfo != null)
				{
					return actionInfo.Actions[0].DrillthroughReport;
				}
				return null;
			}
		}

		public NameValueCollection DrillthroughParameters
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = this.ActionInfo;
				}
				if (actionInfo != null)
				{
					return actionInfo.Actions[0].DrillthroughParameters;
				}
				return null;
			}
		}

		public string BookmarkLink
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (actionInfo == null)
				{
					actionInfo = this.ActionInfo;
				}
				if (actionInfo != null)
				{
					return actionInfo.Actions[0].BookmarkLink;
				}
				return null;
			}
		}

		public ActionInfo ActionInfo
		{
			get
			{
				ActionInfo actionInfo = this.m_actionInfo;
				if (!base.IsCustomControl && actionInfo == null)
				{
					AspNetCore.ReportingServices.ReportProcessing.Action action = ((AspNetCore.ReportingServices.ReportProcessing.Image)base.ReportItemDef).Action;
					if (action != null)
					{
						AspNetCore.ReportingServices.ReportProcessing.ActionInstance actionInstance = null;
						string ownerUniqueName = base.UniqueName;
						if (base.ReportItemInstance != null)
						{
							actionInstance = ((ImageInstanceInfo)base.InstanceInfo).Action;
							if (base.RenderingContext.InPageSection)
							{
								ownerUniqueName = base.ReportItemInstance.UniqueName.ToString(CultureInfo.InvariantCulture);
							}
						}
						else if (base.RenderingContext.InPageSection && base.m_intUniqueName != 0)
						{
							ownerUniqueName = base.m_intUniqueName.ToString(CultureInfo.InvariantCulture);
						}
						actionInfo = new ActionInfo(action, actionInstance, ownerUniqueName, base.RenderingContext);
						if (base.RenderingContext.CacheState)
						{
							this.m_actionInfo = actionInfo;
						}
					}
				}
				return actionInfo;
			}
			set
			{
				if (!base.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.m_actionInfo = value;
			}
		}

		public Sizings Sizing
		{
			get
			{
				if (base.IsCustomControl)
				{
					return this.Processing.m_sizing;
				}
				return (Sizings)((AspNetCore.ReportingServices.ReportProcessing.Image)base.ReportItemDef).Sizing;
			}
			set
			{
				if (!base.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.Processing.m_sizing = value;
			}
		}

		public ImageMapAreasCollection ImageMap
		{
			get
			{
				if (base.IsCustomControl)
				{
					return this.m_imageMap;
				}
				ImageMapAreasCollection imageMapAreasCollection = this.m_imageMap;
				if (this.m_imageMap == null && this.Rendering.ImageMapAreaInstances != null)
				{
					imageMapAreasCollection = new ImageMapAreasCollection(this.Rendering.ImageMapAreaInstances, base.RenderingContext);
					if (base.RenderingContext.CacheState)
					{
						this.m_imageMap = imageMapAreasCollection;
					}
				}
				return imageMapAreasCollection;
			}
			set
			{
				if (!base.IsCustomControl)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				this.m_imageMap = value;
			}
		}

		private InternalImage Rendering
		{
			get
			{
				Global.Tracer.Assert(!base.IsCustomControl);
				InternalImage internalImage = this.m_internalImage as InternalImage;
				if (internalImage == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return internalImage;
			}
		}

		internal new ImageProcessing Processing
		{
			get
			{
				Global.Tracer.Assert(base.IsCustomControl);
				ImageProcessing imageProcessing = this.m_internalImage as ImageProcessing;
				if (imageProcessing == null)
				{
					throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
				}
				return imageProcessing;
			}
		}

		public Image(string definitionName, string instanceName)
			: base(definitionName, instanceName)
		{
			if (definitionName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "definitionName");
			}
			if (instanceName == null)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidParameterValue, "instanceName");
			}
			Global.Tracer.Assert(base.IsCustomControl && null != base.Processing);
			this.m_internalImage = new ImageProcessing();
		}

		internal Image(string uniqueName, int intUniqueName, AspNetCore.ReportingServices.ReportProcessing.Image reportItemDef, AspNetCore.ReportingServices.ReportProcessing.ImageInstance reportItemInstance, RenderingContext renderingContext)
			: base(uniqueName, intUniqueName, reportItemDef, reportItemInstance, renderingContext)
		{
			ImageInstanceInfo imageInstanceInfo = (ImageInstanceInfo)base.InstanceInfo;
			string mimeType = null;
			if (reportItemDef.Source == AspNetCore.ReportingServices.ReportProcessing.Image.SourceType.Database && reportItemDef.MIMEType.Type == ExpressionInfo.Types.Constant)
			{
				mimeType = reportItemDef.MIMEType.Value;
			}
			this.m_internalImage = new InternalImage((SourceType)reportItemDef.Source, mimeType, (imageInstanceInfo != null) ? imageInstanceInfo.ValueObject : reportItemDef.Value.Value, renderingContext, imageInstanceInfo != null && imageInstanceInfo.BrokenImage, (imageInstanceInfo != null) ? imageInstanceInfo.ImageMapAreas : null);
		}

		private Image()
		{
		}

		ReportItem IDeepCloneable.DeepClone()
		{
			if (!base.IsCustomControl)
			{
				throw new RenderingObjectModelException(ProcessingErrorCode.rsInvalidOperation);
			}
			Image image = new Image();
			base.DeepClone(image);
			Global.Tracer.Assert(this.m_internalImage != null && this.m_internalImage is ImageProcessing);
			image.m_internalImage = this.Processing.DeepClone();
			if (this.m_actionInfo != null)
			{
				image.m_actionInfo = this.m_actionInfo.DeepClone();
			}
			if (this.m_imageMap != null)
			{
				image.m_imageMap = this.m_imageMap.DeepClone();
			}
			return image;
		}
	}
}
