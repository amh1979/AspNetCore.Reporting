namespace AspNetCore.ReportingServices.Rendering.WordRenderer.WordOpenXmlRenderer.Models
{
	internal sealed class TableContext
	{
		internal enum State
		{
			InBody,
			InHeaderFooter,
			InTable,
			InRow,
			InCell
		}

		private OpenXmlTableModel _currentTable;

		private OpenXmlTableRowModel _currentRow;

		private OpenXmlTableCellModel _currentCell;

		private State _state;

		private InterleavingWriter _interleavingWriter;

		private bool _startedInHeaderFooter;

		private int _depth;

		public OpenXmlTableModel CurrentTable
		{
			get
			{
				return this._currentTable;
			}
		}

		public OpenXmlTableRowModel CurrentRow
		{
			get
			{
				return this._currentRow;
			}
		}

		public OpenXmlTableCellModel CurrentCell
		{
			get
			{
				return this._currentCell;
			}
		}

		public State Location
		{
			get
			{
				return this._state;
			}
		}

		public int Depth
		{
			get
			{
				return this._depth;
			}
		}

		public TableContext(InterleavingWriter interleavingWriter, bool inHeaderFooter)
		{
			this._interleavingWriter = interleavingWriter;
			this._state = (State)(inHeaderFooter ? 1 : 0);
			this._startedInHeaderFooter = inHeaderFooter;
			this._depth = 0;
		}

		public void WriteTableBegin(float left, bool layoutTable, AutoFit autofit)
		{
			if (this._state == State.InBody || this._state == State.InHeaderFooter || this._state == State.InCell)
			{
				if (this._state == State.InCell)
				{
					this._currentCell.PrepareForNestedTable();
				}
				this._state = State.InTable;
				this._currentTable = new OpenXmlTableModel(this.CurrentTable, this._interleavingWriter, autofit);
				this._depth++;
			}
			else
			{
				WordOpenXmlUtils.FailCodingError();
			}
		}

		public void WriteTableRowBegin(float left, float height, float[] columnWidths)
		{
			if (this._state == State.InTable)
			{
				this._currentTable.WriteProperties();
				int[] array = new int[columnWidths.Length];
				int num = 0;
				for (int i = 0; i < array.Length; i++)
				{
					int num2 = WordOpenXmlUtils.ToTwips(columnWidths[i]);
					if ((float)(num + num2) > 31680.0)
					{
						num2 = (int)(31680.0 - (float)num);
					}
					num += num2;
					array[i] = num2;
				}
				this._currentTable.TableGrid.AddRow(array);
				this._state = State.InRow;
				this._currentRow = new OpenXmlTableRowModel(this.CurrentRow, this._interleavingWriter);
				this._currentRow.RowProperties.Height = height;
				this._currentRow.ColumnWidths = array;
				this._currentRow.RowProperties.RowIndent = left;
			}
			else
			{
				WordOpenXmlUtils.FailCodingError();
			}
		}

		public void WriteTableCellBegin(int cellIndex, int numColumns, bool firstVertMerge, bool firstHorzMerge, bool vertMerge, bool horzMerge)
		{
			if (this._state == State.InRow)
			{
				this._state = State.InCell;
				this._currentCell = new OpenXmlTableCellModel(this.CurrentCell, this._currentTable.TableProperties, this._interleavingWriter.TextWriter);
				this._currentCell.CellProperties.Width = this._currentRow.ColumnWidths[cellIndex];
				if (firstHorzMerge)
				{
					this._currentCell.CellProperties.HorizontalMerge = OpenXmlTableCellPropertiesModel.MergeState.Start;
				}
				else if (horzMerge)
				{
					this._currentCell.CellProperties.HorizontalMerge = OpenXmlTableCellPropertiesModel.MergeState.Continue;
				}
				if (firstVertMerge)
				{
					this._currentCell.CellProperties.VerticalMerge = OpenXmlTableCellPropertiesModel.MergeState.Start;
				}
				else if (vertMerge)
				{
					this._currentCell.CellProperties.VerticalMerge = OpenXmlTableCellPropertiesModel.MergeState.Continue;
				}
				this._currentCell.CellProperties.BackgroundColor = this._currentTable.TableProperties.BackgroundColor;
			}
			else
			{
				WordOpenXmlUtils.FailCodingError();
			}
		}

		public void WriteTableEnd()
		{
			if (this._state == State.InTable)
			{
				this.CurrentTable.WriteCloseTag();
				this._currentTable = this.CurrentTable.ContainingTable;
				if (this.CurrentTable == null)
				{
					this._state = (State)(this._startedInHeaderFooter ? 1 : 0);
				}
				else
				{
					this._state = State.InCell;
				}
				this._depth--;
			}
			else
			{
				WordOpenXmlUtils.FailCodingError();
			}
		}

		public void WriteTableRowEnd()
		{
			if (this._state == State.InRow)
			{
				this.CurrentRow.WriteCloseTag();
				this._currentRow = this.CurrentRow.ContainingRow;
				this._state = State.InTable;
			}
			else
			{
				WordOpenXmlUtils.FailCodingError();
			}
		}

		public void WriteTableCellEnd(int cellIndex, BorderContext borderContext, bool emptyLayoutCell)
		{
			if (this._state == State.InCell)
			{
				this.CurrentCell.WriteCloseTag(emptyLayoutCell);
				this._currentCell = this.CurrentCell.ContainingCell;
				this._state = State.InRow;
			}
			else
			{
				WordOpenXmlUtils.FailCodingError();
			}
		}
	}
}
