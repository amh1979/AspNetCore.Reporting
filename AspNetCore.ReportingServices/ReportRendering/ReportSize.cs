using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.OnDemandReportRendering;
using AspNetCore.ReportingServices.ReportProcessing;

namespace AspNetCore.ReportingServices.ReportRendering
{
	internal sealed class ReportSize
	{
		private string m_size;

		private double m_sizeInMM;

		private bool m_parsed;

		internal bool Parsed
		{
			get
			{
				return this.m_parsed;
			}
		}

		public ReportSize(string size)
		{
			this.m_size = size;
			this.Validate();
			this.m_parsed = true;
		}

		internal ReportSize(string size, double sizeInMM)
		{
			this.m_size = size;
			this.m_sizeInMM = sizeInMM;
			this.m_parsed = true;
		}

		internal ReportSize(ReportSize newSize)
		{
			this.m_size = newSize.ToString();
			this.m_sizeInMM = newSize.ToMillimeters();
			this.m_parsed = true;
		}

		internal ReportSize(string size, bool parsed)
		{
			this.m_size = size;
			this.m_parsed = parsed;
		}

		private ReportSize()
		{
		}

		public override string ToString()
		{
			return this.m_size;
		}

		public double ToMillimeters()
		{
			this.ParseSize();
			return this.m_sizeInMM;
		}

		public double ToInches()
		{
			this.ParseSize();
			return this.m_sizeInMM / 25.4;
		}

		public double ToPoints()
		{
			this.ParseSize();
			return this.m_sizeInMM / 0.3528;
		}

		public double ToCentimeters()
		{
			this.ParseSize();
			return this.m_sizeInMM / 10.0;
		}

		internal void ParseSize()
		{
			if (!this.m_parsed)
			{
				Validator.ParseSize(this.m_size, out this.m_sizeInMM);
				this.m_parsed = true;
			}
		}

		internal void Validate()
		{
			RVUnit rVUnit = default(RVUnit);
			if (!Validator.ValidateSizeString(this.m_size, out rVUnit))
			{
				throw new ReportRenderingException(ErrorCode.rrInvalidSize, this.m_size);
			}
			if (!Validator.ValidateSizeUnitType(rVUnit))
			{
				throw new ReportRenderingException(ErrorCode.rrInvalidMeasurementUnit, this.m_size);
			}
			if (!Validator.ValidateSizeIsPositive(rVUnit))
			{
				throw new ReportRenderingException(ErrorCode.rrNegativeSize, this.m_size);
			}
			double sizeInMM = Converter.ConvertToMM(rVUnit);
			if (!Validator.ValidateSizeValue(sizeInMM, Validator.NormalMin, Validator.NormalMax))
			{
				throw new ReportRenderingException(ErrorCode.rrOutOfRange, this.m_size);
			}
			this.m_sizeInMM = sizeInMM;
		}

		internal ReportSize DeepClone()
		{
			ReportSize reportSize = new ReportSize();
			if (this.m_size != null)
			{
				reportSize.m_size = string.Copy(this.m_size);
			}
			reportSize.m_parsed = this.m_parsed;
			reportSize.m_sizeInMM = this.m_sizeInMM;
			return reportSize;
		}
	}
}
