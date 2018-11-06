using AspNetCore.ReportingServices.ReportIntermediateFormat;
using AspNetCore.ReportingServices.ReportProcessing;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class InternalImageInstance : ImageInstance
	{
		private ActionInfoWithDynamicImageMapCollection m_actionInfoImageMapAreas;

		private readonly ImageDataHandler m_imageDataHandler;

		public override byte[] ImageData
		{
			get
			{
				return this.m_imageDataHandler.ImageData;
			}
			set
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
			}
		}

		public override string StreamName
		{
			get
			{
				return this.m_imageDataHandler.StreamName;
			}
			internal set
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
			}
		}

		public override string MIMEType
		{
			get
			{
				return this.m_imageDataHandler.MIMEType;
			}
			set
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		public override TypeCode TagDataType
		{
			get
			{
				if (base.ImageDef.Tags != null)
				{
					return ((ReportElementCollectionBase<Tag>)base.ImageDef.Tags)[0].Instance.DataType;
				}
				return TypeCode.Empty;
			}
		}

		public override object Tag
		{
			get
			{
				if (base.ImageDef.Tags != null)
				{
					return ((ReportElementCollectionBase<Tag>)base.ImageDef.Tags)[0].Instance.Value;
				}
				return null;
			}
		}

		internal override string ImageDataId
		{
			get
			{
				return this.m_imageDataHandler.ImageDataId;
			}
		}

		public override ActionInfoWithDynamicImageMapCollection ActionInfoWithDynamicImageMapAreas
		{
			get
			{
				if (this.m_actionInfoImageMapAreas == null)
				{
					this.m_actionInfoImageMapAreas = new ActionInfoWithDynamicImageMapCollection();
				}
				return this.m_actionInfoImageMapAreas;
			}
		}

		internal override bool IsNullImage
		{
			get
			{
				return this.m_imageDataHandler.IsNullImage;
			}
		}

		internal InternalImageInstance(Image reportItemDef)
			: base(reportItemDef)
		{
			this.m_imageDataHandler = ImageDataHandlerFactory.Create(base.m_reportElementDef, reportItemDef);
		}

		internal override List<string> GetFieldsUsedInValueExpression()
		{
			return this.m_imageDataHandler.FieldsUsedInValue;
		}

		public override ActionInfoWithDynamicImageMap CreateActionInfoWithDynamicImageMap()
		{
			throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_actionInfoImageMapAreas = null;
			this.m_imageDataHandler.ClearCache();
		}

		internal string LoadAndCacheTransparentImage(out string mimeType, out byte[] imageData)
		{
			return this.m_imageDataHandler.LoadAndCacheTransparentImage(out mimeType, out imageData);
		}
	}
}
