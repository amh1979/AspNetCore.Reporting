using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AspNetCore.Reporting
{
	internal sealed class GridLayoutCellDefinitionCollection : ReadOnlyCollection<GridLayoutCellDefinition>
	{
		private struct Index
		{
			public int Row;

			public int Column;

			public Index(int row, int column)
			{
				this.Row = row;
				this.Column = column;
			}
		}

		private readonly Dictionary<Index, GridLayoutCellDefinition> m_cellDefintionsByIndex = new Dictionary<Index, GridLayoutCellDefinition>();

		private readonly Dictionary<string, GridLayoutCellDefinition> m_cellDefintionsByName = new Dictionary<string, GridLayoutCellDefinition>(StringComparer.OrdinalIgnoreCase);

		public string this[int rowIndex, int colIndex]
		{
			get
			{
				GridLayoutCellDefinition gridLayoutCellDefinition = default(GridLayoutCellDefinition);
				if (this.m_cellDefintionsByIndex.TryGetValue(new Index(rowIndex, colIndex), out gridLayoutCellDefinition))
				{
					return gridLayoutCellDefinition.ParameterName;
				}
				return null;
			}
		}

		internal GridLayoutCellDefinitionCollection(IList<GridLayoutCellDefinition> cellDefinitions)
			: base(cellDefinitions)
		{
			foreach (GridLayoutCellDefinition cellDefinition in cellDefinitions)
			{
				this.m_cellDefintionsByIndex[new Index(cellDefinition.Row, cellDefinition.Column)] = cellDefinition;
				this.m_cellDefintionsByName[cellDefinition.ParameterName] = cellDefinition;
			}
		}

		internal GridLayoutCellDefinitionCollection()
			: base((IList<GridLayoutCellDefinition>)new GridLayoutCellDefinition[0])
		{
		}

		public GridLayoutCellDefinition GetByName(string parameterName)
		{
			GridLayoutCellDefinition result = default(GridLayoutCellDefinition);
			if (this.m_cellDefintionsByName.TryGetValue(parameterName, out result))
			{
				return result;
			}
			return null;
		}
	}
}
