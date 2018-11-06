using System;
using System.Drawing.Printing;

namespace AspNetCore.Reporting
{
	[Serializable]
	internal sealed class ReportPageSettings
	{
		private int m_pageWidth;

		private int m_pageHeight;

		private Margins m_margins;

		public PaperSize PaperSize
		{
			get
			{
				PageSettings customPageSettings = this.CustomPageSettings;
				ReportPageSettings.UpdatePageSettingsForPrinter(customPageSettings, new PrinterSettings());
				return customPageSettings.PaperSize;
			}
		}

		public Margins Margins
		{
			get
			{
				return (Margins)this.m_margins.Clone();
			}
		}

		public bool IsLandscape
		{
			get
			{
				return this.m_pageWidth > this.m_pageHeight;
			}
		}

		internal PageSettings CustomPageSettings
		{
			get
			{
				PageSettings pageSettings = new PageSettings();
				int width = Math.Min(this.m_pageWidth, this.m_pageHeight);
				int height = Math.Max(this.m_pageWidth, this.m_pageHeight);
				pageSettings.PaperSize = new PaperSize("", width, height);
				pageSettings.Landscape = this.IsLandscape;
				pageSettings.Margins = this.Margins;
				return pageSettings;
			}
		}

		internal ReportPageSettings(double pageHeight, double pageWidth, double leftMargin, double rightMargin, double topMargin, double bottomMargin)
		{
			this.m_pageWidth = ReportPageSettings.ConvertMmTo100thInch(pageWidth);
			this.m_pageHeight = ReportPageSettings.ConvertMmTo100thInch(pageHeight);
			this.m_margins = new Margins(ReportPageSettings.ConvertMmTo100thInch(leftMargin), ReportPageSettings.ConvertMmTo100thInch(rightMargin), ReportPageSettings.ConvertMmTo100thInch(topMargin), ReportPageSettings.ConvertMmTo100thInch(bottomMargin));
		}

		internal ReportPageSettings()
			: this(1100.0, 850.0, 50.0, 50.0, 50.0, 50.0)
		{
		}

		private static int ConvertMmTo100thInch(double mm)
		{
			return (int)Math.Round(mm / 25.4 * 100.0);
		}

		internal static void UpdatePageSettingsForPrinter(PageSettings pageSettings, PrinterSettings printerSettings)
		{
			if (printerSettings.IsValid)
			{
				int num = pageSettings.Landscape ? pageSettings.PaperSize.Height : pageSettings.PaperSize.Width;
				int num2 = pageSettings.Landscape ? pageSettings.PaperSize.Width : pageSettings.PaperSize.Height;
				foreach (PaperSize paperSize in printerSettings.PaperSizes)
				{
					if (num == paperSize.Width && num2 == paperSize.Height)
					{
						pageSettings.Landscape = false;
						pageSettings.PaperSize = paperSize;
						break;
					}
					if (num == paperSize.Height && num2 == paperSize.Width)
					{
						pageSettings.Landscape = true;
						pageSettings.PaperSize = paperSize;
						break;
					}
				}
				pageSettings.PrinterSettings = printerSettings;
			}
		}

		internal PageSettings ToPageSettings(PrinterSettings currentPrinter)
		{
			PageSettings customPageSettings = this.CustomPageSettings;
			ReportPageSettings.UpdatePageSettingsForPrinter(customPageSettings, currentPrinter);
			customPageSettings.Margins = this.Margins;
			return customPageSettings;
		}
	}
}
