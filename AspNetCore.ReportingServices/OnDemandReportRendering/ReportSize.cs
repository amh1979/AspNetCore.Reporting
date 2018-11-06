using AspNetCore.ReportingServices.Common;
using AspNetCore.ReportingServices.Diagnostics.Utilities;
using AspNetCore.ReportingServices.ReportProcessing;
using AspNetCore.ReportingServices.ReportPublishing;
using AspNetCore.ReportingServices.ReportRendering;
using System;
using System.Globalization;

namespace AspNetCore.ReportingServices.OnDemandReportRendering
{
	internal sealed class ReportSize
	{
		private const string m_zeroMM = "0mm";

		private string m_size;

		private double m_sizeInMM;

		private bool m_parsed;

		private bool m_allowNegative;

		public ReportSize(string size)
			: this(size, true, false)
		{
		}

		public ReportSize(string size, bool allowNegative)
			: this(size, true, allowNegative)
		{
		}

		internal ReportSize(string size, bool validate, bool allowNegative)
		{
			if (string.IsNullOrEmpty(size))
			{
				this.m_size = "0mm";
			}
			else
			{
				this.m_size = size;
			}
			this.m_allowNegative = allowNegative;
			if (validate)
			{
				this.Validate();
				this.m_parsed = true;
			}
			else
			{
				this.m_parsed = false;
			}
		}

		internal ReportSize(string size, double sizeInMM)
		{
			this.m_sizeInMM = sizeInMM;
			this.m_parsed = true;
			if (string.IsNullOrEmpty(size))
			{
				this.m_size = ReportSize.ConvertToMM(this.m_sizeInMM);
			}
			else
			{
				this.m_size = size;
			}
		}

		internal ReportSize(AspNetCore.ReportingServices.ReportRendering.ReportSize oldSize)
		{
			this.m_size = oldSize.ToString();
			this.m_parsed = oldSize.Parsed;
			if (this.m_parsed)
			{
				this.m_sizeInMM = oldSize.ToMillimeters();
			}
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
			return this.m_sizeInMM / 0.35277777777777775;
		}

		public double ToCentimeters()
		{
			this.ParseSize();
			return this.m_sizeInMM / 10.0;
		}

		public static ReportSize SumSizes(ReportSize size1, ReportSize size2)
		{
			if (size1 == null)
			{
				return size2;
			}
			if (size2 == null)
			{
				return size1;
			}
			double millimeters = size1.ToMillimeters() + size2.ToMillimeters();
			return ReportSize.FromMillimeters(millimeters);
		}

		public static ReportSize FromMillimeters(double millimeters)
		{
			return new ReportSize(ReportSize.ConvertToMM(millimeters), millimeters);
		}

		private static string ConvertToMM(double millimeters)
		{
			return Convert.ToString(millimeters, CultureInfo.InvariantCulture) + "mm";
		}

		internal void ParseSize()
		{
			if (!this.m_parsed)
			{
				AspNetCore.ReportingServices.ReportPublishing.Validator.ParseSize(this.m_size, out this.m_sizeInMM);
				this.m_parsed = true;
			}
		}

		internal void Validate()
		{
			RVUnit rVUnit = default(RVUnit);
			if (!AspNetCore.ReportingServices.ReportPublishing.Validator.ValidateSizeString(this.m_size, out rVUnit))
			{
				throw new RenderingObjectModelException(ErrorCode.rrInvalidSize, this.m_size);
			}
			if (!AspNetCore.ReportingServices.ReportPublishing.Validator.ValidateSizeUnitType(rVUnit))
			{
				throw new RenderingObjectModelException(ErrorCode.rrInvalidMeasurementUnit, this.m_size);
			}
			if (!this.m_allowNegative && !AspNetCore.ReportingServices.ReportPublishing.Validator.ValidateSizeIsPositive(rVUnit))
			{
				throw new RenderingObjectModelException(ErrorCode.rrNegativeSize, this.m_size);
			}
			double sizeInMM = AspNetCore.ReportingServices.ReportPublishing.Converter.ConvertToMM(rVUnit);
			if (!AspNetCore.ReportingServices.ReportPublishing.Validator.ValidateSizeValue(sizeInMM, this.m_allowNegative ? AspNetCore.ReportingServices.ReportPublishing.Validator.NegativeMin : AspNetCore.ReportingServices.ReportPublishing.Validator.NormalMin, AspNetCore.ReportingServices.ReportPublishing.Validator.NormalMax))
			{
				throw new RenderingObjectModelException(ErrorCode.rrOutOfRange, this.m_size);
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

		public static bool TryParse(string value, out ReportSize reportSize)
		{
			return ReportSize.TryParse(value, false, out reportSize);
		}

		public static bool TryParse(string value, bool allowNegative, out ReportSize reportSize)
		{
			double sizeInMM = default(double);
			if (AspNetCore.ReportingServices.ReportPublishing.Validator.ValidateSize(value, allowNegative, out sizeInMM))
			{
				reportSize = new ReportSize(value, sizeInMM);
				return true;
			}
			reportSize = null;
			return false;
		}
	}
}
