using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixColumnList : ArrayList
	{
		internal new MatrixColumn this[int index]
		{
			get
			{
				return (MatrixColumn)base[index];
			}
		}

		internal MatrixColumnList()
		{
		}

		internal MatrixColumnList(int capacity)
			: base(capacity)
		{
		}
	}
}
