using AspNetCore.ReportingServices.Diagnostics.Utilities;
using System.Globalization;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.SPBProcessing
{
	internal class ReportSectionHelper
	{
		private PaginationInfoItems m_type = PaginationInfoItems.ReportSection;

		private int m_sectionIndex = -1;

		private PageItemHelper m_bodyHelper;

		internal PaginationInfoItems Type
		{
			get
			{
				return this.m_type;
			}
		}

		internal PageItemHelper BodyHelper
		{
			get
			{
				return this.m_bodyHelper;
			}
			set
			{
				this.m_bodyHelper = value;
			}
		}

		internal int SectionIndex
		{
			get
			{
				return this.m_sectionIndex;
			}
			set
			{
				this.m_sectionIndex = value;
			}
		}

		internal ReportSectionHelper()
		{
		}

		internal static ReportSectionHelper ReadReportSection(BinaryReader reader, long offsetEndPage)
		{
			if (reader != null && offsetEndPage > 0)
			{
				long position = reader.BaseStream.Position;
				ReportSectionHelper reportSectionHelper = null;
				byte b = reader.ReadByte();
				if (b == 16)
				{
					reportSectionHelper = new ReportSectionHelper();
					ReportSectionHelper.ReadReportSectionProperties(reportSectionHelper, reader, offsetEndPage);
					if (reader.BaseStream.Position > offsetEndPage)
					{
						throw new InvalidDataException(SPBRes.InvalidPaginationStream);
					}
					return reportSectionHelper;
				}
				throw new InvalidDataException(SPBRes.InvalidTokenPaginationItems(b.ToString("x", CultureInfo.InvariantCulture)));
			}
			return null;
		}

		private static void ReadReportSectionProperties(ReportSectionHelper section, BinaryReader reader, long offsetEndPage)
		{
			RSTrace.RenderingTracer.Assert(section != null, "The section helper is null.");
			RSTrace.RenderingTracer.Assert(reader != null, "The pagination stream is null.");
			RSTrace.RenderingTracer.Assert(offsetEndPage > 0, "The pagination stream is corrupt.");
			byte b = reader.ReadByte();
			if (b == 23)
			{
				section.SectionIndex = reader.ReadInt32();
				section.BodyHelper = PageItemHelper.ReadItems(reader, offsetEndPage);
				b = reader.ReadByte();
				if (reader.BaseStream.Position <= offsetEndPage)
				{
					return;
				}
				throw new InvalidDataException(SPBRes.InvalidPaginationStream);
			}
			throw new InvalidDataException(SPBRes.InvalidPaginationStream);
		}
	}
}
