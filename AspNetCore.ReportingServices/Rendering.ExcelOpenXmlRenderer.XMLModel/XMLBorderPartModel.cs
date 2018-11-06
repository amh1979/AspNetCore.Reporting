using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLBorderPartModel : IBorderPartModel, IDeepCloneable<XMLBorderPartModel>
	{
		private readonly CT_BorderPr _part;

		private readonly XMLPaletteModel _palette;

		private bool _hasBeenModified;

		public bool HasBeenModified
		{
			get
			{
				return this._hasBeenModified;
			}
		}

		public virtual ColorModel Color
		{
			get
			{
				if (this.Part.Color == null)
				{
					this.Part.Color = new CT_Color();
					this.Part.Color.Theme_Attr = 1u;
				}
				return this._palette.GetColorFromCT(this.Part.Color);
			}
			set
			{
				if (value != null)
				{
					this.Part.Color = ((XMLColorModel)value).Data;
					this._hasBeenModified = true;
				}
				else
				{
					this.Part.Color = null;
				}
			}
		}

		public virtual ST_BorderStyle Style
		{
			get
			{
				return this.Part.Style_Attr;
			}
			set
			{
				this.Part.Style_Attr = value;
				this._hasBeenModified = true;
			}
		}

		public CT_BorderPr Part
		{
			get
			{
				return this._part;
			}
		}

		public XMLBorderPartModel(CT_BorderPr part, XMLPaletteModel palette)
		{
			this._part = part;
			this._palette = palette;
		}

		public XMLBorderPartModel DeepClone()
		{
			XMLBorderPartModel xMLBorderPartModel = new XMLBorderPartModel(new CT_BorderPr(), this._palette);
			if (this.Part.Color != null)
			{
				xMLBorderPartModel.Part.Color = this._palette.GetColorFromCT(this.Part.Color).Clone().Data;
			}
			if (this.Part.Style_Attr != (ST_BorderStyle)null)
			{
				xMLBorderPartModel.Part.Style_Attr = this.Part.Style_Attr;
			}
			xMLBorderPartModel._hasBeenModified = this._hasBeenModified;
			return xMLBorderPartModel;
		}

		public override bool Equals(object o)
		{
			if (!(o is XMLBorderPartModel))
			{
				return false;
			}
			XMLBorderPartModel xMLBorderPartModel = (XMLBorderPartModel)o;
			if (this.HasBeenModified != xMLBorderPartModel.HasBeenModified)
			{
				return false;
			}
			if (this.Part.Style_Attr != xMLBorderPartModel.Part.Style_Attr)
			{
				return false;
			}
			if (!this._palette.GetColorFromCT(this.Part.Color).Equals(this._palette.GetColorFromCT(xMLBorderPartModel.Part.Color)))
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int num = 0;
			num ^= this.Part.Style_Attr.GetHashCode();
			return num ^ this._palette.GetColorFromCT(this.Part.Color).GetHashCode();
		}
	}
}
