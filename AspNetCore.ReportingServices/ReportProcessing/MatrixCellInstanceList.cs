using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixCellInstanceList : ArrayList
	{
		internal new MatrixCellInstance this[int index]
		{
			get
			{
				return (MatrixCellInstance)base[index];
			}
		}

		internal MatrixCellInstanceList()
		{
		}

		internal MatrixCellInstanceList(int capacity)
			: base(capacity)
		{
		}
	}
}
