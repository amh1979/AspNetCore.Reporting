using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal abstract class ImageInstance : ReportItemInstance, IImageInstance
	{
		public abstract byte[] ImageData
		{
			get;
			set;
		}

		public abstract string StreamName
		{
			get;
			internal set;
		}

		public abstract string MIMEType
		{
			get;
			set;
		}

		public abstract ActionInfoWithDynamicImageMapCollection ActionInfoWithDynamicImageMapAreas
		{
			get;
		}

		internal abstract bool IsNullImage
		{
			get;
		}

		public abstract TypeCode TagDataType
		{
			get;
		}

		public abstract object Tag
		{
			get;
		}

		internal abstract string ImageDataId
		{
			get;
		}

		internal Image ImageDef
		{
			get
			{
				return (Image)base.m_reportElementDef;
			}
		}

		internal ImageInstance(Image reportItemDef)
			: base(reportItemDef)
		{
		}

		internal abstract List<string> GetFieldsUsedInValueExpression();

		public abstract ActionInfoWithDynamicImageMap CreateActionInfoWithDynamicImageMap();
	}
}
