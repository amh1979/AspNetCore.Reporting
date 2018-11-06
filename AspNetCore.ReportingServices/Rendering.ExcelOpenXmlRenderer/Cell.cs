using AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer.Model;

namespace AspNetCore.ReportingServices.Rendering.ExcelOpenXmlRenderer
{
	internal sealed class Cell
	{
		internal sealed class CellValueType
		{
			private readonly int mValue;

			private readonly string mName;

			public static readonly CellValueType Blank;

			public static readonly CellValueType Boolean;

			public static readonly CellValueType Currency;

			public static readonly CellValueType Date;

			public static readonly CellValueType Double;

			public static readonly CellValueType Error;

			public static readonly CellValueType Integer;

			public static readonly CellValueType Text;

			public static readonly CellValueType Time;

			public int Value
			{
				get
				{
					return this.mValue;
				}
			}

			static CellValueType()
			{
				CellValueType.Blank = new CellValueType("Blank", 4);
				CellValueType.Boolean = new CellValueType("Boolean", 5);
				CellValueType.Currency = new CellValueType("Currency", 7);
				CellValueType.Date = new CellValueType("Date", 3);
				CellValueType.Double = new CellValueType("Double", 2);
				CellValueType.Error = new CellValueType("Error", 6);
				CellValueType.Integer = new CellValueType("Integer", 1);
				CellValueType.Text = new CellValueType("Text", 0);
				CellValueType.Time = new CellValueType("Time", 8);
			}

			private CellValueType(string aName, int aValue)
			{
				this.mName = aName;
				this.mValue = aValue;
			}

			public override bool Equals(object aObject)
			{
				if (aObject is CellValueType)
				{
					CellValueType cellValueType = (CellValueType)aObject;
					return this.Value == cellValueType.Value;
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

		private readonly ICellModel mModel;

		public string Name
		{
			get
			{
				return this.mModel.Name;
			}
		}

		public Style Style
		{
			set
			{
				this.mModel.Style = ((value == null) ? null : value.Model);
			}
		}

		public object Value
		{
			get
			{
				return this.mModel.Value;
			}
			set
			{
				this.mModel.Value = value;
			}
		}

		public CellValueType ValueType
		{
			get
			{
				return this.mModel.ValueType;
			}
		}

		internal Cell(ICellModel model)
		{
			this.mModel = model;
		}

		public override bool Equals(object obj)
		{
			if (obj != null && obj is Cell)
			{
				if (obj == this)
				{
					return true;
				}
				Cell cell = (Cell)obj;
				return cell.mModel.Equals(this.mModel);
			}
			return false;
		}

		public CharacterRun GetCharacters(int startIndex, int length)
		{
			return this.mModel.getCharacters(startIndex, length).Interface;
		}

		public override int GetHashCode()
		{
			return this.mModel.GetHashCode();
		}
	}
}
