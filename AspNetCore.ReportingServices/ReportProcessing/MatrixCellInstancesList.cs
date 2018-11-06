using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixCellInstancesList : ArrayList
	{
		internal new MatrixCellInstanceList this[int index]
		{
			get
			{
				return (MatrixCellInstanceList)base[index];
			}
		}

		internal MatrixCellInstancesList()
		{
		}

		internal MatrixCellInstancesList(int capacity)
			: base(capacity)
		{
		}
	}
}
