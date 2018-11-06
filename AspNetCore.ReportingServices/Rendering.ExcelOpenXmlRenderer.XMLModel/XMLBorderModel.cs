using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLBorderModel : IBorderModel, IDeepCloneable<XMLBorderModel>
	{
		private readonly CT_Border _border;

		private readonly XMLPaletteModel _palette;

		private XMLBorderPartModel _topBorder;

		private XMLBorderPartModel _bottomBorder;

		private XMLBorderPartModel _leftBorder;

		private XMLBorderPartModel _rightBorder;

		private XMLBorderPartModel _diagonalBorder;

		public CT_Border Data
		{
			get
			{
				return this._border;
			}
		}

		public bool HasBeenModified
		{
			get
			{
				if (!this._topBorder.HasBeenModified && !this._bottomBorder.HasBeenModified && !this._leftBorder.HasBeenModified && !this._rightBorder.HasBeenModified)
				{
					return this._diagonalBorder.HasBeenModified;
				}
				return true;
			}
		}

		public XMLBorderPartModel TopBorder
		{
			get
			{
				return this._topBorder;
			}
		}

		public XMLBorderPartModel BottomBorder
		{
			get
			{
				return this._bottomBorder;
			}
		}

		public XMLBorderPartModel LeftBorder
		{
			get
			{
				return this._leftBorder;
			}
		}

		public XMLBorderPartModel RightBorder
		{
			get
			{
				return this._rightBorder;
			}
		}

		public XMLBorderPartModel DiagonalBorder
		{
			get
			{
				return this._diagonalBorder;
			}
		}

		public ExcelBorderPart DiagonalPartDirection
		{
			set
			{
				switch (value)
				{
				case ExcelBorderPart.DiagonalUp:
					this._border.DiagonalDown_Attr = false;
					this._border.DiagonalUp_Attr = true;
					break;
				case ExcelBorderPart.DiagonalDown:
					this._border.DiagonalDown_Attr = true;
					this._border.DiagonalUp_Attr = false;
					break;
				case ExcelBorderPart.DiagonalBoth:
					this._border.DiagonalDown_Attr = true;
					this._border.DiagonalUp_Attr = true;
					break;
				}
			}
		}

		public XMLBorderModel(XMLPaletteModel palette)
		{
			this._border = new CT_Border();
			this._border.Top = new CT_BorderPr();
			this._border.Bottom = new CT_BorderPr();
			this._border.Left = new CT_BorderPr();
			this._border.Right = new CT_BorderPr();
			this._border.Diagonal = new CT_BorderPr();
			this._palette = palette;
			this.InitBorderPartModels();
		}

		public XMLBorderModel(CT_Border border, XMLPaletteModel palette)
		{
			this._border = border;
			this._palette = palette;
			this.InitBorderPartModels();
		}

		private void InitBorderPartModels()
		{
			this._topBorder = new XMLBorderPartModel(this._border.Top, this._palette);
			this._bottomBorder = new XMLBorderPartModel(this._border.Bottom, this._palette);
			this._leftBorder = new XMLBorderPartModel(this._border.Left, this._palette);
			this._rightBorder = new XMLBorderPartModel(this._border.Right, this._palette);
			this._diagonalBorder = new XMLBorderPartModel(this._border.Diagonal, this._palette);
		}

		public XMLBorderModel DeepClone()
		{
			XMLBorderModel xMLBorderModel = new XMLBorderModel(this._palette);
			CT_Border border = this._border;
			CT_Border border2 = xMLBorderModel._border;
			if (border.DiagonalDown_Attr_Is_Specified)
			{
				border2.DiagonalDown_Attr = border.DiagonalDown_Attr;
			}
			if (border.DiagonalUp_Attr_Is_Specified)
			{
				border2.DiagonalUp_Attr = border.DiagonalUp_Attr;
			}
			border2.Outline_Attr = border.Outline_Attr;
			xMLBorderModel._bottomBorder = this._bottomBorder.DeepClone();
			border2.Bottom = xMLBorderModel._bottomBorder.Part;
			xMLBorderModel._topBorder = this._topBorder.DeepClone();
			border2.Top = xMLBorderModel._topBorder.Part;
			xMLBorderModel._leftBorder = this._leftBorder.DeepClone();
			border2.Left = xMLBorderModel._leftBorder.Part;
			xMLBorderModel._rightBorder = this._rightBorder.DeepClone();
			border2.Right = xMLBorderModel._rightBorder.Part;
			xMLBorderModel._diagonalBorder = this._diagonalBorder.DeepClone();
			border2.Diagonal = xMLBorderModel._diagonalBorder.Part;
			return xMLBorderModel;
		}

		public override bool Equals(object o)
		{
			if (!(o is XMLBorderModel))
			{
				return false;
			}
			XMLBorderModel xMLBorderModel = (XMLBorderModel)o;
			if (new XMLBorderPartModel(this._border.Bottom, this._palette).Equals(new XMLBorderPartModel(xMLBorderModel._border.Bottom, this._palette)) && new XMLBorderPartModel(this._border.Diagonal, this._palette).Equals(new XMLBorderPartModel(xMLBorderModel._border.Diagonal, this._palette)) && new XMLBorderPartModel(this._border.Left, this._palette).Equals(new XMLBorderPartModel(xMLBorderModel._border.Left, this._palette)) && new XMLBorderPartModel(this._border.Right, this._palette).Equals(new XMLBorderPartModel(xMLBorderModel._border.Right, this._palette)) && new XMLBorderPartModel(this._border.Top, this._palette).Equals(new XMLBorderPartModel(xMLBorderModel._border.Top, this._palette)) && (bool)(this._border.DiagonalDown_Attr == xMLBorderModel._border.DiagonalDown_Attr) && (bool)(this._border.DiagonalUp_Attr == xMLBorderModel._border.DiagonalUp_Attr))
			{
				return this._border.Outline_Attr == xMLBorderModel._border.Outline_Attr;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int num = 0;
			num ^= this._border.DiagonalDown_Attr.GetHashCode();
			num ^= this._border.DiagonalUp_Attr.GetHashCode();
			num ^= this._border.Outline_Attr.GetHashCode();
			num ^= new XMLBorderPartModel(this._border.Bottom, this._palette).GetHashCode();
			num ^= new XMLBorderPartModel(this._border.Top, this._palette).GetHashCode();
			num ^= new XMLBorderPartModel(this._border.Left, this._palette).GetHashCode();
			num ^= new XMLBorderPartModel(this._border.Right, this._palette).GetHashCode();
			return num ^ new XMLBorderPartModel(this._border.Diagonal, this._palette).GetHashCode();
		}
	}
}
