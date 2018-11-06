using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimImageInstance : ImageInstance
	{
		private ActionInfoWithDynamicImageMapCollection m_actionInfoImageMapAreas;

		public override byte[] ImageData
		{
			get
			{
				return ((AspNetCore.ReportingServices.ReportRendering.Image)base.m_reportElementDef.RenderReportItem).ImageData;
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
				return ((AspNetCore.ReportingServices.ReportRendering.Image)base.m_reportElementDef.RenderReportItem).StreamName;
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
				return ((AspNetCore.ReportingServices.ReportRendering.Image)base.m_reportElementDef.RenderReportItem).MIMEType;
			}
			set
			{
				throw new RenderingObjectModelException(RPRes.rsErrorDuringROMWriteback);
			}
		}

		internal override string ImageDataId
		{
			get
			{
				return this.StreamName;
			}
		}

		public override TypeCode TagDataType
		{
			get
			{
				return TypeCode.Empty;
			}
		}

		public override object Tag
		{
			get
			{
				return null;
			}
		}

		public override ActionInfoWithDynamicImageMapCollection ActionInfoWithDynamicImageMapAreas
		{
			get
			{
				if (this.m_actionInfoImageMapAreas == null && ((AspNetCore.ReportingServices.ReportRendering.Image)base.m_reportElementDef.RenderReportItem).ImageMap != null && 0 < ((AspNetCore.ReportingServices.ReportRendering.Image)base.m_reportElementDef.RenderReportItem).ImageMap.Count)
				{
					this.m_actionInfoImageMapAreas = new ActionInfoWithDynamicImageMapCollection(base.m_reportElementDef.RenderingContext, ((AspNetCore.ReportingServices.ReportRendering.Image)base.m_reportElementDef.RenderReportItem).ImageMap);
				}
				return this.m_actionInfoImageMapAreas;
			}
		}

		internal override bool IsNullImage
		{
			get
			{
				return false;
			}
		}

		internal ShimImageInstance(Image reportItemDef)
			: base(reportItemDef)
		{
		}

		internal override List<string> GetFieldsUsedInValueExpression()
		{
			return null;
		}

		public override ActionInfoWithDynamicImageMap CreateActionInfoWithDynamicImageMap()
		{
			throw new RenderingObjectModelException(RPRes.rsErrorDuringROMDefinitionWriteback);
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
			this.m_actionInfoImageMapAreas = null;
		}
	}
}
