using AspNetCore.ReportingServices.Common;
using System.Globalization;

namespace AspNetCore.ReportingServices.Rendering.RPLProcessing
{
	internal sealed class RPLReportSize
	{
		private string m_size;

		private double m_sizeInMM;

		public RPLReportSize(string size)
		{
			this.m_size = size;
			this.ParseSize();
		}

		public RPLReportSize(double sizeInMM)
		{
			this.m_size = sizeInMM.ToString(CultureInfo.InvariantCulture) + "mm";
			this.m_sizeInMM = sizeInMM;
		}

		public override string ToString()
		{
			return this.m_size;
		}

		public double ToMillimeters()
		{
			return this.m_sizeInMM;
		}

		public double ToInches()
		{
			return this.m_sizeInMM / 25.4;
		}

		public double ToPoints()
		{
			return this.m_sizeInMM / 0.3528;
		}

		public double ToCentimeters()
		{
			return this.m_sizeInMM / 10.0;
		}

		internal void ParseSize()
		{
			this.m_sizeInMM = RVUnit.Parse(this.m_size, CultureInfo.InvariantCulture).ToMillimeters();
		}
	}
}
