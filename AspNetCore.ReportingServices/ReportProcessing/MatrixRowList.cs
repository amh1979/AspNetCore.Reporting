using System;
using System.Collections;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	[Serializable]
	internal sealed class MatrixRowList : ArrayList
	{
		internal new MatrixRow this[int index]
		{
			get
			{
				return (MatrixRow)base[index];
			}
		}

		internal MatrixRowList()
		{
		}

		internal MatrixRowList(int capacity)
			: base(capacity)
		{
		}
	}
}
