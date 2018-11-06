using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Models;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLColumnModel : IColumnModel, IOoxmlCtWrapperModel
	{
		private readonly ColumnProperties _interface;

		private readonly CT_Col _col;

		private int _colnumber;

		public ColumnProperties Interface
		{
			get
			{
				return this._interface;
			}
		}

		public double Width
		{
			set
			{
				double num = this.ConvertPointsToCharacterWidths(value);
				if (num < 0.0)
				{
					num = 0.0;
				}
				else if (num > 255.0)
				{
					num = 255.0;
				}
				this._col.Width_Attr = num;
				this._col.CustomWidth_Attr = true;
			}
		}

		public bool Hidden
		{
			set
			{
				this._col.Hidden_Attr = value;
			}
		}

		public int OutlineLevel
		{
			set
			{
				if (value >= 0 && value <= 7)
				{
					this._col.OutlineLevel_Attr = (byte)value;
					return;
				}
				throw new FatalException();
			}
		}

		public bool OutlineCollapsed
		{
			set
			{
				this._col.Collapsed_Attr = value;
			}
		}

		public OoxmlComplexType OoxmlTag
		{
			get
			{
				return this._col;
			}
		}

		public XMLColumnModel(CT_Col col, int number)
		{
			this._col = col;
			this._colnumber = number;
			this._interface = new ColumnProperties(this);
			this._col.Width_Attr = 9.140625;
		}

		private double ConvertPointsToCharacterWidths(double points)
		{
			double num = points * 96.0 / 72.0;
			double num2 = 7.0;
			double num3 = Math.Truncate((num - 5.0) / num2 * 100.0 + 0.5) / 100.0;
			return Math.Truncate((num3 * num2 + 5.0) / num2 * 256.0) / 256.0;
		}

		public void Cleanup()
		{
			this._col.Min_Attr = (uint)this._colnumber;
			this._col.Max_Attr = (uint)this._colnumber;
		}
	}
}
