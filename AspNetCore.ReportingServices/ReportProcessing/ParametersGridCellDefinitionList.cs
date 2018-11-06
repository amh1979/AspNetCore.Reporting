using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class ParametersGridCellDefinitionList : ArrayList
	{
		public new ParameterGridLayoutCellDefinition this[int index]
		{
			get
			{
				return (ParameterGridLayoutCellDefinition)base[index];
			}
		}

		public ParametersGridCellDefinitionList()
		{
		}

		public ParametersGridCellDefinitionList(int capacity)
			: base(capacity)
		{
		}
	}
}
