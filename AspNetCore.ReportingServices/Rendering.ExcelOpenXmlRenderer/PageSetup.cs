using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System.Collections;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal class PageSetup
	{
		internal sealed class PageOrientation
		{
			private readonly string mName;

			private readonly int mValue;

			public static readonly PageOrientation Landscape;

			public static readonly PageOrientation Portrait;

			public int Value
			{
				get
				{
					return this.mValue;
				}
			}

			static PageOrientation()
			{
				PageOrientation.Landscape = new PageOrientation("Landscape", 1);
				PageOrientation.Portrait = new PageOrientation("Portrait", 0);
			}

			private PageOrientation(string aName, int aValue)
			{
				this.mName = aName;
				this.mValue = aValue;
			}

			public override bool Equals(object aObject)
			{
				if (aObject is PageOrientation)
				{
					PageOrientation pageOrientation = (PageOrientation)aObject;
					return this.Value == pageOrientation.Value;
				}
				return false;
			}

			public override int GetHashCode()
			{
				return this.Value;
			}

			public override string ToString()
			{
				return this.mName;
			}
		}

		internal sealed class PagePaperSize
		{
			private static readonly Hashtable mCollection;

			private readonly string mName;

			private readonly int mValue;

			public static readonly PagePaperSize A3;

			public static readonly PagePaperSize A4;

			public static readonly PagePaperSize A4Small;

			public static readonly PagePaperSize A5;

			public static readonly PagePaperSize B4;

			public static readonly PagePaperSize B5;

			public static readonly PagePaperSize CSheet;

			public static readonly PagePaperSize Default;

			public static readonly PagePaperSize DSheet;

			public static readonly PagePaperSize Envelope10;

			public static readonly PagePaperSize Envelope11;

			public static readonly PagePaperSize Envelope12;

			public static readonly PagePaperSize Envelope14;

			public static readonly PagePaperSize Envelope9;

			public static readonly PagePaperSize EnvelopeB4;

			public static readonly PagePaperSize EnvelopeB5;

			public static readonly PagePaperSize EnvelopeB6;

			public static readonly PagePaperSize EnvelopeC3;

			public static readonly PagePaperSize EnvelopeC4;

			public static readonly PagePaperSize EnvelopeC5;

			public static readonly PagePaperSize EnvelopeC6;

			public static readonly PagePaperSize EnvelopeC65;

			public static readonly PagePaperSize EnvelopeDL;

			public static readonly PagePaperSize EnvelopeItaly;

			public static readonly PagePaperSize EnvelopeMonarch;

			public static readonly PagePaperSize EnvelopePersonal;

			public static readonly PagePaperSize ESheet;

			public static readonly PagePaperSize Executive;

			public static readonly PagePaperSize FanfoldLegalGerman;

			public static readonly PagePaperSize FanfoldStdGerman;

			public static readonly PagePaperSize FanfoldUS;

			public static readonly PagePaperSize Folio;

			public static readonly PagePaperSize Ledger;

			public static readonly PagePaperSize Legal;

			public static readonly PagePaperSize Letter;

			public static readonly PagePaperSize LetterSmall;

			public static readonly PagePaperSize Note;

			public static readonly PagePaperSize Paper10x14;

			public static readonly PagePaperSize Paper11x17;

			public static readonly PagePaperSize Quarto;

			public static readonly PagePaperSize Statement;

			public static readonly PagePaperSize Tabloid;

			public static readonly PagePaperSize User;

			public int Value
			{
				get
				{
					return this.mValue;
				}
			}

			static PagePaperSize()
			{
				PagePaperSize.mCollection = Hashtable.Synchronized(new Hashtable());
				PagePaperSize.A3 = new PagePaperSize("A3", 8);
				PagePaperSize.A4 = new PagePaperSize("A4", 9);
				PagePaperSize.A4Small = new PagePaperSize("A4 Small", 10);
				PagePaperSize.A5 = new PagePaperSize("A5", 11);
				PagePaperSize.B4 = new PagePaperSize("B4", 12);
				PagePaperSize.B5 = new PagePaperSize("B5", 13);
				PagePaperSize.CSheet = new PagePaperSize("C Sheet", 24);
				PagePaperSize.Default = new PagePaperSize("Default", 0);
				PagePaperSize.DSheet = new PagePaperSize("D Sheet", 25);
				PagePaperSize.Envelope10 = new PagePaperSize("Envelope 10", 20);
				PagePaperSize.Envelope11 = new PagePaperSize("Envelope 11", 21);
				PagePaperSize.Envelope12 = new PagePaperSize("Envelope 12", 22);
				PagePaperSize.Envelope14 = new PagePaperSize("Envelope 14", 23);
				PagePaperSize.Envelope9 = new PagePaperSize("Envelope 9", 19);
				PagePaperSize.EnvelopeB4 = new PagePaperSize("Envelope B4", 33);
				PagePaperSize.EnvelopeB5 = new PagePaperSize("Envelope B5", 34);
				PagePaperSize.EnvelopeB6 = new PagePaperSize("Envelope B6", 35);
				PagePaperSize.EnvelopeC3 = new PagePaperSize("Envelope C3", 29);
				PagePaperSize.EnvelopeC4 = new PagePaperSize("Envelope C4", 30);
				PagePaperSize.EnvelopeC5 = new PagePaperSize("Envelope C5", 28);
				PagePaperSize.EnvelopeC6 = new PagePaperSize("Envelope C6", 31);
				PagePaperSize.EnvelopeC65 = new PagePaperSize("Envelope C65", 32);
				PagePaperSize.EnvelopeDL = new PagePaperSize("Envelope DL", 27);
				PagePaperSize.EnvelopeItaly = new PagePaperSize("Envelope Italy", 36);
				PagePaperSize.EnvelopeMonarch = new PagePaperSize("Envelope Monarch", 37);
				PagePaperSize.EnvelopePersonal = new PagePaperSize("Envelope Personal", 38);
				PagePaperSize.ESheet = new PagePaperSize("E Sheet", 26);
				PagePaperSize.Executive = new PagePaperSize("Executive", 7);
				PagePaperSize.FanfoldLegalGerman = new PagePaperSize("Fanfold Legal German", 41);
				PagePaperSize.FanfoldStdGerman = new PagePaperSize("Fanfold Standard German", 40);
				PagePaperSize.FanfoldUS = new PagePaperSize("Fanfold US", 39);
				PagePaperSize.Folio = new PagePaperSize("Folio", 14);
				PagePaperSize.Ledger = new PagePaperSize("Ledger", 4);
				PagePaperSize.Legal = new PagePaperSize("Legal", 5);
				PagePaperSize.Letter = new PagePaperSize("Letter", 1);
				PagePaperSize.LetterSmall = new PagePaperSize("Letter Small", 2);
				PagePaperSize.Note = new PagePaperSize("Note", 18);
				PagePaperSize.Paper10x14 = new PagePaperSize("10x14", 16);
				PagePaperSize.Paper11x17 = new PagePaperSize("11x17", 17);
				PagePaperSize.Quarto = new PagePaperSize("Quarto", 15);
				PagePaperSize.Statement = new PagePaperSize("Statement", 6);
				PagePaperSize.Tabloid = new PagePaperSize("Tabloid", 3);
				PagePaperSize.User = new PagePaperSize("User", 256);
			}

			private PagePaperSize(string aName, int aValue)
			{
				this.mName = aName;
				this.mValue = aValue;
				PagePaperSize.mCollection[base.GetType().FullName + this.mValue] = this;
			}

			public override bool Equals(object aObject)
			{
				if (aObject is PagePaperSize)
				{
					PagePaperSize pagePaperSize = (PagePaperSize)aObject;
					return this.Value == pagePaperSize.Value;
				}
				return false;
			}

			public static PagePaperSize findByValue(int aValue)
			{
				switch (aValue)
				{
				case 0:
					return PagePaperSize.Default;
				case 1:
					return PagePaperSize.Letter;
				case 2:
					return PagePaperSize.LetterSmall;
				case 3:
					return PagePaperSize.Tabloid;
				case 4:
					return PagePaperSize.Ledger;
				case 5:
					return PagePaperSize.Legal;
				case 6:
					return PagePaperSize.Statement;
				case 7:
					return PagePaperSize.Executive;
				case 8:
					return PagePaperSize.A3;
				case 9:
					return PagePaperSize.A4;
				case 10:
					return PagePaperSize.A4Small;
				case 11:
					return PagePaperSize.A5;
				case 12:
					return PagePaperSize.B4;
				case 13:
					return PagePaperSize.B5;
				case 14:
					return PagePaperSize.Folio;
				case 15:
					return PagePaperSize.Quarto;
				case 16:
					return PagePaperSize.Paper10x14;
				case 17:
					return PagePaperSize.Paper11x17;
				case 18:
					return PagePaperSize.Note;
				case 19:
					return PagePaperSize.Envelope9;
				case 20:
					return PagePaperSize.Envelope10;
				case 21:
					return PagePaperSize.Envelope11;
				case 22:
					return PagePaperSize.Envelope12;
				case 23:
					return PagePaperSize.Envelope14;
				case 24:
					return PagePaperSize.CSheet;
				case 25:
					return PagePaperSize.DSheet;
				case 26:
					return PagePaperSize.ESheet;
				case 27:
					return PagePaperSize.EnvelopeDL;
				case 28:
					return PagePaperSize.EnvelopeC5;
				case 29:
					return PagePaperSize.EnvelopeC3;
				case 30:
					return PagePaperSize.EnvelopeC4;
				case 31:
					return PagePaperSize.EnvelopeC6;
				case 32:
					return PagePaperSize.EnvelopeC65;
				case 33:
					return PagePaperSize.EnvelopeB4;
				case 34:
					return PagePaperSize.EnvelopeB5;
				case 35:
					return PagePaperSize.EnvelopeB6;
				case 36:
					return PagePaperSize.EnvelopeItaly;
				case 37:
					return PagePaperSize.EnvelopeMonarch;
				case 38:
					return PagePaperSize.EnvelopePersonal;
				case 39:
					return PagePaperSize.FanfoldUS;
				case 40:
					return PagePaperSize.FanfoldStdGerman;
				case 41:
					return PagePaperSize.FanfoldLegalGerman;
				case 256:
					return PagePaperSize.User;
				default:
					throw new FatalException();
				}
			}

			public override int GetHashCode()
			{
				return this.Value;
			}

			public override string ToString()
			{
				return this.mName;
			}
		}

		private readonly IPageSetupModel mModel;

		public double BottomMargin
		{
			set
			{
				this.mModel.BottomMargin = value;
			}
		}

		public string CenterFooter
		{
			set
			{
				this.mModel.CenterFooter = value;
			}
		}

		public string CenterHeader
		{
			set
			{
				this.mModel.CenterHeader = value;
			}
		}

		public double FooterMargin
		{
			set
			{
				this.mModel.FooterMargin = value;
			}
		}

		public double HeaderMargin
		{
			set
			{
				this.mModel.HeaderMargin = value;
			}
		}

		public string LeftFooter
		{
			set
			{
				this.mModel.LeftFooter = value;
			}
		}

		public string LeftHeader
		{
			set
			{
				this.mModel.LeftHeader = value;
			}
		}

		public double LeftMargin
		{
			set
			{
				this.mModel.LeftMargin = value;
			}
		}

		public PageOrientation Orientation
		{
			set
			{
				this.mModel.Orientation = value;
			}
		}

		public PagePaperSize PaperSize
		{
			set
			{
				this.mModel.PaperSize = value;
			}
		}

		public string RightFooter
		{
			set
			{
				this.mModel.RightFooter = value;
			}
		}

		public string RightHeader
		{
			set
			{
				this.mModel.RightHeader = value;
			}
		}

		public double RightMargin
		{
			set
			{
				this.mModel.RightMargin = value;
			}
		}

		public double TopMargin
		{
			set
			{
				this.mModel.TopMargin = value;
			}
		}

		public bool SummaryRowsBelow
		{
			set
			{
				this.mModel.SummaryRowsBelow = value;
			}
		}

		public bool SummaryColumnsRight
		{
			set
			{
				this.mModel.SummaryColumnsRight = value;
			}
		}

		internal PageSetup(IPageSetupModel model)
		{
			this.mModel = model;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is PageSetup)
			{
				if (obj == this)
				{
					return true;
				}
				PageSetup pageSetup = (PageSetup)obj;
				return pageSetup.mModel.Equals(this.mModel);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.mModel.GetHashCode();
		}

		public void SetPrintTitleToRows(int firstRow, int lastRow)
		{
			this.mModel.SetPrintTitleToRows(firstRow, lastRow);
		}
	}
}
