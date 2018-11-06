using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Reporting
{
	internal sealed class GridLayoutDefinition
	{
		private readonly int m_numberOfColumns;

		private readonly int m_numberOfRows;

		private readonly GridLayoutCellDefinitionCollection m_cellDefinitions;

		private readonly bool[] m_rowsVisibility;

		private readonly bool[] m_columnsVisibility;

		public int NumberOfColumns
		{
			get
			{
				return this.m_numberOfColumns;
			}
		}

		public int NumberOfRows
		{
			get
			{
				return this.m_numberOfRows;
			}
		}

		public GridLayoutCellDefinitionCollection CellDefinitions
		{
			get
			{
				return this.m_cellDefinitions;
			}
		}

		public bool[] RowsVisibility
		{
			get
			{
				return this.m_rowsVisibility;
			}
		}

		public bool[] ColumnsVisibility
		{
			get
			{
				return this.m_columnsVisibility;
			}
		}

		internal GridLayoutDefinition(GridLayoutCellDefinitionCollection cellDefs, int numRows, int numColumns, ReportParameterInfoCollection paramInfoCollection)
		{
			this.m_cellDefinitions = cellDefs;
			this.m_numberOfRows = numRows;
			this.m_numberOfColumns = numColumns;
			this.m_columnsVisibility = new bool[numColumns];
			this.m_rowsVisibility = new bool[numRows];
			if (paramInfoCollection != null && paramInfoCollection.Any())
			{
				int i;
				for (i = 0; i < this.m_numberOfColumns; i++)
				{
					IEnumerable<GridLayoutCellDefinition> enumerable = from p in this.m_cellDefinitions
					where p.Column == i
					select p;
					if (!enumerable.Any() && i < this.m_cellDefinitions.Max((GridLayoutCellDefinition p) => p.Column))
					{
						this.m_columnsVisibility[i] = true;
					}
					foreach (GridLayoutCellDefinition item in enumerable)
					{
						Func<ReportParameterInfo, bool> predicate = delegate(ReportParameterInfo p)
						{
							if (p.Name == item.ParameterName)
							{
								return this.IsParamVisible(p);
							}
							return false;
						};
						if (paramInfoCollection.Any(predicate))
						{
							this.m_columnsVisibility[i] = true;
							this.m_rowsVisibility[item.Row] = true;
						}
					}
				}
				int j;
				for (j = 0; j < this.m_numberOfRows; j++)
				{
					IEnumerable<GridLayoutCellDefinition> source = from p in this.m_cellDefinitions
					where p.Row == j
					select p;
					if (!source.Any() && j < this.m_cellDefinitions.Max((GridLayoutCellDefinition p) => p.Row))
					{
						this.m_rowsVisibility[j] = true;
					}
				}
			}
		}

		private bool IsParamVisible(ReportParameterInfo param)
		{
			if (!param.PromptUser)
			{
				return false;
			}
			if (!string.IsNullOrEmpty(param.Prompt))
			{
				return param.Visible;
			}
			return false;
		}
	}
}
