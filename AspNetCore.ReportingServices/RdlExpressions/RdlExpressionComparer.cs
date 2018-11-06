using AspNetCore.ReportingServices.ReportIntermediateFormat;
using System;
using System.Collections.Generic;

namespace AspNetCore.ReportingServices.RdlExpressions
{
	internal sealed class RdlExpressionComparer : IComparer<ExpressionInfo>
	{
		private static readonly RdlExpressionComparer m_instance = new RdlExpressionComparer();

		public static RdlExpressionComparer Instance
		{
			get
			{
				return RdlExpressionComparer.m_instance;
			}
		}

		private RdlExpressionComparer()
		{
		}

		public int Compare(ExpressionInfo x, ExpressionInfo y)
		{
			if (x == null && y == null)
			{
				return 0;
			}
			if (x == null)
			{
				return -1;
			}
			if (y == null)
			{
				return 1;
			}
			if (x.Type == ExpressionInfo.Types.Field && y.Type == ExpressionInfo.Types.Field)
			{
				return x.FieldIndex - y.FieldIndex;
			}
			return StringComparer.Ordinal.Compare(x.OriginalText, y.OriginalText);
		}

		public bool Equals(ExpressionInfo x, ExpressionInfo y)
		{
			return this.Compare(x, y) == 0;
		}
	}
}
