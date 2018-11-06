using AspNetCore.ReportingServices.OnDemandReportRendering;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace AspNetCore.ReportingServices.Rendering.HPBProcessing
{
	internal sealed class PaginationSettings
	{
		internal enum DeviceInfoTags
		{
			StartPage,
			EndPage,
			PageWidth,
			PageHeight,
			Columns,
			ColumnSpacing,
			MarginTop,
			MarginLeft,
			MarginBottom,
			MarginRight,
			DpiX,
			DpiY,
			OutputFormat,
			ReportItemPath,
			Unknown
		}

		internal enum FormatEncoding
		{
			BMP,
			EMF,
			EMFPLUS,
			GIF,
			JPEG,
			PNG,
			TIFF,
			PDF
		}

		public const double MINIMUMCOLUMNHEIGHT = 12.699999809265137;

		public const double MINIMUMCOLUMNWIDTH = 12.699999809265137;

		public const double DEFAULTCOLUMNSPACING = 12.699999809265137;

		public const int DEFAULTRESOLUTIONX = 96;

		public const int DEFAULTRESOLUTIONY = 96;

		private int m_startPage;

		private int m_endPage;

		private double m_physicalPageWidth = 215.9;

		private double m_physicalPageHeight = 279.4;

		private double m_usableWidth;

		private double m_usableHeight;

		private double m_currentColumnWidth;

		private double m_currentColumnHeight;

		private double m_marginTop;

		private double m_marginLeft;

		private double m_marginBottom;

		private double m_marginRight;

		private FormatEncoding m_outputFormat = FormatEncoding.TIFF;

		private string m_reportItemPath;

		private int m_dpiX = 96;

		private int m_dpiY = 96;

		private int m_dynamicImageDpiX = 96;

		private int m_dynamicImageDpiY = 96;

		private bool m_useGenericDefault;

		private bool m_useEmSquare;

		private int m_measureTextDpi = 96;

		private int m_measureImageDpiX = 96;

		private int m_measureImageDpiY = 96;

		private SectionPaginationSettings[] m_sectionPaginationSettings;

		private static Hashtable DeviceInfoTagLookup;

		public int StartPage
		{
			get
			{
				return this.m_startPage;
			}
			set
			{
				this.m_startPage = value;
			}
		}

		public int EndPage
		{
			get
			{
				return this.m_endPage;
			}
			set
			{
				this.m_endPage = value;
			}
		}

		private string PageHeightStr
		{
			set
			{
				this.m_physicalPageHeight = this.ParseSize(value, 279.4);
			}
		}

		private string PageWidthStr
		{
			set
			{
				this.m_physicalPageWidth = this.ParseSize(value, 215.9);
			}
		}

		internal SectionPaginationSettings[] SectionPaginationSettings
		{
			get
			{
				return this.m_sectionPaginationSettings;
			}
		}

		internal double MarginTop
		{
			get
			{
				return this.m_marginTop;
			}
			set
			{
				this.m_marginTop = value;
			}
		}

		internal double MarginLeft
		{
			get
			{
				return this.m_marginLeft;
			}
			set
			{
				this.m_marginLeft = value;
			}
		}

		internal double MarginBottom
		{
			get
			{
				return this.m_marginBottom;
			}
			set
			{
				this.m_marginBottom = value;
			}
		}

		internal double MarginRight
		{
			get
			{
				return this.m_marginRight;
			}
			set
			{
				this.m_marginRight = value;
			}
		}

		public int DpiX
		{
			get
			{
				return this.m_dpiX;
			}
			set
			{
				this.m_dpiX = value;
			}
		}

		public int DpiY
		{
			get
			{
				return this.m_dpiY;
			}
			set
			{
				this.m_dpiY = value;
			}
		}

		public int DynamicImageDpiX
		{
			get
			{
				return this.m_dynamicImageDpiX;
			}
			set
			{
				this.m_dynamicImageDpiX = value;
			}
		}

		public int DynamicImageDpiY
		{
			get
			{
				return this.m_dynamicImageDpiY;
			}
			set
			{
				this.m_dynamicImageDpiY = value;
			}
		}

		public bool UseGenericDefault
		{
			get
			{
				return this.m_useGenericDefault;
			}
			set
			{
				this.m_useGenericDefault = value;
			}
		}

		public double CurrentColumnHeight
		{
			get
			{
				return this.m_currentColumnHeight;
			}
			set
			{
				this.m_currentColumnHeight = value;
			}
		}

		public double CurrentColumnWidth
		{
			get
			{
				return this.m_currentColumnWidth;
			}
			set
			{
				this.m_currentColumnWidth = value;
			}
		}

		public double UsablePageHeight
		{
			get
			{
				return this.m_usableHeight;
			}
		}

		public double UsablePageWidth
		{
			get
			{
				return this.m_usableWidth;
			}
		}

		public double PhysicalPageHeight
		{
			get
			{
				return this.m_physicalPageHeight;
			}
		}

		public double PhysicalPageWidth
		{
			get
			{
				return this.m_physicalPageWidth;
			}
		}

		public FormatEncoding OutputFormat
		{
			get
			{
				return this.m_outputFormat;
			}
		}

		public string ReportItemPath
		{
			get
			{
				return this.m_reportItemPath;
			}
			set
			{
				this.m_reportItemPath = value;
			}
		}

		public bool UseEmSquare
		{
			get
			{
				return this.m_useEmSquare;
			}
			set
			{
				this.m_useEmSquare = value;
			}
		}

		public int MeasureTextDpi
		{
			get
			{
				return this.m_measureTextDpi;
			}
			set
			{
				this.m_measureTextDpi = value;
			}
		}

		public int MeasureImageDpiX
		{
			get
			{
				return this.m_measureImageDpiX;
			}
			set
			{
				this.m_measureImageDpiX = value;
			}
		}

		public int MeasureImageDpiY
		{
			get
			{
				return this.m_measureImageDpiY;
			}
			set
			{
				this.m_measureImageDpiY = value;
			}
		}

		internal bool EMFOutputFormat
		{
			get
			{
				if (this.m_outputFormat != FormatEncoding.EMF && this.m_outputFormat != FormatEncoding.EMFPLUS)
				{
					return false;
				}
				return true;
			}
		}

		static PaginationSettings()
		{
			string[] names = Enum.GetNames(typeof(DeviceInfoTags));
			PaginationSettings.DeviceInfoTagLookup = CollectionsUtil.CreateCaseInsensitiveHashtable(names.Length);
			for (int i = 0; i < names.Length; i++)
			{
				PaginationSettings.DeviceInfoTagLookup.Add(names[i], Enum.Parse(typeof(DeviceInfoTags), names[i]));
			}
		}

		private void Init(AspNetCore.ReportingServices.OnDemandReportRendering.Report report)
		{
			Page page = ((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)report.ReportSections)[0].Page;
			this.PageHeightStr = page.PageHeight.ToString();
			this.PageWidthStr = page.PageWidth.ToString();
			this.MarginTop = page.TopMargin.ToMillimeters();
			this.MarginLeft = page.LeftMargin.ToMillimeters();
			this.MarginBottom = page.BottomMargin.ToMillimeters();
			this.MarginRight = page.RightMargin.ToMillimeters();
			int count = report.ReportSections.Count;
			this.m_sectionPaginationSettings = new SectionPaginationSettings[count];
			for (int i = 0; i < count; i++)
			{
				this.m_sectionPaginationSettings[i] = new SectionPaginationSettings(((ReportElementCollectionBase<AspNetCore.ReportingServices.OnDemandReportRendering.ReportSection>)report.ReportSections)[i]);
			}
		}

		public PaginationSettings(AspNetCore.ReportingServices.OnDemandReportRendering.Report report)
		{
			this.Init(report);
			this.ValidateFields(0, -1.0);
		}

		public PaginationSettings(AspNetCore.ReportingServices.OnDemandReportRendering.Report report, NameValueCollection aDeviceInfo)
		{
			this.Init(report);
			int columns = default(int);
			double columnSpacing = default(double);
			this.ParseDeviceInfo(aDeviceInfo, out columns, out columnSpacing);
			this.ValidateFields(columns, columnSpacing);
			this.m_dynamicImageDpiX = this.m_dpiX;
			this.m_dynamicImageDpiY = this.m_dpiY;
		}

		private void ValidateDeviceInfoValue(ref double currValue, double defaultValue)
		{
			if (currValue <= 0.0)
			{
				currValue = defaultValue;
			}
		}

		private void ValidateDeviceInfoValue(ref int currValue, int defaultValue)
		{
			if (currValue <= 0)
			{
				currValue = defaultValue;
			}
		}

		private void ValidateFields(int columns, double columnSpacing)
		{
			if (this.m_startPage < 0)
			{
				this.m_startPage = 0;
			}
			if (this.m_endPage < 0)
			{
				this.m_endPage = 0;
			}
			if (this.m_endPage < this.m_startPage)
			{
				this.m_endPage = this.m_startPage;
			}
			else if (this.m_endPage > this.m_startPage && this.m_startPage == 0)
			{
				this.m_startPage = 1;
			}
			this.ValidateDeviceInfoValue(ref this.m_physicalPageWidth, 215.9);
			this.ValidateDeviceInfoValue(ref this.m_physicalPageHeight, 279.4);
			if (this.m_marginTop < 0.0)
			{
				this.m_marginTop = 0.0;
			}
			if (this.m_marginBottom < 0.0)
			{
				this.m_marginBottom = 0.0;
			}
			if (this.m_marginTop + this.m_marginBottom >= this.m_physicalPageHeight)
			{
				this.m_marginTop = (this.m_marginBottom = 0.0);
			}
			if (this.m_marginLeft < 0.0)
			{
				this.m_marginLeft = 0.0;
			}
			if (this.m_marginRight < 0.0)
			{
				this.m_marginRight = 0.0;
			}
			if (this.m_marginLeft + this.m_marginRight >= this.m_physicalPageWidth)
			{
				this.m_marginLeft = (this.m_marginRight = 0.0);
			}
			this.ValidateDeviceInfoValue(ref this.m_dpiX, 96);
			this.ValidateDeviceInfoValue(ref this.m_dpiY, 96);
			double physicalPageWidth = this.PhysicalPageWidth;
			double physicalPageHeight = this.PhysicalPageHeight;
			SectionPaginationSettings[] sectionPaginationSettings = this.m_sectionPaginationSettings;
			foreach (SectionPaginationSettings sectionPaginationSettings2 in sectionPaginationSettings)
			{
				sectionPaginationSettings2.Validate(this, columns, columnSpacing, ref physicalPageHeight, ref physicalPageWidth);
			}
			this.m_physicalPageWidth = physicalPageWidth;
			this.m_physicalPageHeight = physicalPageHeight;
			this.m_usableWidth = physicalPageWidth - this.MarginLeft - this.MarginRight;
			this.m_usableHeight = physicalPageHeight - this.MarginTop - this.MarginBottom;
			SectionPaginationSettings[] sectionPaginationSettings3 = this.m_sectionPaginationSettings;
			foreach (SectionPaginationSettings sectionPaginationSettings4 in sectionPaginationSettings3)
			{
				sectionPaginationSettings4.SetColumnArea(this);
			}
		}

		private void ParseDeviceInfo(NameValueCollection deviceInfo, out int columns, out double columnSpacing)
		{
			int count = deviceInfo.Count;
			columns = 0;
			columnSpacing = -1.0;
			for (int i = 0; i < count; i++)
			{
				string key = deviceInfo.GetKey(i);
				string text = deviceInfo.Get(i);
				object obj = PaginationSettings.DeviceInfoTagLookup[key];
				switch ((obj != null) ? ((DeviceInfoTags)obj) : DeviceInfoTags.Unknown)
				{
				case DeviceInfoTags.StartPage:
					this.m_startPage = this.ParseInt(text, 0);
					break;
				case DeviceInfoTags.EndPage:
					this.m_endPage = this.ParseInt(text, 0);
					break;
				case DeviceInfoTags.PageWidth:
					this.m_physicalPageWidth = this.ParseSize(text, this.m_physicalPageWidth);
					break;
				case DeviceInfoTags.PageHeight:
					this.m_physicalPageHeight = this.ParseSize(text, this.m_physicalPageHeight);
					break;
				case DeviceInfoTags.Columns:
					columns = this.ParseInt(text, columns);
					break;
				case DeviceInfoTags.ColumnSpacing:
					columnSpacing = this.ParseSize(text, columnSpacing);
					break;
				case DeviceInfoTags.MarginTop:
					this.m_marginTop = this.ParseSize(text, this.m_marginTop);
					break;
				case DeviceInfoTags.MarginLeft:
					this.m_marginLeft = this.ParseSize(text, this.m_marginLeft);
					break;
				case DeviceInfoTags.MarginBottom:
					this.m_marginBottom = this.ParseSize(text, this.m_marginBottom);
					break;
				case DeviceInfoTags.MarginRight:
					this.m_marginRight = this.ParseSize(text, this.m_marginRight);
					break;
				case DeviceInfoTags.DpiX:
					this.m_dpiX = this.ParseInt(text, 96);
					break;
				case DeviceInfoTags.DpiY:
					this.m_dpiY = this.ParseInt(text, 96);
					break;
				case DeviceInfoTags.OutputFormat:
					this.m_outputFormat = this.ParseFormat(text, FormatEncoding.TIFF);
					break;
				case DeviceInfoTags.ReportItemPath:
					this.m_reportItemPath = text;
					break;
				}
			}
		}

		internal int ParseInt(string intValue, int defaultValue)
		{
			int result = defaultValue;
			if (!string.IsNullOrEmpty(intValue))
			{
				try
				{
					result = int.Parse(intValue, CultureInfo.InvariantCulture);
					return result;
				}
				catch (FormatException)
				{
					return result;
				}
			}
			return result;
		}

		private double ParseSize(string sizeValue, double defaultValue)
		{
			double result = defaultValue;
			ReportSize reportSize = default(ReportSize);
			if (sizeValue != null && sizeValue.Length > 0 && ReportSize.TryParse(sizeValue, out reportSize))
			{
				result = reportSize.ToMillimeters();
			}
			return result;
		}

		private FormatEncoding ParseFormat(string enumValue, FormatEncoding defaultValue)
		{
			string text = enumValue.Trim().ToUpperInvariant();
			switch (text)
			{
			case "BMP":
				return FormatEncoding.BMP;
			case "EMF":
				return FormatEncoding.EMF;
			case "EMFPLUS":
				return FormatEncoding.EMFPLUS;
			case "GIF":
				return FormatEncoding.GIF;
			case "JPEG":
				return FormatEncoding.JPEG;
			case "PNG":
				return FormatEncoding.PNG;
			case "TIFF":
				return FormatEncoding.TIFF;
			default:
				return defaultValue;
			}
		}
	}
}
