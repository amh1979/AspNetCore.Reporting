using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Globalization;
using System.Text;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLPageSetupModel : IPageSetupModel
	{
		private readonly PageSetup _interface;

		private readonly CT_Worksheet _sheet;

		private readonly XMLWorksheetModel _sheetModel;

		private string _rightHeader;

		private string _centerHeader;

		private string _leftHeader;

		private string _rightFooter;

		private string _centerFooter;

		private string _leftFooter;

		private bool _useZoom;

		public CT_Worksheet BackingSheet
		{
			get
			{
				return this._sheet;
			}
		}

		public PageSetup Interface
		{
			get
			{
				return this._interface;
			}
		}

		public PageSetup.PageOrientation Orientation
		{
			set
			{
				if (value == PageSetup.PageOrientation.Landscape)
				{
					this._sheet.PageSetup.Orientation_Attr = ST_Orientation.landscape;
					return;
				}
				if (value == PageSetup.PageOrientation.Portrait)
				{
					this._sheet.PageSetup.Orientation_Attr = ST_Orientation.portrait;
					return;
				}
				throw new FatalException();
			}
		}

		public PageSetup.PagePaperSize PaperSize
		{
			set
			{
				this._sheet.PageSetup.PaperSize_Attr = (uint)value.Value;
			}
		}

		public double LeftMargin
		{
			set
			{
				XMLPageSetupModel.TryValidMargin(value);
				this._sheet.PageMargins.Left_Attr = value;
			}
		}

		public double RightMargin
		{
			set
			{
				XMLPageSetupModel.TryValidMargin(value);
				this._sheet.PageMargins.Right_Attr = value;
			}
		}

		public double TopMargin
		{
			set
			{
				XMLPageSetupModel.TryValidMargin(value);
				this._sheet.PageMargins.Top_Attr = value;
			}
		}

		public double BottomMargin
		{
			set
			{
				XMLPageSetupModel.TryValidMargin(value);
				this._sheet.PageMargins.Bottom_Attr = value;
			}
		}

		public double FooterMargin
		{
			set
			{
				XMLPageSetupModel.TryValidMargin(value);
				this._sheet.PageMargins.Footer_Attr = value;
			}
		}

		public double HeaderMargin
		{
			set
			{
				XMLPageSetupModel.TryValidMargin(value);
				this._sheet.PageMargins.Header_Attr = value;
			}
		}

		public string RightFooter
		{
			set
			{
				XMLPageSetupModel.TryValidHeaderFooter(this._leftFooter, this._centerFooter, value);
				this._rightFooter = value;
			}
		}

		public string CenterFooter
		{
			set
			{
				XMLPageSetupModel.TryValidHeaderFooter(this._leftFooter, value, this._rightFooter);
				this._centerFooter = value;
			}
		}

		public string LeftFooter
		{
			set
			{
				XMLPageSetupModel.TryValidHeaderFooter(value, this._centerFooter, this._rightFooter);
				this._leftFooter = value;
			}
		}

		public string RightHeader
		{
			set
			{
				XMLPageSetupModel.TryValidHeaderFooter(this._leftHeader, this._centerHeader, value);
				this._rightHeader = value;
			}
		}

		public string CenterHeader
		{
			set
			{
				XMLPageSetupModel.TryValidHeaderFooter(this._leftHeader, value, this._rightHeader);
				this._centerHeader = value;
			}
		}

		public string LeftHeader
		{
			set
			{
				XMLPageSetupModel.TryValidHeaderFooter(value, this._centerHeader, this._rightHeader);
				this._leftHeader = value;
			}
		}

		public bool UseZoom
		{
			get
			{
				return this._useZoom;
			}
			set
			{
				this._useZoom = value;
			}
		}

		public bool SummaryRowsBelow
		{
			set
			{
				this.GetOutlineProperties().SummaryBelow_Attr = value;
			}
		}

		public bool SummaryColumnsRight
		{
			set
			{
				this.GetOutlineProperties().SummaryRight_Attr = value;
			}
		}

		public XMLPageSetupModel(CT_Worksheet sheet, XMLWorksheetModel sheetModel)
		{
			if (sheet != null && sheetModel != null)
			{
				this._sheet = sheet;
				this._sheetModel = sheetModel;
				this._interface = new PageSetup(this);
				if (this._sheet.PageMargins == null)
				{
					this.SetDefaultMargins();
				}
				if (this._sheet.PageSetup == null)
				{
					this.SetDefaultPageSetup();
				}
				this.SetupUseZoom();
				return;
			}
			throw new FatalException();
		}

		private void SetDefaultMargins()
		{
			if (this._sheet.PageMargins == null)
			{
				this._sheet.PageMargins = new CT_PageMargins();
			}
			this._sheet.PageMargins.Bottom_Attr = 0.75;
			this._sheet.PageMargins.Footer_Attr = 0.3;
			this._sheet.PageMargins.Header_Attr = 0.3;
			this._sheet.PageMargins.Left_Attr = 0.7;
			this._sheet.PageMargins.Right_Attr = 0.7;
			this._sheet.PageMargins.Top_Attr = 0.75;
		}

		private void SetDefaultPageSetup()
		{
			if (this._sheet.PageSetup == null)
			{
				this._sheet.PageSetup = new CT_PageSetup();
			}
			this._sheet.PageSetup.Orientation_Attr = XMLConstants.DefaultWorksheetOrientation;
			this._sheet.PageSetup.HorizontalDpi_Attr = 300u;
			this._sheet.PageSetup.VerticalDpi_Attr = 300u;
		}

		private void SetupUseZoom()
		{
			if (this._sheet.PageSetup.FitToHeight_Attr != 1 || this._sheet.PageSetup.FitToWidth_Attr != 1)
			{
				this.UseZoom = false;
			}
			else if (this._sheet.PageSetup.Scale_Attr != 100)
			{
				this.UseZoom = true;
			}
			else
			{
				this.UseZoom = true;
			}
		}

		private static void TryValidMargin(double margin)
		{
			if (!(margin < 0.0) && !(margin >= 49.0))
			{
				return;
			}
			throw new FatalException();
		}

		private static void TryValidHeaderFooter(string left, string center, string right)
		{
			int num = 0;
			string[] array = new string[3]
			{
				left,
				center,
				right
			};
			foreach (string text in array)
			{
				if (!string.IsNullOrEmpty(text))
				{
					num += 2;
					num += text.Length;
				}
			}
			if (num <= 255)
			{
				return;
			}
			throw new FatalException();
		}

		private CT_OutlinePr GetOutlineProperties()
		{
			if (this._sheet.SheetPr == null)
			{
				this._sheet.SheetPr = new CT_SheetPr();
			}
			if (this._sheet.SheetPr.OutlinePr == null)
			{
				this._sheet.SheetPr.OutlinePr = new CT_OutlinePr();
			}
			return this._sheet.SheetPr.OutlinePr;
		}

		public void SetPrintTitleToRows(int firstRow, int lastRow)
		{
			XMLDefinedName xMLDefinedName = this._sheetModel.NameManager.CreateDefinedName("_xlnm.Print_Titles");
			xMLDefinedName.Content = string.Format(CultureInfo.InvariantCulture, "'{0}'!${1}:${2}", this._sheetModel.Name.Replace("'", "''"), firstRow + 1, lastRow + 1);
			xMLDefinedName.SheetIndex = this._sheetModel.Position;
		}

		private static string BuildHeaderFooter(string left, string center, string right)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(left))
			{
				stringBuilder.AppendFormat("&L{0}", left);
			}
			if (!string.IsNullOrEmpty(center))
			{
				stringBuilder.AppendFormat("&C{0}", center);
			}
			if (!string.IsNullOrEmpty(right))
			{
				stringBuilder.AppendFormat("&R{0}", right);
			}
			return stringBuilder.ToString();
		}

		public void Cleanup()
		{
			if (this._leftHeader != null || this._centerHeader != null || this._rightHeader != null)
			{
				if (this._sheet.HeaderFooter == null)
				{
					this._sheet.HeaderFooter = new CT_HeaderFooter();
				}
				this._sheet.HeaderFooter.OddHeader = XMLPageSetupModel.BuildHeaderFooter(this._leftHeader, this._centerHeader, this._rightHeader);
			}
			if (this._leftFooter != null || this._centerFooter != null || this._rightFooter != null)
			{
				if (this._sheet.HeaderFooter == null)
				{
					this._sheet.HeaderFooter = new CT_HeaderFooter();
				}
				this._sheet.HeaderFooter.OddFooter = XMLPageSetupModel.BuildHeaderFooter(this._leftFooter, this._centerFooter, this._rightFooter);
			}
			if (this.UseZoom)
			{
				this._sheet.PageSetup.FitToHeight_Attr = 1u;
				this._sheet.PageSetup.FitToWidth_Attr = 1u;
			}
			else
			{
				if (this._sheet.SheetPr == null)
				{
					this._sheet.SheetPr = new CT_SheetPr();
				}
				this._sheet.PageSetup.Scale_Attr = 100u;
			}
		}
	}
}
