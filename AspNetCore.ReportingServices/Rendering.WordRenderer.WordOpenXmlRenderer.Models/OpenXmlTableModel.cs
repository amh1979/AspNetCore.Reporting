namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class OpenXmlTableModel
	{
		private readonly OpenXmlTableModel _containingTable;

		private InterleavingWriter _interleavingWriter;

		private OpenXmlTablePropertiesModel _tableProperties;

		private bool _writtenProperties;

		private OpenXmlTableGridModel _tableGrid;

		public OpenXmlTableModel ContainingTable
		{
			get
			{
				return this._containingTable;
			}
		}

		public OpenXmlTablePropertiesModel TableProperties
		{
			get
			{
				return this._tableProperties;
			}
		}

		public OpenXmlTableGridModel TableGrid
		{
			get
			{
				return this._tableGrid;
			}
		}

		public OpenXmlTableModel(OpenXmlTableModel containingTable, InterleavingWriter interleavingWriter, AutoFit autofit)
		{
			this._containingTable = containingTable;
			this._interleavingWriter = interleavingWriter;
			this._interleavingWriter.TextWriter.Write("<w:tbl>");
			this._tableProperties = new OpenXmlTablePropertiesModel(autofit);
		}

		public void WriteProperties()
		{
			if (!this._writtenProperties)
			{
				this._tableProperties.Write(this._interleavingWriter.TextWriter);
				this._interleavingWriter.WriteInterleaver(delegate(int index, long location)
				{
					this._tableGrid = new OpenXmlTableGridModel(index, location);
					return this.TableGrid;
				});
				this._writtenProperties = true;
			}
		}

		public void WriteCloseTag()
		{
			this.WriteProperties();
			this._interleavingWriter.CommitInterleaver(this._tableGrid);
			this._interleavingWriter.TextWriter.Write("</w:tbl>");
		}
	}
}
