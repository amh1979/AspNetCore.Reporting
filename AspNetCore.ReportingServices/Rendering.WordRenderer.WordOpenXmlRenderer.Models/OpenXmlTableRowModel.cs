namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTableRowModel
	{
		private readonly OpenXmlTableRowModel _containingRow;

		private InterleavingWriter _interleavingWriter;

		private OpenXmlTableRowPropertiesModel _rowProperties;

		private int[] _columnWidths;

		public OpenXmlTableRowModel ContainingRow
		{
			get
			{
				return this._containingRow;
			}
		}

		public OpenXmlTableRowPropertiesModel RowProperties
		{
			get
			{
				return this._rowProperties;
			}
		}

		public int[] ColumnWidths
		{
			get
			{
				return this._columnWidths;
			}
			set
			{
				this._columnWidths = value;
			}
		}

		public OpenXmlTableRowModel(OpenXmlTableRowModel containingRow, InterleavingWriter interleavingWriter)
		{
			this._containingRow = containingRow;
			this._interleavingWriter = interleavingWriter;
			interleavingWriter.TextWriter.Write("<w:tr>");
			InterleavingWriter interleavingWriter2 = this._interleavingWriter;
			InterleavingWriter.CreateInterleaver<OpenXmlTableRowPropertiesModel> createInterleaver = delegate(int index, long location)
			{
				this._rowProperties = new OpenXmlTableRowPropertiesModel(index, location);
				return this.RowProperties;
			};
			interleavingWriter2.WriteInterleaver(createInterleaver);
		}

		public void WriteCloseTag()
		{
			this._interleavingWriter.CommitInterleaver(this.RowProperties);
			this._interleavingWriter.TextWriter.Write("</w:tr>");
		}
	}
}
