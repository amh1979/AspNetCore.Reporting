using AspNetCore.ReportingServices.Rendering.RPLProcessing;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer
{
	internal class SectionEntry
	{
		internal string SectionId;

		internal RPLItemMeasurement HeaderMeasurement;

		internal RPLItemMeasurement FooterMeasurement;

		public SectionEntry(RPLReportSection section)
		{
			this.SectionId = section.ID;
			this.HeaderMeasurement = section.Header;
			this.FooterMeasurement = section.Footer;
		}
	}
}
