using AspNetCore.ReportingServices.OnDemandReportRendering;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal sealed class DocumentMapLabels : InteractivityChunks
	{
		internal DocumentMapLabels(Stream stream, int page)
			: base(stream, page)
		{
		}

		internal void WriteDocMapLabel(ReportItemInstance itemInstance)
		{
			if (itemInstance != null && itemInstance.DocumentMapLabel != null)
			{
				base.m_writer.Write((byte)1);
				base.m_writer.Write(itemInstance.UniqueName);
				base.m_writer.Write(base.m_page);
				base.m_writer.Write((byte)4);
			}
		}

		internal void WriteDocMapLabel(GroupInstance groupInstance)
		{
			if (groupInstance != null && groupInstance.DocumentMapLabel != null)
			{
				base.m_writer.Write((byte)1);
				base.m_writer.Write(groupInstance.UniqueName);
				base.m_writer.Write(base.m_page);
				base.m_writer.Write((byte)4);
			}
		}

		internal void WriteDocMapRootLabel(string rootLabelUniqueName)
		{
			base.m_writer.Write((byte)1);
			base.m_writer.Write(rootLabelUniqueName);
			base.m_writer.Write(base.m_page);
			base.m_writer.Write((byte)4);
		}
	}
}
