using AspNetCore.ReportingServices.OnDemandReportRendering;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal class SectionPaginationSettings
	{
		private double m_columnSpacing = 12.699999809265137;

		private int m_columns = 1;

		private double m_headerHeight;

		private double m_footerHeight;

		private double m_columnWidth = 12.699999809265137;

		public double ColumnSpacing
		{
			get
			{
				return this.m_columnSpacing;
			}
		}

		public int Columns
		{
			get
			{
				return this.m_columns;
			}
		}

		public double ColumnSpacingWidth
		{
			get
			{
				return (double)(this.m_columns - 1) * this.m_columnSpacing;
			}
		}

		public double ColumnWidth
		{
			get
			{
				return this.m_columnWidth;
			}
		}

		public double HeaderHeight
		{
			get
			{
				return this.m_headerHeight;
			}
		}

		public double FooterHeight
		{
			get
			{
				return this.m_footerHeight;
			}
		}

		public SectionPaginationSettings(AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection reportSection)
		{
			this.m_columns = reportSection.Page.Columns;
			this.m_columnSpacing = reportSection.Page.ColumnSpacing.ToMillimeters();
			if (reportSection.Page.PageHeader != null)
			{
				this.m_headerHeight = reportSection.Page.PageHeader.Height.ToMillimeters();
			}
			if (reportSection.Page.PageFooter != null)
			{
				this.m_footerHeight = reportSection.Page.PageFooter.Height.ToMillimeters();
			}
		}

		public void Validate(PaginationSettings deviceInfoSettings, int columns, double columnSpacing, ref double pageHeight, ref double pageWidth)
		{
			if (columns > 0)
			{
				this.m_columns = columns;
			}
			if (columnSpacing >= 0.0)
			{
				this.m_columnSpacing = columnSpacing;
			}
			double num = (double)this.m_columns * 12.699999809265137;
			double num2 = num + (double)(this.m_columns - 1) * this.m_columnSpacing + deviceInfoSettings.MarginLeft + deviceInfoSettings.MarginRight;
			if (num2 > pageWidth)
			{
				this.m_columnSpacing = 0.0;
				deviceInfoSettings.MarginLeft = 0.0;
				deviceInfoSettings.MarginRight = 0.0;
				if (num > pageWidth)
				{
					pageWidth = num;
				}
			}
			double num3 = 12.699999809265137 + this.m_headerHeight + this.m_footerHeight;
			double num4 = pageHeight - (deviceInfoSettings.MarginTop + deviceInfoSettings.MarginBottom);
			if (num3 > num4)
			{
				deviceInfoSettings.MarginTop = 0.0;
				deviceInfoSettings.MarginBottom = 0.0;
				if (num3 > pageHeight)
				{
					pageHeight = num3;
				}
			}
		}

		public void SetColumnArea(PaginationSettings deviceInfoSettings)
		{
			double num = (double)(this.m_columns - 1) * this.m_columnSpacing;
			double num2 = deviceInfoSettings.UsablePageWidth - num;
			this.m_columnWidth = num2 / (double)this.m_columns;
		}
	}
}
