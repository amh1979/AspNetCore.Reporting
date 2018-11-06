using System;

namespace AspNetCore.ReportingServices.ReportProcessing
{
	internal sealed class ExpressionInfoExtended : ExpressionInfo
	{
		[NonSerialized]
		private bool m_isExtendedSimpleFieldReference;

		internal bool IsExtendedSimpleFieldReference
		{
			get
			{
				return this.m_isExtendedSimpleFieldReference;
			}
			set
			{
				this.m_isExtendedSimpleFieldReference = value;
			}
		}
	}
}
