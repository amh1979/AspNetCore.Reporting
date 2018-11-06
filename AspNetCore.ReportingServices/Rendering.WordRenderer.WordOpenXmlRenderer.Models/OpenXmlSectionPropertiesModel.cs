using AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Parser.wordprocessingml.x2006.main;
using System.IO;

namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlSectionPropertiesModel
	{
		internal interface IHeaderFooterReferences
		{
			string Header
			{
				get;
			}

			string Footer
			{
				get;
			}

			string FirstPageHeader
			{
				get;
			}

			string FirstPageFooter
			{
				get;
			}
		}

		private CT_SectPr _sectionPr;

		internal float Height
		{
			set
			{
				this.PageSize().H_Attr = WordOpenXmlUtils.ToTwips(value, 144f, 31680f);
			}
		}

		internal float Width
		{
			set
			{
				this.PageSize().W_Attr = WordOpenXmlUtils.ToTwips(value, 144f, 31680f);
			}
		}

		internal bool IsLandscape
		{
			set
			{
				if (value)
				{
					this.PageSize().Orient_Attr = ST_Orientation.landscape;
				}
			}
		}

		internal float BottomMargin
		{
			set
			{
				this.PageMargins().Bottom_Attr = WordOpenXmlUtils.ToTwips(value, 0f, 31680f);
			}
		}

		internal float LeftMargin
		{
			set
			{
				this.PageMargins().Left_Attr = WordOpenXmlUtils.ToTwips(value, 0f, 31680f);
			}
		}

		internal float RightMargin
		{
			set
			{
				this.PageMargins().Right_Attr = WordOpenXmlUtils.ToTwips(value, 0f, 31680f);
			}
		}

		internal float TopMargin
		{
			set
			{
				this.PageMargins().Top_Attr = WordOpenXmlUtils.ToTwips(value, 0f, 31680f);
			}
		}

		public bool Continuous
		{
			set
			{
				if (value)
				{
					this._sectionPr.Type = new CT_SectType
					{
						Val_Attr = ST_SectionMark.continuous
					};
				}
				else
				{
					this._sectionPr.Type = null;
				}
			}
		}

		public bool HasTitlePage
		{
			set
			{
				if (value)
				{
					this._sectionPr.TitlePg = new CT_OnOff();
				}
				else
				{
					this._sectionPr.TitlePg = null;
				}
			}
		}

		public CT_SectPr CtSectPr
		{
			get
			{
				return this._sectionPr;
			}
		}

		internal OpenXmlSectionPropertiesModel()
		{
			this._sectionPr = new CT_SectPr();
		}

		private CT_PageSz PageSize()
		{
			if (this._sectionPr.PgSz == null)
			{
				this._sectionPr.PgSz = new CT_PageSz();
			}
			return this._sectionPr.PgSz;
		}

		private CT_PageMar PageMargins()
		{
			if (this._sectionPr.PgMar == null)
			{
				this._sectionPr.PgMar = new CT_PageMar();
			}
			return this._sectionPr.PgMar;
		}

		internal void AddHeaderId(string value)
		{
			if (value != null)
			{
				this._sectionPr.EG_HdrFtrReferencess.Add(new CT_HdrRef
				{
					Id_Attr = value,
					Type_Attr = ST_HdrFtr._default
				});
			}
		}

		internal void AddFirstPageHeaderId(string value)
		{
			if (value != null)
			{
				this._sectionPr.EG_HdrFtrReferencess.Add(new CT_HdrRef
				{
					Id_Attr = value,
					Type_Attr = ST_HdrFtr.first
				});
			}
		}

		internal void AddFooterId(string value)
		{
			if (value != null)
			{
				this._sectionPr.EG_HdrFtrReferencess.Add(new CT_FtrRef
				{
					Id_Attr = value,
					Type_Attr = ST_HdrFtr._default
				});
			}
		}

		internal void AddFirstPageFooterId(string value)
		{
			if (value != null)
			{
				this._sectionPr.EG_HdrFtrReferencess.Add(new CT_FtrRef
				{
					Id_Attr = value,
					Type_Attr = ST_HdrFtr.first
				});
			}
		}

		public void SetHeaderFooterReferences(IHeaderFooterReferences headerFooterReferences)
		{
			this.AddHeaderId(headerFooterReferences.Header);
			this.AddFooterId(headerFooterReferences.Footer);
			this.AddFirstPageHeaderId(headerFooterReferences.FirstPageHeader);
			this.AddFirstPageFooterId(headerFooterReferences.FirstPageFooter);
		}

		public void ResetHeadersAndFooters()
		{
			this._sectionPr.EG_HdrFtrReferencess.Clear();
		}

		public void WriteToBody(TextWriter writer, IHeaderFooterReferences headerFooterReferences)
		{
			this.SetHeaderFooterReferences(headerFooterReferences);
			this._sectionPr.Write(writer, CT_Body.SectPrElementName);
			this.ResetHeadersAndFooters();
		}
	}
}
