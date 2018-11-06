using System;

namespace AspNetCore.Reporting.Chart.WebForms
{
	[Flags]
	internal enum DrawingOperationTypes
	{
		DrawElement = 1,
		CalcElementPath = 2
	}
}
