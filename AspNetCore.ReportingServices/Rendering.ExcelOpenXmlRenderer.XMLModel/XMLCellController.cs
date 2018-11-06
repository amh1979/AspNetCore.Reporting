using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;
using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Parser.spreadsheetml.x2006.main;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.Excel;
using AspNetCore.ReportingServices.Rendering.ExcelRenderer.ExcelGenerator.OXML;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.XMLModel
{
	internal class XMLCellController : ICellController
	{
		private readonly CT_Cell _cell;

		private readonly XMLWorkbookModel _workbook;

		private readonly XMLWorksheetModel _sheet;

		private readonly PartManager _manager;

		private XMLCharacterRunManager _charManager;

		private XMLStyleModel _style;

		private bool _holdsOwnStyle;

		public object Value
		{
			get
			{
				if (this._cell.T_Attr == ST_CellType.inlineStr)
				{
					if (this._cell.Is != null && this._cell.Is.R != null && this._cell.Is.R.Count > 0)
					{
						string text = "";
						{
							foreach (CT_RElt item in this._cell.Is.R)
							{
								text += item.T;
							}
							return text;
						}
					}
					if (this._cell.Is == null)
					{
						return null;
					}
					return this._cell.Is.T;
				}
				if (this._cell.V == null)
				{
					return null;
				}
				if (this._cell.T_Attr == ST_CellType.b)
				{
					string a = this._cell.V.ToUpper(CultureInfo.InvariantCulture).Trim();
					if (a == "TRUE")
					{
						return true;
					}
					if (a == "FALSE")
					{
						return false;
					}
					if (a == "1")
					{
						return true;
					}
					if (a == "0")
					{
						return false;
					}
					throw new FatalException();
				}
				if (this._cell.T_Attr == ST_CellType.d)
				{
					return DateTime.FromOADate(double.Parse(this._cell.V, CultureInfo.InvariantCulture));
				}
				if (this._cell.T_Attr == ST_CellType.n)
				{
					long num = default(long);
					if (long.TryParse(this._cell.V, out num))
					{
						return num;
					}
					double num2 = default(double);
					if (double.TryParse(this._cell.V, out num2))
					{
						return num2;
					}
					throw new FatalException();
				}
				return null;
			}
			set
			{
				this._cell.V = null;
				this._cell.Is = null;
				if (value == null)
				{
					this._cell.V = null;
				}
				else if (value is bool)
				{
					this._cell.T_Attr = ST_CellType.b;
					this._cell.V = (((bool)value) ? "1" : "0");
				}
				else if (value is DateTime)
				{
					this._cell.T_Attr = ST_CellType.n;
					double num = ((DateTime)value).ToOADate();
					if (num <= 60.0)
					{
						num -= 1.0;
					}
					this._cell.V = num.ToString(CultureInfo.InvariantCulture);
				}
				else if (value is ExcelErrorCode)
				{
					this._cell.T_Attr = ST_CellType.e;
					switch ((ExcelErrorCode)value)
					{
					case ExcelErrorCode.DivByZeroError:
						this._cell.V = "#DIV/0!";
						break;
					case ExcelErrorCode.NameError:
						this._cell.V = "#NAME?";
						break;
					case ExcelErrorCode.ValueError:
						this._cell.V = "#VALUE!";
						break;
					case ExcelErrorCode.NullError:
						this._cell.V = "#NULL!";
						break;
					case ExcelErrorCode.NumError:
						this._cell.V = "#NUM!";
						break;
					case ExcelErrorCode.RefError:
						this._cell.V = "#REF!";
						break;
					case ExcelErrorCode.NAError:
						this._cell.V = "#N/A";
						break;
					default:
						this._cell.V = "#VALUE!";
						break;
					}
				}
				else if (value is string)
				{
					this._cell.T_Attr = ST_CellType.inlineStr;
					this._cell.Is = new CT_Rst();
					this._cell.Is.T = value.ToString();
				}
				else if (value is sbyte || value is byte || value is char || value is short || value is ushort || value is int || value is uint || value is long || value is ulong || value is float || value is double || value is decimal)
				{
					this._cell.T_Attr = ST_CellType.n;
					if (value is float)
					{
						if (this.IsNotValidFloat((float)value))
						{
							this.SetDivByZeroError();
						}
						else
						{
							this._cell.V = ((float)value).ToString(CultureInfo.InvariantCulture);
						}
					}
					else if (value is double)
					{
						if (this.IsNotValidDouble((double)value))
						{
							this.SetDivByZeroError();
						}
						else
						{
							this._cell.V = ((double)value).ToString(CultureInfo.InvariantCulture);
						}
					}
					else if (value is decimal)
					{
						this._cell.V = ((decimal)value).ToString(CultureInfo.InvariantCulture);
					}
					else
					{
						this._cell.V = value.ToString();
					}
				}
				else
				{
					this.Value = value.ToString();
				}
			}
		}

		public Cell.CellValueType ValueType
		{
			get
			{
				if (this._cell.T_Attr == ST_CellType.b)
				{
					return Cell.CellValueType.Boolean;
				}
				if (this._cell.T_Attr == ST_CellType.d)
				{
					return Cell.CellValueType.Date;
				}
				if (this._cell.T_Attr == ST_CellType.e)
				{
					return Cell.CellValueType.Error;
				}
				if (this._cell.T_Attr == ST_CellType.inlineStr)
				{
					return Cell.CellValueType.Text;
				}
				if (this._cell.T_Attr == ST_CellType.n)
				{
					if (this._cell.V != null && this._cell.V.Contains("."))
					{
						return Cell.CellValueType.Double;
					}
					return Cell.CellValueType.Integer;
				}
				return Cell.CellValueType.Text;
			}
		}

		public IStyleModel Style
		{
			get
			{
				if (this._style == null)
				{
					this._style = this._manager.StyleSheet.CreateStyle(this._cell.S_Attr);
				}
				else if (!this._holdsOwnStyle)
				{
					this._style = this._manager.StyleSheet.CreateStyle(this._style);
				}
				this._holdsOwnStyle = true;
				return this._style;
			}
			set
			{
				this._style = (XMLStyleModel)value;
				this._holdsOwnStyle = false;
			}
		}

		public XMLCharacterRunManager CharManager
		{
			get
			{
				if (this._charManager == null)
				{
					if (this._cell.Is == null)
					{
						this._cell.Is = new CT_Rst();
						this._cell.Is.R = new List<CT_RElt>();
						this._cell.Is.R.Add(new CT_RElt());
						this._cell.Is.R[0].T = (this.Value.ToString() ?? "");
					}
					else if (this._cell.Is.T != null)
					{
						this._cell.Is.R = new List<CT_RElt>();
						this._cell.Is.R.Add(new CT_RElt());
						this._cell.Is.R[0].T = this._cell.Is.T;
						this._cell.Is.T = null;
					}
					this._cell.T_Attr = ST_CellType.inlineStr;
					this._charManager = new XMLCharacterRunManager(this._cell.Is, this._manager.StyleSheet.Palette);
				}
				return this._charManager;
			}
		}

		public XMLCellController(CT_Cell cell, XMLWorksheetModel sheet, PartManager manager)
		{
			this._cell = cell;
			this._workbook = (XMLWorkbookModel)sheet.Workbook;
			this._sheet = sheet;
			this._manager = manager;
		}

		public void Cleanup()
		{
			if (this._style != null)
			{
				if (this._style.Index == 0)
				{
					this._cell.S_Attr = this._manager.StyleSheet.CommitStyle(this._style);
				}
				else
				{
					this._cell.S_Attr = this._style.Index;
				}
			}
		}

		public bool IsNotValidDouble(double value)
		{
			if (!double.IsNaN(value))
			{
				return double.IsInfinity(value);
			}
			return true;
		}

		public bool IsNotValidFloat(float value)
		{
			if (!float.IsNaN(value))
			{
				return float.IsInfinity(value);
			}
			return true;
		}

		private void SetDivByZeroError()
		{
			this._cell.T_Attr = ST_CellType.e;
			this._cell.V = "#DIV/0!";
		}
	}
}
