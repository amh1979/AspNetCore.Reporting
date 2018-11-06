using AspNetCore.ReportingServices.ReportRendering;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ShimParagraphInstance : ParagraphInstance
	{
		public override string UniqueName
		{
			get
			{
				if (base.m_uniqueName == null)
				{
					AspNetCore.ReportingServices.ReportRendering.ReportItem renderReportItem = base.m_reportElementDef.RenderReportItem;
					base.m_uniqueName = renderReportItem.ID + 'x' + "0" + 'i' + renderReportItem.UniqueName;
				}
				return base.m_uniqueName;
			}
		}

		public override bool IsCompiled
		{
			get
			{
				return false;
			}
		}

		internal ShimParagraphInstance(Paragraph paragraphDef)
			: base(paragraphDef)
		{
		}

		protected override void ResetInstanceCache()
		{
			base.ResetInstanceCache();
		}
	}
}
